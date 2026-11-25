# Réponses — TP3 C# : Exceptions, JSON, async et HTTP

## Étape 1 : Exceptions — valider et signaler les erreurs

### Observations (Questions 2 à 6)
* **Validation dans le `record Note`** : l'ajout des contrôles dans le constructeur (intervalle `[0, 20]` pour `Valeur` et `ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Coef)`) garantit qu'aucune instance invalide de `Note` ne peut exister en mémoire, qu'elle provienne du code, du CSV ou d'un fichier JSON.
* **Lecture de `notes.csv` avec `ParserStrict`** : après l'ajout des deux lignes erronées (note à `25` et ligne à 3 champs), le programme charge **13 notes valides** et rejette **3 lignes invalides** sur `Console.Error` en précisant à chaque fois le numéro de ligne et la cause exacte (`FormatException` ou `ArgumentOutOfRangeException`).

### Questions de réflexion (Étape 1)
* **Pourquoi intercepter `FormatException` et `ArgumentOutOfRangeException`, et pas simplement `Exception` ?**
  Intercepter uniquement ces deux exceptions ciblées permet de ne gérer que les erreurs attendues liées aux données d'entrée (ligne mal formatée ou valeur hors bornes). À l'inverse, attraper `Exception` masquerait des erreurs critiques ou des bugs imprévus (problème disque `IOException`, manque de mémoire, `NullReferenceException`, etc.) qui doivent faire échouer le programme.
* **Pour lire une note, quand faut-il préférer `double.TryParse` à `double.Parse` dans un `try` ? Justifiez votre choix dans `ParserStrict`.**
  `double.TryParse` est préférable dès que l'échec de conversion est un cas courant (lecture de fichiers externes ou saisies utilisateur), car lever et attraper une exception coûte beaucoup plus cher en performances qu'un simple test booléen. Dans `ParserStrict`, `double.TryParse` permet en plus de détecter l'erreur proprement pour lever une `FormatException` personnalisée contenant le **numéro de ligne** et la **raison précise** de l'échec.
* **Quelle différence entre `throw;` et `throw e;` dans un `catch` ?**
  * `throw;` relance l'exception d'origine en **conservant intacte la pile d'appels (*stack trace*)** jusqu'à la ligne exacte où l'erreur s'est produite (par exemple dans le constructeur de `Note`).
  * `throw e;` **réinitialise la pile d'appels** à la ligne du bloc `catch`, masquant l'origine réelle de l'exception lors du débogage.

---

## Étape 2 : Sauvegarder et recharger la promo en JSON

### Observations (Questions 8 à 11)
* **Formatage des propriétés dans `promo.json` (Q8)** : les noms des propriétés s'écrivent en **camelCase** (`nom`, `prenom`, `matiere`, `valeur`, `coef`), car `JsonSerializerDefaults.Web` configure par défaut `PropertyNamingPolicy = JsonNamingPolicy.CamelCase` (standard des échanges Web/JSON) et active la désérialisation insensible à la casse.
* **Fichier `promo.json` abîmé (Q10)** : supprimer une accolade provoque une `JsonException` lors de la désérialisation. En l'interceptant proprement, le programme affiche un message d'erreur explicite puis se rabat sur l'importation de `notes.csv` pour régénérer un `promo.json` valide.
* **Modification d'une note à `30` dans `promo.json` (Q11)** : au chargement, `JsonSerializer.Deserialize` invoque le constructeur principal du `record Note`, ce qui déclenche la validation et lève une `ArgumentOutOfRangeException`. **C'est bien le comportement souhaité** : les règles métier du modèle restent appliquées même si le fichier JSON est modifié manuellement depuis l'extérieur.

### Questions de réflexion (Étape 2)
* **`Deserialize` renvoie `List<Note>?`. Dans quel cas obtient-on `null` ? Comment le traitez-vous ?**
  `Deserialize` renvoie `null` lorsque le document JSON contient littéralement le jeton JSON `null` (au lieu d'un tableau `[]`). On le traite en utilisant l'opérateur de coalescence nulle (`?? []`) afin de toujours manipuler une liste instanciée et d'éviter toute `NullReferenceException`.
* **Pourquoi utiliser `Path.Combine` plutôt que d'écrire les chemins à la main, surtout si le programme tourne dans Docker ?**
  Les séparateurs de répertoires diffèrent selon le système d'exploitation (`\` sous Windows contre `/` sous Linux, utilisé par la majorité des conteneurs Docker). `Path.Combine` construit le chemin avec `Path.DirectorySeparatorChar` adapté à l'OS d'exécution, garantissant la portabilité du programme.

---

## Étape 3 : Passer en asynchrone

### Observations (Questions 13 à 16)
* **Suppression temporaire d'un `await` (Q13)** :
  * **Avertissement du compilateur** : `CS4014` (*appel asynchrone non attendu : l'exécution de la méthode courante continue avant la fin de l'appel*).
  * **Comportement à l'exécution** : la tâche s'exécute en arrière-plan de manière non synchronisée (*fire-and-forget*). Le programme peut tenter de lire un fichier pas encore écrit, se terminer avant la fin de la sauvegarde, ou perdre silencieusement les exceptions levées dans la tâche.
* **Mesures avec `Stopwatch` (Q14 et Q15)** :
  * Trois appels de 1 seconde attendus séquentiellement (`await` l'un après l'autre) prennent environ **3 secondes** (`~3000 ms`).
  * Les trois mêmes appels lancés ensemble puis attendus via `Task.WhenAll` prennent environ **1 seconde** (`~1000 ms`), car les attentes s'exécutent en parallèle.
* **Appels avec `.Result` au lieu de `await` (Q16)** :
  Dans une application console, le programme fonctionne (en ~3 secondes), **mais `.Result` est fortement déconseillé** :
  1. Il bloque de manière synchrone le thread appelant pendant toute l'attente I/O, annulant le bénéfice de l'asynchrone.
  2. Dans un environnement possédant un `SynchronizationContext` (application graphique, certains serveurs), il provoque un **interblocage (*deadlock*)**.
  3. Il encapsule les exceptions dans une `AggregateException`, compliquant leur interception.

---

## Étape 4 : Interroger l'API `geo.api.gouv.fr`

### Observations (Questions 17 à 24)
* **Type choisi pour `Population` dans `Commune` (Q17)** : **`int?`** (entier nullable). La population d'une commune est un entier positif tenant largement dans un `int` 32 bits, mais certaines communes inhabitées ou cas particuliers renvoyés par l'API omettent le champ `population` ou renvoient `null` dans le JSON.
* **Réponse pour le code postal `99999` (Q19)** : l'API répond avec un statut HTTP `200 OK` et un tableau JSON vide `[]`. La méthode `CommunesAsync` retourne donc une `List<Commune>` vide (`?? []`), jamais `null`.
* **Comparaison `Stopwatch` parallèle vs séquentiel (Q22)** : avec `Task.WhenAll`, les 6 requêtes partent simultanément et la durée totale correspond uniquement au temps de réponse de la requête la plus lente, contre la somme des 6 latences réseau en séquentiel.
* **Réseau coupé / URL invalide `https://geo.api.gouv.invalid/` (Q23)** : l'appel lève une **`HttpRequestException`** (liée à l'échec de résolution DNS).
* **Dépassement du délai avec `CancellationTokenSource` (Q24)** : lorsque le délai (ex. `1 ms`) expire avant la réponse, une **`TaskCanceledException`** (sous-classe de `OperationCanceledException`) est levée.

### Questions de réflexion (Étape 4)
* **Pourquoi ne faut-il pas créer un nouveau `HttpClient` pour chaque requête ?**
  Chaque instance de `HttpClient` gère son propre pool de connexions TCP/TLS. Instancier et libérer (`Dispose`) un `HttpClient` à chaque appel empêche la réutilisation des connexions et laisse les sockets système en état **`TIME_WAIT`**, ce qui finit par épuiser les ports disponibles de la machine (**épuisement des sockets / `SocketException`**).
* **Dans la question 21, pourquoi une erreur sur une requête ne doit-elle pas faire échouer toutes les autres ? Comment l'avez-vous garanti ?**
  Par défaut, `Task.WhenAll` échoue globalement dès qu'une seule tâche lève une exception non interceptée, ce qui empêcherait d'afficher les résultats des autres étudiants. Pour l'éviter, on encapsule la gestion des erreurs (`try/catch` sur `HttpRequestException` et gestion des listes vides) **à l'intérieur de la méthode asynchrone appelée pour chaque étudiant**, de sorte que chaque tâche se termine toujours avec un résultat exploitable (`"commune inconnue"` en cas d'échec) sans interrompre les autres.
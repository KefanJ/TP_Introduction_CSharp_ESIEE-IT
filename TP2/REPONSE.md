# Réponses — TP2 C# : Délégués, lambdas et LINQ

## Exercice 0 : Mise en place et lecture en LINQ

### Observations (Questions 1 à 3)
* **Lecture des fichiers CSV** : le programme charge **13 notes valides** (la ligne `Liskov, Barbara, Algo, abc, 3` est écartée car la note n'est pas numérique) et **6 inscrits**.
* **`Where(n => n is not null)` vs `OfType<Note>()` (Q2)** :
  * `Where(n => n is not null)` conserve le type **`IEnumerable<Note?>`** : le compilateur considère toujours que les éléments peuvent être `null`.
  * `OfType<Note>()` filtre les valeurs nulles **et** change le type de retour en **`IEnumerable<Note>`** (non nullable), ce qui évite d'avoir à utiliser l'opérateur `!` ou d'autres vérifications de nullité par la suite.

---

## Exercice 1 : Délégués et lambdas

### 1.1 Un filtre générique (Question 3)
* **Type de la lambda stockée** : `Func<Note, bool>` (ou `Predicate<Note>` si déclaré explicitement ainsi).
* **Peut-on écrire `var c = n => n.Valeur >= 15;` ?**
  **Non.** Le compilateur génère une erreur (`CS0815` / `CS8917` : impossible d'inférer le type délégué d'une expression lambda implicitement typée), car le type du paramètre `n` n'est pas connu sans le type cible `Func<Note, bool>` (ou sans typer explicitement le paramètre `(Note n) => ...`).

### 1.3 La capture de variables (Questions)
* **Valeurs affichées** :
  * Premier affichage (`seuil = 12`) : **`10`** notes.
  * Second affichage (`seuil = 15`) : **`6`** notes.
  * *Explication* : la lambda capture une **référence** à la variable locale `seuil` (*closure*), et non sa valeur au moment de sa création. Quand `seuil` est modifié à `15`, le second appel à `Filtrer` évalue la condition avec la nouvelle valeur.
* **Différence avec Java** :
  En Java, ce code **ne compile pas**. Les variables locales capturées par une expression lambda doivent être `final` ou *effectively final* (jamais réaffectées). En C#, le compilateur génère une classe cachée qui encapsule la variable `seuil` dans un champ, autorisant sa modification ultérieure.
* **Comment garder le seuil fixe à 12 même si `seuil` change ensuite ?**
  En copiant la valeur dans une variable locale dédiée avant la lambda (`double seuilFixe = seuil; Func<Note, bool> admis = n => n.Valeur >= seuilFixe;`) ou en appelant une méthode fabrique comme `AuDessusDe(seuil)` (où le paramètre est passé par valeur).

---

## Exercice 2 : Premières requêtes

### 2.1 Filtrer, projeter, trier
* **Double `OrderBy` vs `ThenBy` (Q4)** : chaîner deux `OrderBy` successifs (`OrderBy(n => n.Matiere).OrderByDescending(n => n.Valeur)`) **écrase le premier tri** : la liste finale n'est triée que par valeur décroissante. Il faut utiliser **`.OrderBy(n => n.Matiere).ThenByDescending(n => n.Valeur).ThenBy(n => n.Prenom)`** pour conserver le tri principal et appliquer les tris secondaires.

### 2.2 Agréger
* **Statistiques globales (Q1 & Q3)** : **13 notes**, dont **1** note inférieure à 10 (`Alan` en `Algo` avec `9`), moyenne simple de **13,77**, moyenne pondérée de **13,72**.
* **`Max` vs `MaxBy` (Q2)** :
  * `notes.Max(n => n.Valeur)` renvoie uniquement la **valeur maximale** (`double` : `18`).
  * `notes.MaxBy(n => n.Valeur)` renvoie **l'objet `Note` complet** qui atteint ce maximum (`Grace Hopper` en `Algo`, note `18`, coef `3`).
* **`Aggregate` vs `string.Join` (Q4)** :
  `string.Join(", ", prenoms)` est nettement préférable : la syntaxe est plus concise, plus lisible, et bien plus performante (elle alloue directement le tampon final au lieu de créer une nouvelle chaîne de caractères à chaque itération dans `Aggregate`).

### 2.3 Chercher et tester
* **`FirstOrDefault` vs `First` sur la note de Linus en BD (Q1)** :
  Linus n'a aucune note en BD. `FirstOrDefault` renvoie **`null`**, tandis que `First` lève une **`InvalidOperationException`** (*Sequence contains no matching element*).
* **`Single` sur les notes d'Ada (Q2)** :
  Sur les notes d'Ada en `Algo`, `Single` renvoie sa note (`15`). Sur *toutes* les notes d'Ada, `Single` lève une **`InvalidOperationException`** (*Sequence contains more than one element*), car Ada possède 3 notes et `Single` exige strictement un seul élément.
* **`Any` et `All` (Q3)** :
  * Existe-t-il une note inférieure à 10 (`Any`) ? **`true`** (`Alan`, `9` en `Algo`).
  * Tous les étudiants ont-ils au moins 12 en BD (`All`) ? **`true`** (`Ada` 16, `Alan` 12, `Grace` 17, `Barbara` 13).
  * Que renvoie `All` sur une séquence vide ? **`true`** (vérité par vacuité : aucun élément de la séquence vide ne contredit la condition).

---

## Exercice 3 : Regrouper et joindre

### 3.1 GroupBy
* **Type d'un groupe (Q1)** : **`IGrouping<string, Note>`** (une collection d'éléments `Note` associée à une propriété `Key` de type `string`).
* **Statistiques par matière, triées par moyenne décroissante (Q2)** :
  * **BD** : 4 notes, moyenne **14,50**, min `12`, max `17`, major : **Grace**
  * **Réseau** : 5 notes, moyenne **13,60**, min `11`, max `16`, major : **Linus** (premier dans l'ordre du fichier à égalité avec Ada à `16`)
  * **Algo** : 4 notes, moyenne **13,25**, min `9`, max `18`, major : **Grace**
* **Moyennes pondérées par étudiant (Q3)** :
  `Grace` : **16,67** | `Ada` : **15,17** | `Barbara` : **13,67** | `Linus` : **12,25** | `Alan` : **10,33**.
* **`CountBy` vs `GroupBy` + `Count` (Q4)** :
  `CountBy(n => n.Prenom)` est plus direct et plus léger en mémoire : il compte directement les occurrences par clé sans matérialiser chaque groupe complet de notes, et renvoie une séquence de `KeyValuePair<string, int>`.

### 3.2 Join
* **Moyenne simple par promo (Q2)** :
  * **M1** (`Ada`, `Alan`, `Linus`) : **13,00**
  * **M2** (`Grace`, `Barbara`, `Margaret`) : **15,00**
* **Absence de Margaret Hamilton dans le relevé (Q3)** :
  `Join` effectue une **jointure interne (*INNER JOIN*)**. Comme Margaret Hamilton n'a aucune note dans `notes.csv`, elle n'a aucune correspondance et est exclue du résultat. On la retrouve en filtrant avec `!notes.Any(...)` ou via une jointure externe gauche avec `GroupJoin`.
* **`ToLookup` pour un prénom absent (Q4)** :
  Contrairement à un `Dictionary` (qui lèverait une `KeyNotFoundException`), interroger un `ILookup` avec une clé absente (ex. `"Margaret"`) renvoie une **séquence vide** sans lever d'exception.

### 3.3 SelectMany
* **`Select` vs `SelectMany` (Q2)** :
  `bulletins.Select(b => b.Valeurs)` renvoie une collection imbriquée de type **`IEnumerable<List<double>>`**, tandis que `bulletins.SelectMany(b => b.Valeurs)` aplatit toutes les listes en une seule séquence continue de type **`IEnumerable<double>`**.

---

## Exercice 4 : L'évaluation différée

* **Valeurs affichées par le bloc de code (Q1)** :
  1. `bonnes.Count()` : **`6`** (les 6 notes `>= 15` d'origine).
  2. `bonnes.Count()` après ajout de `19` : **`7`**. Comme `bonnes` est une requête différée (`IEnumerable<Note>`), elle réévalue le filtre sur l'état actuel de la liste `notes` à chaque parcours.
  3. `bonnes.Count() + "/" + figees.Count` après ajout de `20` : **`8/7`**. `bonnes` reflète les 8 notes actuelles `>= 15`, alors que `figees` a été figée en mémoire par `.ToList()` au moment où il n'y en avait que 7.
* **Effet de bord dans `Where` (Q2)** :
  * Sans `.ToList()`, appeler `Count()` deux fois exécute le prédicat **2 fois** sur chaque note (la requête est rejouée à chaque parcours).
  * Avec `.ToList()` d'abord, chaque note n'est testée qu'**1 seule fois** lors de la matérialisation ; les accès suivants lisent la liste en mémoire.
* **`Any()` au lieu de `Count() > 0` (Q3)** :
  **1 seule note** est testée : dès la première note (`Lovelace`, `Algo`, `15`), la condition est vraie et `Any()` court-circuite immédiatement le parcours, contrairement à `Count()` qui parcourt toute la collection.
* **`Where` avant vs après `OrderBy` (Q4)** :
  Le résultat final est identique, mais les performances diffèrent :
  * Avec `Where` **avant** `OrderBy`, seules les **6 notes** filtrées sont triées.
  * Avec `OrderBy` **avant** `Where`, les **13 notes** sont d'abord toutes triées avant d'être filtrées. Il faut toujours filtrer avant de trier.

---

## Exercice 5 : Réécrire la classe `Promo`

* **Division par zéro dans `Moyenne(string prenom)` (Q1)** :
  Si un étudiant n'a aucune note, calculer `Sum(n => n.Valeur * n.Coef) / Sum(n => n.Coef)` revient à faire `0.0 / 0`, ce qui renvoie **`double.NaN`**. On l'évite en testant `.Any()` sur les notes de l'étudiant et en renvoyant `0` si la séquence est vide.
* **Comparaison avec la séance 1 (Q3)** :
  Le code écrit en LINQ est nettement plus **concis** (quelques lignes déclaratives par méthode au lieu de multiples boucles `foreach`, listes temporaires et variables accumulatrices), plus **lisible** (l'intention métier apparaît directement dans le chaînage des opérateurs) et plus simple à maintenir.
* **Recalcul et mise en cache de `Classement()` (Q4)** :
  **Oui**, `Classement()` est recalculée à chaque appel car elle retourne une requête LINQ différée. Sur une petite promotion de quelques dizaines de notes, le coût est négligeable. En revanche, il faudrait la mettre en cache (en stockant le résultat de `.ToList()` dans un champ privé invalidé uniquement lors de la modification des notes) si la promotion comportait des milliers d'étudiants ou si `Classement()` était appelée de manière répétée dans une boucle ou une interface graphique.
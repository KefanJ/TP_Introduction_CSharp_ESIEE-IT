# Réponses — TP4 Entity Framework Core

## Étape 1 : Le modèle et la base

### Question 6 — Analyse de la première migration

- **Tables créées** : `Etudiants`, `Notes`, ainsi que `__EFMigrationsHistory` qui suit l'historique des migrations appliquées[cite: 40].
- **Clé étrangère et suppression** : `FK_Notes_Etudiants_EtudiantId` relie `Notes.EtudiantId` à `Etudiants.Id` avec `onDelete: ReferentialAction.Cascade`[cite: 40]. Supprimer un étudiant entraîne donc la suppression automatique de toutes ses notes[cite: 17, 40].

### Questions de réflexion (Étape 1)

- **Pourquoi `Nom` et `Matiere` sont-elles `NOT NULL` ?**
  En C#, ces propriétés sont déclarées avec le type non annulable `string` (et non `string?`)[cite: 16, 17]. Par convention, EF Core traduit tout type `string` non annulable en colonne `NOT NULL` en SQL (tandis que `required` oblige à les renseigner lors de l'instanciation en C#)[cite: 15, 16, 17].
- **Comment EF Core a-t-il deviné que `EtudiantId` est une clé étrangère ?**
  Grâce à ses conventions de nommage : en repérant la propriété de navigation `Etudiant` (de type `Etudiant`) dans `Note`, il associe automatiquement la propriété `EtudiantId` (`<NomNavigation>Id`) comme clé étrangère vers la clé primaire `Id` de `Etudiants`[cite: 17].

---

## Étape 2 : Insérer la promo

### Question 8 — Second lancement du programme

- Lors du deuxième `dotnet run`, aucune donnée n'est insérée[cite: 41]. EF Core exécute la requête `SELECT EXISTS (SELECT 1 FROM "Etudiants" AS "e")` pour évaluer `db.Etudiants.Any()` : comme `promo.db` contient déjà les 6 étudiants, la condition est vraie et le bloc d'insertion est ignoré[cite: 41].

## Étape 3 : Interroger la base

### Question 9 — Étudiants de M2 triés par nom

- **Traduction SQL** : `.Where(e => e.Promo == "M2").OrderBy(e => e.Nom)` est traduit par EF Core en une requête SQL avec les clauses `WHERE "e"."Promo" = 'M2'` et `ORDER BY "e"."Nom"`.
- **Résultat affiché** : `Hamilton Margaret`, `Hopper Grace`, `Liskov Barbara`.

### Question 10 — Projection et comptage des notes sans `Include`

- **Comportement SQL** : utiliser `e.Notes.Count` à l'intérieur d'un `.Select(...)` permet à EF Core de traduire le comptage via une sous-requête SQL `(SELECT COUNT(*) FROM "Notes" ...)`, sans avoir à charger les objets `Note` en mémoire ni à utiliser `Include`.
- **Résultat affiché** : `Lovelace (3)`, `Turing (3)`, `Hopper (3)`, `Torvalds (2)`, `Liskov (2)`, `Hamilton (0)`.

### Question 11 — Moyenne par matière

- **Exécution côté base** : l'enchaînement `.GroupBy(n => n.Matiere)` et `.Average(n => n.Valeur)` est traduit directement en SQL avec `GROUP BY` et `AVG()`. Le calcul est donc effectué par SQLite, qui ne renvoie au programme C# que les 3 résultats agrégés (`Algo : 13.25`, `BD : 14.50`, `Reseau : 13.60`).

### Question 12 — Chargement des données liées (`Include`)

- **Sans `Include`** : la console affiche `0` note pour chaque étudiant. Par défaut, EF Core ne charge jamais les propriétés de navigation d'office (_lazy loading_ désactivé) afin d'éviter de rapatrier inutilement des données en mémoire.
- **Avec `Include(e => e.Notes)`** : EF Core génère un `LEFT JOIN "Notes"` dans la requête SQL (_eager loading_) et remplit la collection `Notes` de chaque étudiant.
- **Piège de performance (problème des requêtes N+1)** : charger les notes de chaque étudiant par une requête séparée à l'intérieur d'une boucle `foreach` génèrerait `1` requête pour lister les étudiants suivie de `N` requêtes individuelles pour leurs notes, dégradant fortement les performances à cause des allers-retours avec la base.

### Question 13 — Moyenne pondérée par étudiant

- **Traitement** : le filtre `.Where(e => e.Notes.Any())` écarte les étudiants sans note (`Hamilton`) et évite toute division par zéro lors du calcul `Sum(n => n.Valeur * n.Coef) / Sum(n => n.Coef)`, avant de trier les résultats par ordre décroissant avec `.OrderByDescending(...)`.

## Étape 4 : Modifier, supprimer, faire évoluer

### Question 14 — Passage d'Alan Turing en M2

- **État et SQL généré** : avant `SaveChanges()`, `db.Entry(alan).State` affiche `Modified`. EF Core génère uniquement `UPDATE "Etudiants" SET "Promo" = @p0 WHERE "Id" = @p1`, ne mettant à jour que la colonne modifiée.

### Question 15 — Suppression de Linus Torvalds

- **Notes restantes** : il reste **11 notes** dans la base (13 initiales − les 2 notes de Linus, automatiquement supprimées par la contrainte `ON DELETE CASCADE`).

### Question 16 — Mise à jour en masse avec `ExecuteUpdate`

- **Lignes modifiées** : **4 lignes** sont modifiées (les notes de Réseau d'Ada, Alan, Grace et Barbara). `ExecuteUpdate` exécute directement la requête SQL `UPDATE` sans passer par `SaveChanges()`.

### Question 17 — Migration `AjoutEmail`

- **Évolution du schéma** : le fichier de migration généré ne contient qu'un `AddColumn` sur la table `Etudiants`. Après `dotnet ef database update`, les **5 étudiants** sont toujours présents dans la base et leur propriété `Email` vaut `null` (colonne SQL nullable).

### Questions de réflexion (Étape 4)

- **Pourquoi n'a-t-on appelé aucune méthode `Update` à la question 14 ?**
  Parce que l'entité `alan` a été chargée depuis le `DbContext` et est donc suivie par son _Change Tracker_ (état initial `Unchanged`). Dès qu'on modifie une propriété en C#, EF Core détecte le changement par comparaison avec sa copie initiale (_snapshot_), passe l'entité à l'état `Modified` et génère le `UPDATE` ciblé lors du `SaveChanges()`.
- **Après `ExecuteUpdate`, les objets `Note` déjà chargés dans le contexte ont-ils leur nouveau coefficient ? Pourquoi ?**
  **Non.** `ExecuteUpdate` envoie directement une requête SQL `UPDATE` à la base de données en contournant complètement le _Change Tracker_ et le cache mémoire du `DbContext`. Les entités `Note` déjà chargées en mémoire conservent donc leur ancienne valeur de `Coef` tant qu'elles ne sont pas rechargées depuis la base.

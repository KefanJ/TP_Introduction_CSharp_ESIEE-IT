# Réponses — TP4 Entity Framework Core

## Étape 1 : Le modèle et la base

### Question 6 — Analyse de la première migration
* **Tables créées** : `Etudiants`, `Notes`, ainsi que `__EFMigrationsHistory` qui suit l'historique des migrations appliquées[cite: 40].
* **Clé étrangère et suppression** : `FK_Notes_Etudiants_EtudiantId` relie `Notes.EtudiantId` à `Etudiants.Id` avec `onDelete: ReferentialAction.Cascade`[cite: 40]. Supprimer un étudiant entraîne donc la suppression automatique de toutes ses notes[cite: 17, 40].

### Questions de réflexion (Étape 1)
* **Pourquoi `Nom` et `Matiere` sont-elles `NOT NULL` ?**
  En C#, ces propriétés sont déclarées avec le type non annulable `string` (et non `string?`)[cite: 16, 17]. Par convention, EF Core traduit tout type `string` non annulable en colonne `NOT NULL` en SQL (tandis que `required` oblige à les renseigner lors de l'instanciation en C#)[cite: 15, 16, 17].
* **Comment EF Core a-t-il deviné que `EtudiantId` est une clé étrangère ?**
  Grâce à ses conventions de nommage : en repérant la propriété de navigation `Etudiant` (de type `Etudiant`) dans `Note`, il associe automatiquement la propriété `EtudiantId` (`<NomNavigation>Id`) comme clé étrangère vers la clé primaire `Id` de `Etudiants`[cite: 17].

---

## Étape 2 : Insérer la promo

### Question 8 — Second lancement du programme
* Lors du deuxième `dotnet run`, aucune donnée n'est insérée[cite: 41]. EF Core exécute la requête `SELECT EXISTS (SELECT 1 FROM "Etudiants" AS "e")` pour évaluer `db.Etudiants.Any()` : comme `promo.db` contient déjà les 6 étudiants, la condition est vraie et le bloc d'insertion est ignoré[cite: 41].
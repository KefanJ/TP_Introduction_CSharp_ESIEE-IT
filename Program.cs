using System.Globalization;

static Note? Parser(string ligne)
{
    var c = ligne.Split(',');
    if (c.Length != 5) return null;
    if (!double.TryParse(c[3], CultureInfo.InvariantCulture, out var v)) return null;
    if (v < 0 || v > 20) return null;
    if (!int.TryParse(c[4], out var coef)) return null;
    return new Note(c[0], c[1], c[2], v, coef);
}

// 1.
//var lignes = File.ReadLines("notes.csv").Skip(1).Select(Parser).Where(n => n is not null).ToList();

var notes = File.ReadLines("notes.csv").Skip(1).Select(Parser).OfType<Note>().ToList();

var inscrits = File.ReadLines("etudiants.csv")
    .Skip(1)
    .Select(inscrit =>
    {
        var elements = inscrit.Split(',');
        return new Inscrit(elements[0], elements[1], elements[2]);
    })
    .ToList();

//foreach (var ligne in lignesEtudiants)
//{
//    Console.WriteLine(ligne);
//}

//foreach (var ligne in lignes)
//{
//    Console.WriteLine(ligne);
//}

// 1.2 Func,Action, Predicate

//1

static List<Note> Filtrer(List<Note> source, Func<Note, bool> critere)
{
    var res = new List<Note>();
    foreach (var n in source)
        if (critere(n)) res.Add(n);
    return res;
}



static bool EstEnAlgo(Note n) => n.Matiere == "Algo";
Filtrer(notes, EstEnAlgo);
Filtrer(notes, n => n.Valeur <10);
Filtrer(notes, n => n.Valeur>=15 && n.Prenom == "Ada") ;

// 3.
Func<Note, bool> c = n => n.Valeur >= 15;


//1.2 Func, Action, Predicate

//1.
Func<Note, double> pointsPonderes = n => n.Valeur * n.Coef;

//Action<Note> afficherNote = n => Console.WriteLine($"{n.Prenom} . {n.Matiere} . {n.Valeur}");

// 2.
static Func<Note, bool> AuDessusDe(double seuil)
{
    return n => n.Valeur >= seuil;
}

var noteAuDessusDe = Filtrer(notes, AuDessusDe(14));
//foreach (var n in noteAuDessusDe)
//{
//    Console.WriteLine(n);
//}

// 3.
notes.Sort((a, b) =>
{
    var matiere = a.Matiere.CompareTo(b.Matiere);

    if (matiere != 0)
    {
        return matiere;
    }

    return b.Valeur.CompareTo(a.Valeur);
});

notes.RemoveAll(n => n.Matiere == "Reseau");

// 1.3 La capture de variables 

//double seuil = 12;
//Func<Note, bool> admis = n => n.Valeur >= seuil;
//Console.WriteLine(Filtrer(notes, admis).Count);
//seuil = 15;
//Console.WriteLine(Filtrer(notes, admis).Count);


//2. Premières requêtes

////2.1 Filtrer, projeter, trier 

//// 1. Les notes d’Algo, triées par valeur décroissante. 

//var noteAlgo = notes
//                .Where(n => n.Matiere == "Algo")
//                .OrderByDescending(n => n.Valeur)
//                .ToList();

////2. La liste des matières, sans doublon et triée. 
//var matieresSansDoublon = notes
//                 .Select(n => n.Matiere)
//                 .Distinct()
//                 .Order()
//                 .ToList();

////3. Le prénom et la valeur de chaque note sous la note éliminatoire de 10

//var etudiantEnDessousDe10 = notes
//                            .Where(n => n.Valeur < 10)
//                            .Select(new { n => n.Prenom, n.Valeur })
//                            .ToList();
//// Version avec Tuple
//var etudiantEnDessousDe10Tuple = notes
//                            .Where(n => n.Valeur < 10)
//                            .Select(n => (n.Prenom, n.Valeur))
//                            .ToList();

////4. Le classement complet : par matière, puis par valeur décroissante, puis par prénom.
//var classementComplet = notes
//                        .orderBy(n => n.Matiere)
//                        .orderByDescending(n => n.Valeur)
//                        .orderBy(n => n.Prenom)
//                        .ToList();


////Correction avec ThenBy

//var classementCompletAvecThen = notes
//                        .orderBy(n => n.Matiere)
//                        .thenByDescending(n => n.Valeur)
//                        .thenBy(n => n.Prenom)
//                        .ToList();


//// 5. Les trois meilleures notes, toutes matières confondues 

//var troisMeilleurNotes = notes 
//                        .OrderByDescending(n => n.Valeur)
//                        .Take(3)
//                        .ToList();

//// les notes de la 4e à la 6e place (Skip et Take). 
//var quatreAsixemeNote = notes
//                       .OrderByDescending(n => n.Valeur)
//                       .Skip(3)
//                       .Take(3)
//                       .ToList();


//// 2.2 Agréger

//// 1. Le nombre de notes.
//var nbNotes = notes.Count();

//// le nombre de notes inférieures à 10,
//var nbNotesInferDix = notes.Count(n => n.Valeur < 10);

//// la moyenne simple de toutes les notes.
//var moyenneNotes = notes.Average(n => n.Valeur);

// // 2.La meilleure note avec Max, p
//double meilleurNote = notes.Max(n => n.Valeur);
//Console.WriteLine($"Meilleur note : {meilleurNote} ");

//// Note complète 
//Note noteComplete = notes.MaxBy(n => n.Valeur)!;
//Console.WriteLine($"Note complète : {noteComplete} ");


//// 3. La moyenne pondérée de toutes les notes : somme des valeurs × coefficients divisée par la somme des coefficients. 

//double ponderee = notes.Sum(n => n.Valeur * n.Coef) / notes.Sum(n => n.Coef);
//Console.WriteLine($"Moyenne pondere : {ponderee} ");

////4 . La chaîne « Ada, Alan, Grace, Linus, Barbara » (prénoms distincts, dans l’ordre du fichier) avec aggregate 
//string liste = notes.Select(n => n.Prenom).Distinct().Aggregate((acc, nom) => acc + ", " + nom);

//foreach (var l in liste)
//{
//    Console.Write(l);
//}

//// version striing.join
//string listeAvecJoin = string.Join(", ", notes.Select(n => n.Prenom).Distinct());


////  2.3 Chercher et tester 

//// 1. La note de Linus en BD avec FirstOrDefault. 
//var noteLinusBd = notes.FirstOrDefault(n => n.Prenom == "Linus" && n.Matiere == "BD");
//Console.WriteLine($"FirstOrDefault (Linus en BD) : {(noteLinusBd is null ? "null (aucune note)" : noteLinusBd)}");

//try
//{
//    var noteLinusBdFirst = notes.First(n => n.Prenom == "Linus" && n.Matiere == "BD");
//    Console.WriteLine($"First (Linus en BD) : {noteLinusBdFirst}");
//}
//catch (InvalidOperationException ex)
//{
//    Console.WriteLine($"First a levé une exception : {ex.Message}");
//}

//// 2. La note d’Ada en Algo avec Single.

//var noteAdaAlgo = notes.Single(n => n.Prenom == "Ada" && n.Matiere == "Algo");
//Console.WriteLine($"Single (Ada en Algo) : {noteAdaAlgo}");

//try
//{
//    var notesAda = notes.Single(n => n.Prenom == "Ada");
//    Console.WriteLine($"Single (toutes les notes d'Ada) : {notesAda}");
//}
//catch (InvalidOperationException ex)
//{
//    Console.WriteLine($"Single (Ada) a levé une exception : {ex.Message}");
//}

//// 3. Existe-t-il une note inférieure à 10 ?
//bool existeSous10 = notes.Any(n => n.Valeur < 10);
//Console.WriteLine($"Existe-t-il une note < 10 ? {existeSous10}");

//bool tousAuMoins12EnBd = notes
//    .Where(n => n.Matiere == "BD")
//    .All(n => n.Valeur >= 12);
//Console.WriteLine($"Tous au moins 12 en BD ? {tousAuMoins12EnBd}");

//// Test de All sur une séquence vide
//bool allSurVide = Enumerable.Empty<Note>().All(n => n.Valeur >= 12);
//Console.WriteLine($"All sur une séquence vide : {allSurVide}");


// 3. Regrouper et joindre 

//// 3.1 GroupBy 

//// 1. Regroupez les notes par matière 

//var groupesParMatiere = notes.GroupBy(n => n.Matiere);

//foreach (var groupe in groupesParMatiere)
//{
//    Console.WriteLine($"=== Matière : {groupe.Key} ===");
//    foreach (var note in groupe)
//    {
//        Console.WriteLine($"  - {note.Prenom} : {note.Valeur} (coef {note.Coef})");
//    }
//}

//// 2. Statistique

//var statsParMatiere = notes
//    .GroupBy(n => n.Matiere)
//    .Select(g => new
//    {
//        Matiere = g.Key,
//        NbNotes = g.Count(),
//        Moyenne = g.Average(n => n.Valeur),
//        Min = g.Min(n => n.Valeur),
//        Max = g.Max(n => n.Valeur),
//        Major = g.MaxBy(n => n.Valeur)!.Prenom
//    })
//    .OrderByDescending(s => s.Moyenne);

//foreach (var s in statsParMatiere)
//{
//    Console.WriteLine($"{s.Matiere} : {s.NbNotes} notes | Moy: {s.Moyenne:F2} | Min: {s.Min} | Max: {s.Max} | Major: {s.Major}");
//}


//// 3. Calculez la moyenne pondérée

//Dictionary<string, double> moyennesEtudiants = notes
//    .GroupBy(n => n.Prenom)
//    .ToDictionary(
//        g => g.Key,
//        g => g.Sum(n => n.Valeur * n.Coef) / g.Sum(n => n.Coef)
//    );

//foreach (var (prenom, moyenne) in moyennesEtudiants)
//{
//    Console.WriteLine($"{prenom} : moyenne pondérée = {moyenne:F2}");
//}



//// 4. Comptez les notes par étudiant avec CountBy

//// Version .NET 9 avec CountBy
//var nbNotesCountBy = notes.CountBy(n => n.Prenom);

//Console.WriteLine("--- Avec CountBy ---");
//foreach (var (prenom, nb) in nbNotesCountBy)
//{
//    Console.WriteLine($"{prenom} : {nb} notes");
//}

//// Version avec GroupBy et Count
//var nbNotesGroupBy = notes
//    .GroupBy(n => n.Prenom)
//    .Select(g => (Prenom: g.Key, Nb: g.Count()));

//Console.WriteLine("--- Avec GroupBy et Count ---");
//foreach (var item in nbNotesGroupBy)
//{
//    Console.WriteLine($"{item.Prenom} : {item.Nb} notes");
//}


//// 3.2 Join 

//// 1. Jointure avec clé composite 

//var releve = notes.Join(
//    inscrits,
//    n => (n.Nom, n.Prenom),          // Clé côté notes (tuple)
//    i => (i.Nom, i.Prenom),          // Clé côté inscrits (tuple)
//    (n, i) => new { i.Prenom, i.Promo, n.Matiere, n.Valeur }
//).ToList();

//Console.WriteLine("=== Relevé complet ===");
//foreach (var r in releve)
//{
//    Console.WriteLine($"{r.Prenom} ({r.Promo}) - {r.Matiere} : {r.Valeur}");
//}

//// 2. Moyenne simple par promo

//var moyennesParPromo = releve
//    .GroupBy(r => r.Promo)
//    .Select(g => new
//    {
//        Promo = g.Key,
//        Moyenne = g.Average(r => r.Valeur)
//    });

//Console.WriteLine("\n=== Moyenne par promo ===");
//foreach (var p in moyennesParPromo)
//{
//    Console.WriteLine($"{p.Promo} : {p.Moyenne:F2}");
//}

//// 3. Trouvez les inscrits sans aucune note

//// Méthode : Where + Any
//var sansNoteWhere = inscrits
//    .Where(i => !notes.Any(n => n.Nom == i.Nom && n.Prenom == i.Prenom));

//Console.WriteLine("\n=== Sans note (Where + Any) ===");
//foreach (var i in sansNoteWhere)
//{
//    Console.WriteLine($"{i.Prenom} {i.Nom} ({i.Promo})");
//}

//// Méthode  : GroupJoin 
//var sansNoteGroupJoin = inscrits
//    .GroupJoin(
//        notes,
//        i => (i.Nom, i.Prenom),
//        n => (n.Nom, n.Prenom),
//        (i, notesEtudiant) => new { Inscrit = i, Notes = notesEtudiant }
//    )
//    .Where(x => !x.Notes.Any())
//    .Select(x => x.Inscrit);

//Console.WriteLine("\n=== Sans note (GroupJoin) ===");
//foreach (var i in sansNoteGroupJoin)
//{
//    Console.WriteLine($"{i.Prenom} {i.Nom} ({i.Promo})");
//}

//// 4. Index avec ToLookup

//ILookup<string, Note> indexNotes = notes.ToLookup(n => n.Prenom);

//Console.WriteLine("\n=== Notes de Grace ===");
//foreach (var n in indexNotes["Grace"])
//{
//    Console.WriteLine($"{n.Matiere} : {n.Valeur} (coef {n.Coef})");
//}

//// Test avec un prénom absent
//var notesAbsent = indexNotes["Margaret"];
//Console.WriteLine($"\nNombre de notes pour Margaret : {notesAbsent.Count()}");


//// 3.3 SelectMany

//// 1. Construisez la liste des bulletins à partir des notes (GroupBy puis Select). 

//List<Bulletin> bulletins = notes
//    .GroupBy(n => n.Prenom)
//    .Select(g => new Bulletin(g.Key, g.Select(n => n.Valeur).ToList()))
//    .ToList();

//Console.WriteLine("=== Bulletins ===");
//foreach (var b in bulletins)
//{
//    Console.WriteLine($"{b.Prenom} : [{string.Join(", ", b.Valeurs)}]");
//}

////2. Retrouvez la liste « à plat » de toutes les valeurs avec SelectMany. 

//// Avec SelectMany : une seule liste plate de doubles
//IEnumerable<double> toutesLesValeurs = bulletins.SelectMany(b => b.Valeurs);

//Console.WriteLine("\n=== Toutes les valeurs à plat (SelectMany) ===");
//Console.WriteLine(string.Join(", ", toutesLesValeurs));

//// Comparaison avec Select (pour observer le type)
//IEnumerable<List<double>> listesDeValeurs = bulletins.Select(b => b.Valeurs);

//// 3. Avec la seconde forme de SelectMany, produisez des paires (prénom, valeur). 

//var paires = bulletins.SelectMany(
//    b => b.Valeurs,                                  // 1. Quelle sous-collection aplatir
//    (bulletin, valeur) => (bulletin.Prenom, valeur)  // 2. Comment combiner le parent et chaque élément
//);

//Console.WriteLine("\n=== Paires (Prénom, Valeur) ===");
//foreach (var (prenom, valeur) in paires)
//{
//    Console.WriteLine($"{prenom} -> {valeur}");
//}

//// 4. L’évaluation différée


//// Affiche toute les notes supérieur ou égal à 15
//var bonnes = notes.Where(n => n.Valeur >= 15);
//Console.WriteLine(bonnes.Count());


//// On ajoute une note à Hamilton une note de 19 en algo coef 3. On reaffiche le nombre de note supérieur ou égal à 15
//notes.Add(new Note("Hamilton", "Margaret", "Algo", 19, 3));
//Console.WriteLine(bonnes.Count());

///*
// * On crééer une liste qui stock les notes supérieur ou égal à 15
// * Ensuite on à ajoute à la liste des notes une nouvelle note qui est un 20 à Hamilton dans la matière BD coef 2
// *  A la fin on voit la comparaison  de  la liste bonne et la liste figee on voit que la nouvelle de note hamilton à été pris en compte dans la liste bonnes mais dans  figee

// */

//var figees = notes.Where(n => n.Valeur >= 15).ToList();
//notes.Add(new Note("Hamilton", "Margaret", "BD", 20, 2));
//Console.WriteLine(bonnes.Count() + " / " + figees.Count);

////2. Ajoutez un effet de bord 

//Console.WriteLine("\n=== Sans ToList (2 appels à Count) ===");
//var bonnesTrace = notes.Where(n =>
//{
//    Console.WriteLine($"  test de {n.Prenom} ({n.Matiere}) : {n.Valeur}");
//    return n.Valeur >= 15;
//});

//Console.WriteLine($"Count 1 : {bonnesTrace.Count()}");
//Console.WriteLine($"Count 2 : {bonnesTrace.Count()}");

//Console.WriteLine("\n=== Avec ToList d'abord ===");
//var bonnesFigees = notes.Where(n =>
//{
//    Console.WriteLine($"  test de {n.Prenom} ({n.Matiere}) : {n.Valeur}");
//    return n.Valeur >= 15;
//}).ToList();

//Console.WriteLine($"Count 1 : {bonnesFigees.Count}");
//Console.WriteLine($"Count 2 : {bonnesFigees.Count}");

//// 3. Remplacez Count() > 0 par Any() dans le même montage.

//Console.WriteLine("\n=== Test avec Count() > 0 ===");
//bool avecCount = bonnesTrace.Count() > 0;
//Console.WriteLine($"Résultat Count() > 0 : {avecCount}");

//Console.WriteLine("\n=== Test avec Any() ===");
//bool avecAny = bonnesTrace.Any();
//Console.WriteLine($"Résultat Any() : {avecAny}");

//// 4. Placez le Where avant puis après un OrderBy dans une même requête. 

//Console.WriteLine("\n=== Where AVANT OrderBy ===");
//int nbTriesAvant = 0;
//var resAvant = notes
//    .Where(n => n.Valeur >= 15)
//    .OrderBy(n =>
//    {
//        nbTriesAvant++;
//        return n.Valeur;
//    })
//    .ToList();
//Console.WriteLine($"Notes passées par le tri : {nbTriesAvant}");

//Console.WriteLine("\n=== Where APRÈS OrderBy ===");
//int nbTriesApres = 0;
//var resApres = notes
//    .OrderBy(n =>
//    {
//        nbTriesApres++;
//        return n.Valeur;
//    })
//    .Where(n => n.Valeur >= 15)
//    .ToList();
//Console.WriteLine($"Notes passées par le tri : {nbTriesApres}");

// 5. Réécrire la classe Promo 

var promo = new Promo("M1 Info", notes);

Console.WriteLine($"=== Promo : {promo.Nom} ===");
Console.WriteLine($"Étudiants : {string.Join(", ", promo.Etudiants())}");
Console.WriteLine($"Moyenne de Grace : {promo.Moyenne("Grace"):F2}");
Console.WriteLine($"Moyenne d'un absent : {promo.Moyenne("Inconnu"):F2}");
Console.WriteLine($"Moyenne générale : {promo.MoyenneGenerale():F2}");
Console.WriteLine($"Major : {promo.Major()}");

Console.WriteLine("\n--- Admis par matière ---");
foreach (var (matiere, nb) in promo.NbAdmisParMatiere())
{
    Console.WriteLine($"{matiere} : {nb} admis");
}

Console.WriteLine("\n--- Relevé ---");
foreach (var ligne in promo.Releve())
{
    Console.WriteLine(ligne);
}
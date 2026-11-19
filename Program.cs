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


//  2.3 Chercher et tester 

// 1. La note de Linus en BD avec FirstOrDefault. 
var noteLinusBd = notes.FirstOrDefault(n => n.Prenom == "Linus" && n.Matiere == "BD");
Console.WriteLine($"FirstOrDefault (Linus en BD) : {(noteLinusBd is null ? "null (aucune note)" : noteLinusBd)}");

try
{
    var noteLinusBdFirst = notes.First(n => n.Prenom == "Linus" && n.Matiere == "BD");
    Console.WriteLine($"First (Linus en BD) : {noteLinusBdFirst}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"First a levé une exception : {ex.Message}");
}

// 2. La note d’Ada en Algo avec Single.

var noteAdaAlgo = notes.Single(n => n.Prenom == "Ada" && n.Matiere == "Algo");
Console.WriteLine($"Single (Ada en Algo) : {noteAdaAlgo}");

try
{
    var notesAda = notes.Single(n => n.Prenom == "Ada");
    Console.WriteLine($"Single (toutes les notes d'Ada) : {notesAda}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Single (Ada) a levé une exception : {ex.Message}");
}

// 3. Existe-t-il une note inférieure à 10 ?
bool existeSous10 = notes.Any(n => n.Valeur < 10);
Console.WriteLine($"Existe-t-il une note < 10 ? {existeSous10}");

bool tousAuMoins12EnBd = notes
    .Where(n => n.Matiere == "BD")
    .All(n => n.Valeur >= 12);
Console.WriteLine($"Tous au moins 12 en BD ? {tousAuMoins12EnBd}");

// Test de All sur une séquence vide
bool allSurVide = Enumerable.Empty<Note>().All(n => n.Valeur >= 12);
Console.WriteLine($"All sur une séquence vide : {allSurVide}");
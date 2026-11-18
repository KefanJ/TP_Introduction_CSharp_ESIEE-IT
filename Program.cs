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

var lignesEtudiants = File.ReadLines("etudiants.csv")
    .Skip(1)
    .Select(ligne =>
    {
        var elements = ligne.Split(',');
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
List<Note> notes = new List<Note>();
Filtrer(notes, EstEnAlgo);
Filtrer(notes, n => n.Valeur <10);
Filtrer(notes, n => n.Valeur>=15 && n.Prenom == "Ada") ;

// 3.
Func<Note, bool> c = n => n.Valeur >= 15;


//1.2
Func<Note, double> pointsPonderes = n => n.Valeur * n.Coef;

Action<Note> afficherNote = n => Console.WriteLine($"{n.Prenom} . {n.Matiere} . {n.Valeur}");

static Func<Note, bool> AuDessusDe(double seuil)
{
    return n => n.Valeur >= seuil;
}

var noteAuDessusDe = Filtrer(notes, AuDessusDe(14));
foreach (var n in noteAuDessusDe)
{
    Console.WriteLine(n);
}

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
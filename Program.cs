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

var lignes = File.ReadLines("notes.csv").Skip(1).Select(Parser).OfType<Note>().ToList();

var lignesEtudiants = File.ReadLines("etudiants.csv")
    .Skip(1)
    .Select(ligne =>
    {
        var elements = ligne.Split(',');
        return new Inscrit(elements[0], elements[1], elements[2]);
    })
    .ToList();

foreach (var ligne in lignesEtudiants)
{
    Console.WriteLine(ligne);
}

foreach (var ligne in lignes)
{
    Console.WriteLine(ligne);
}


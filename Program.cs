//using Ecole;
using Geometrie;
// ==========================================
// --- PARTIE 2 : Types, chaînes et null ---
// ==========================================

// 2.1 & 2.2 : Types de base, références vs valeurs
//Console.WriteLine("Hello !");
//var c1 = new PointC { X = 1 };
//var c2 = c1;
//c2.X = 9;
//Console.WriteLine($"classe : c1.X = {c1.X}");

//var s1 = new PointS { X = 1 };
//var s2 = s1;
//s2.X = 9;
//Console.WriteLine($"struct : s1.X = {s1.X}");

//class PointC { public int X; }
//struct PointS { public int X; }

// 2.3 : Comparaison de chaînes et précision des flottants
//string a = "info";
//string b = string.Concat("in", "fo");
//Console.WriteLine(a == b);
//Console.WriteLine(0.1 + 0.2 == 0.3);
//Console.WriteLine(0.1m + 0.2m == 0.3m);

// 2.4 : Gestion de null et opérateur null-conditionnel
//string? saisie = Console.ReadLine();
//Console.WriteLine($"Vous avez tapé {saisie?.Length ?? 0} caractères");
//if (saisie is not null) {
//    Console.WriteLine($"Vous avez tapé {saisie.Length} caractères");
//}
//else
//{
//    Console.WriteLine($"Vous avez tapé 0 caractères");
//}

// 2.5 : Boucles et saisie sécurisée (TryParse)
/*
Console.Write("Entre votre age : ");
string texte = Console.ReadLine();
while(!int.TryParse(texte, out int age) || age <0 || age >120) {
    Console.Write("Entre votre age : ");
    texte = Console.ReadLine();
}
*/

// ====================================================
// --- PARTIE 3 : Contrôle et collections  ---
// ====================================================

// 3.1 : Listes, calculs (min/max) et switch expression
/*
string Mention(double moyenne)
{
    if (moyenne < 0 || moyenne > 20) throw new ArgumentOutOfRangeException("0 à 20");
    return moyenne switch
    {
        >= 16 => "Très bien",
        >= 14 => "Bien",
        >= 12 => "Assez bien",
        >= 10 => "Passable",
        _     => "Ajourné"
    };
}

List<double> notes = new List<double> { 12, 15.5, 9, 18 };
double moyenne;
double somme = 0;
double min = 20;
double max = 0;

foreach (var note in notes)
{
    somme += note;
    if (min > note) { min = note; }
    if (max < note) { max = note; }
}
moyenne = somme / notes.Count;
Console.WriteLine($"Moyenne : {moyenne} - {Mention(moyenne)}");
Console.WriteLine($"Note maximale : {max}");
Console.WriteLine($"Note minimale : {min}");
*/

// 3.2 : Dictionnaires et TryGetValue
/*
double moyenne;
var dico = new Dictionary<string, List<double>>
{
    ["Joe"] = [12.5, 9.2, 11.5],
    ["Amy"] = [18.5, 16.2, 14.5],
    ["Kenny"] = [14.5, 11.5, 13.6]
};
dico["Joe"].Add(15);

if(dico.TryGetValue("Inconnu",out var notesInconnu))
{
    Console.WriteLine($"Nombre de notes : {notesInconnu.Count}");
    moyenne = notesInconnu.Average();
    Console.WriteLine($"Moyenne : {moyenne} - {Mention(moyenne)}");
}
else
{
    Console.WriteLine($"Elève introuvable");
}

// Console.WriteLine($"Inconu: {dico["Inconnu"]}"); // Lèverait une exception

foreach (var (nom, notes) in dico)
{
    Console.WriteLine($"Nom étudiant : {nom}");
    Console.WriteLine($"Nombre de notes : {notes.Count}");
    moyenne = notes.Average();
    Console.WriteLine($"Moyenne : {moyenne} - {Mention(moyenne)}");
}
*/

// 3.3 : Filtrage par motif (Pattern matching avancé)
/*
static string Decrire(object? o) => o switch
{
    int n when n % 2 == 0 => "entier pair",
    string { Length: 0 } => "chaîne vide",
    int _ => "entier impair",
    string s => $"chaîne de {s.Length} caractères",
    List<double> l => $"liste de {l.Count} éléments",
    null => "null",
    _ => "autre"
};
*/

// ============================================
// --- PARTIE 4 : Classe Etudiant et propriétés ---
// ============================================

//using Ecole;

// 4.1 : Test de la classe Etudiant
//var ada = new Etudiant("Lovelace", "Ada", 4)
//{
//    Naissance = new DateOnly(1815, 12, 10)
//};
//ada.AjouterNote(15);
//ada.AjouterNote(16);
//Console.WriteLine(ada);          // appelle ToString() 
//ada.Annee = 5;                   // autorisé 
//ada.Nom = "Byron";             // erreur de compilation (lecture seule)
//ada._notes;                    // erreur de compilation (champ privé)
// ada.Naissance = ...;          // erreur de compilation (init-only)

// 4.2 : Test de la classe Promo
//var promoM1 = new Promo("M1 Info");
//promoM1.Inscrire(ada);
//var alan = new Etudiant("Turning", "Alan", 4)
//{
//    Naissance = new DateOnly(1817, 11, 9)
//};
//alan.AjouterNote(10);
//alan.AjouterNote(12);

//promoM1.Inscrire(alan);
//Console.WriteLine(alan);
//Console.WriteLine($"Moyenne de la promo {promoM1.Nom} : {promoM1.MoyenneGenerale():F2}");

// ============================================
// --- PARTIE 5 : Héritage et interfaces : les formes  ---
// ============================================

// 5.2 Polymorphisme 

//List<Forme> formes = [new Cercle(1), new Rectangle(2, 5), new Carre(3), new Cercle(2.5)];
//formes.Sort();
//foreach (var f in formes)
//    f.Dessiner();

// Calculez avec une boucle l’aire totale et la forme de plus grand périmètre.

//double aireTotale = 0;
//Forme? formeMaxPerimetre = null;
//double maxPerimetre = -1;

//foreach (var f in formes)
//{
//    aireTotale += f.Aire();

//    if (f.Perimetre() > maxPerimetre)
//    {
//        maxPerimetre = f.Perimetre();
//        formeMaxPerimetre = f;
//    }
//}

//Console.WriteLine($"Aire totale de toutes les formes : {aireTotale:F2}");
//Console.WriteLine($"La forme avec le plus grand périmètre est : {formeMaxPerimetre?.Nom} (Périmètre = {maxPerimetre:F2})");

// 5.3 Le piège du virtual 

Cercle c = new Cercle(1);
Forme f = c;
Console.WriteLine(c.Decrire());
Console.WriteLine(f.Decrire());
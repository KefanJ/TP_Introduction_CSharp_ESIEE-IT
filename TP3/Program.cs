using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using TP3;

//##### 0. Mise en place #####

// 0.2 Mettre à jour etudiants.csv 

/*
    1. Adaptez la lecture de etudiants.csv (LINQ, comme à la séance 2) au nouveau champ. Vérifiez que 
vous obtenez 6 inscrits.    
 */
Console.WriteLine("##### 0. Mise en place #####");
Console.WriteLine();
var etudiantsInscrits = File.ReadLines("etudiants.csv")
    .Skip(1)
    .Select(inscrit =>
    {
        var elements = inscrit.Split(',');
        return new Inscrit(elements[0], elements[1], elements[2], elements[3]);
    })
    .ToList();

foreach (var etudiant in etudiantsInscrits)
{
    Console.WriteLine(etudiant);
}

//##### 1. Exceptions  #####

Console.WriteLine();
Console.WriteLine("##### 1. Exceptions  #####");
Console.WriteLine();

//1.1 Une note toujours valide 

/*
    2. Modifiez le record Note pour qu’une valeur hors de l’intervalle 0–20 lève une 
ArgumentOutOfRangeException dès la création (voir la slide « Lever ses propres exceptions »). 
Vérifiez avec new Note("Lovelace", "Ada", "Algo", 25, 3). 
 */
static Note? Parser(string ligne)
{
    var c = ligne.Split(',');
    if (c.Length != 5) return null;
    if (!double.TryParse(c[3], CultureInfo.InvariantCulture, out var v)) return null;
    if (v < 0 || v > 20) return null;
    if (!int.TryParse(c[4], out var coef)) return null;
    return new Note(c[0], c[1], c[2], v, coef);
}

//var notes = File.ReadLines("notes.csv").Skip(1).Select(Parser).OfType<Note>().ToList();
/*
    2. Modifiez le record Note pour qu’une valeur hors de l’intervalle 0–20 lève une 
ArgumentOutOfRangeException dès la création (voir la slide « Lever ses propres exceptions »). 
Vérifiez avec new Note("Lovelace", "Ada", "Algo", 25, 3). 
 */

//var noteAuDessusDe20 = new Note("Lovelace", "Ada", "Algo", 25, 3);
//notes.Add(noteAuDessusDe20);

/*
    3. Ajoutez de même une vérification : le coefficient doit être strictement positif. Utilisez une méthode 
d’aide ArgumentOutOfRangeException.ThrowIfNegativeOrZero. 
 */

//var noteCoefNegatif = new Note("Lovelace", "Ada", "Algo", 19, -3);
//notes.Add(noteCoefNegatif);

//foreach (var note in notes)
//{
//    Console.WriteLine(note);
//}

// 1.2 Signaler les lignes invalides

/*
    4. Écrivez static Note ParserStrict(string ligne, int numero) qui lève une FormatException 
dont le message indique le numéro de ligne et la raison (nombre de champs, note non numérique…). 
Laissez remonter l’ArgumentOutOfRangeException du record pour une note hors limites. 
 */
static Note ParserStrict(string ligne, int numero)
{
    var c = ligne.Split(',', StringSplitOptions.TrimEntries);
    if (c.Length != 5) 
        throw new FormatException($"Ligne {numero} : 5 champs attendus ({c.Length} trouvées)");
    if (!double.TryParse(c[3], CultureInfo.InvariantCulture, out var v)) 
        throw  new FormatException($"Ligne {numero} : conversion ({c[3]} impossible)");
    if (!int.TryParse(c[4], out var coef))
        throw new FormatException($"Ligne {numero} : conversion ({c[4]} impossible)");
    return new Note(c[0], c[1], c[2], v, coef);
}

/*
    5. Écrivez une méthode qui lit notes.csv, appelle ParserStrict sur chaque ligne, garde les notes 
valides et affiche un message sur Console.Error pour chaque ligne rejetée. Ici, une boucle foreach 
avec un try/catch à l’intérieur est plus lisible que LINQ. 
 */

Console.WriteLine("// Test ReadNotes");
Console.WriteLine();
List<Note> ReadNotes()
{
    var notesValides = new List<Note>();
    var notes = File.ReadAllLines("notes.csv").Skip(1).ToList();
    int numero = 2;
    foreach (var note in notes)
    {
        try
        {
            var n = ParserStrict(note, numero);
            notesValides.Add(n);
        }
        catch (FormatException e)
        {
            Console.Error.WriteLine(e.Message);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.Error.WriteLine($"Ligne {numero} : {e.Message}");
        }
        numero++;
    }
    
   return notesValides;    
}

//var Nbnotes = ReadNotes().Count;
//Console.WriteLine($"Nombre de note  valides : {Nbnotes}");

//var notes = ReadNotes();

// #### 2. Sauvegarder la promo en JSON ####

/*
    8. Complétez Sauver et Charger. Sauvegardez les notes dans promo.json et ouvrez le fichier : comment 
s’écrivent les noms des propriétés ? Pourquoi ?
 */

//Stockage.Sauver("promo.json", ReadNotes());

/*
    9. Au démarrage, le programme charge promo.json s’il existe, et sinon importe notes.csv puis crée 
promo.json. Testez les deux cas en supprimant le fichier. 


10. Abîmez promo.json à la main (supprimez une accolade). Que se passe-t-il ? Interceptez l’exception, 
affichez un message clair, et repartez de notes.csv. 
11. Modifiez une note dans promo.json pour lui donner la valeur 30. Que se passe-t-il au chargement, et 
pourquoi ? Est-ce le comportement souhaité ?
 */
Console.WriteLine("#### 2. Sauvegarder la promo en JSON  ####");
Console.WriteLine();

//List<Note> notes;
//if (File.Exists("promo.json"))
//{
//    try
//    {
//        notes = Stockage.Charger("promo.json");
//        Console.WriteLine("Chargé depuis promo.json");
//    }
//    catch (JsonException e)
//    { 
//        Console.Error.WriteLine(e.Message);
//        notes = ReadNotes();
//        Stockage.Sauver("promo.json", notes);
//    }
//}
//else
//{
//    notes = ReadNotes();
//    Stockage.Sauver("promo.json",notes);
//    Console.WriteLine();    
//    Console.WriteLine("Importé depuis notes.csv");

//}

//### 3. Passer en asynchrone ###
Console.WriteLine("#### 3. Passer en asynchrone ####");
Console.WriteLine();
// 3.1 Charger et sauvegarder sans bloquer 

/*12. Écrivez ChargerAsync et SauverAsync avec File.ReadAllTextAsync (ou File.OpenRead et 
JsonSerializer.DeserializeAsync) et await using. Remplacez les appels dans Program.cs. */

List<Note> notes;
if (File.Exists("promo.json"))
{
    try
    {
        notes = await Stockage.ChargerAsync("promo.json");
        Console.WriteLine("Chargé depuis promo.json");
    }
    catch (JsonException e)
    {
        Console.Error.WriteLine(e.Message);
        notes = ReadNotes();
        await Stockage.SauverAsync("promo.json", notes);
    }
}
else
{
    notes = ReadNotes();
    await Stockage.SauverAsync("promo.json", notes);
    Console.WriteLine();
    Console.WriteLine("Importé depuis notes.csv");
}


// 3.2 Mesurer l’effet de Task.WhenAll 

static async Task<string> OperationLenteAsync(string nom)
{
    await Task.Delay(1000);
    return $"{nom} terminée";
}

/*14. Avec System.Diagnostics.Stopwatch, mesurez le temps de trois appels 
 * attendus l’un après l’autre.
 */

string[] noms = ["A", "B", "C"];
var sw = Stopwatch.StartNew();
foreach (var n in noms)
{
    await OperationLenteAsync(n);
}
sw.Stop();
Console.WriteLine($"Temps : {sw.ElapsedMilliseconds} ms");

/*15. Mesurez le temps des trois mêmes appels lancés d’abord,
 * puis attendus ensemble avec 
Task.WhenAll. */
sw.Restart();
var t1 = OperationLenteAsync("A");
var t2 = OperationLenteAsync("B");
var t3 = OperationLenteAsync("C");
await Task.WhenAll(t1, t2, t3);
 sw.Stop();

Console.WriteLine($"Temps avec WhenAll : {sw.ElapsedMilliseconds} ms");

/*
    16. Lancez cette fois les trois appels avec .Result au lieu de await. Le programme fonctionne-t-il ? 
Pourquoi est-ce tout de même déconseillé ? 
 */

sw.Restart();
var t4 = OperationLenteAsync("A").Result;
var t5 = OperationLenteAsync("B").Result;
var t6 = OperationLenteAsync("C").Result;
//await Task.WhenAll(t4, t5, t6);
sw.Stop();

Console.WriteLine($"Temps avec Result : {sw.ElapsedMilliseconds} ms");

//### 4. Interroger l’API geo.api.gouv.fr ###
Console.WriteLine();
Console.WriteLine("#### 4. Interroger l’API geo.api.gouv.fr ####");
Console.WriteLine();

// 4.1 Une première requête 

/*
    18. Créez un seul HttpClient pour tout le programme, avec BaseAddress = new 
Uri("https://geo.api.gouv.fr/"), puis affichez la ou les communes du code postal 35000 avec 
GetFromJsonAsync. 
 */

var http = new HttpClient { BaseAddress = new Uri("https://geo.api.gouv.fr/") };

var communes = await http.GetFromJsonAsync<List<Commune>>(
"communes?codePostal=35000");

foreach (var c in communes ?? [])
    Console.WriteLine($"{c.Nom} ({c.Code}) : {c.Population} habitants");

/*
    19. Écrivez Task<List<Commune>> CommunesAsync(string codePostal). Que renvoie l’API pour le code 
postal 99999 ? Votre méthode doit renvoyer une liste vide, jamais null. 
 */

async Task<List<Commune>> CommunesAsync(string codePostal, CancellationToken ct = default)
{
    var  communes = await http.GetFromJsonAsync<List<Commune>>(
$"communes?codePostal={codePostal}",ct);
    return communes ?? [];
}

var communesAsync = await CommunesAsync("99999");

Console.WriteLine();
Console.WriteLine("// 19. ");
Console.WriteLine();
Console.WriteLine(communesAsync.Count);

// 4.2 La commune de chaque étudiant, en parallèle 
/*
    20. Pour chaque inscrit, recherchez sa commune à partir de son code postal. Lancez toutes les requêtes 
en même temps avec Select et Task.WhenAll, puis affichez par exemple « Ada Lovelace (M1) : 
Rennes ». 

21. Un inscrit dont le code postal ne correspond à aucune commune (Margaret Hamilton) doit être 
affiché avec « commune inconnue », sans faire échouer les autres. 
 */
Console.WriteLine($"Nombre d'etudiant {etudiantsInscrits.Count}");
var taches = etudiantsInscrits.Select(async e => (Etudiant: e, Communes: await CommunesAsync(e.CodePostal)));
var resultats = await Task.WhenAll(taches);
foreach (var t in resultats)
{
    Console.WriteLine($"Etudiant : {t.Etudiant.Prenom} - {t.Etudiant.Nom} (Promo {t.Etudiant.Promo})  Ville : {t.Communes.FirstOrDefault()?.Nom ?? "commune inconnue"}");
}


/*
    22. Avec Stopwatch, comparez la durée de la version parallèle avec une version où les requêtes sont 
attendues l’une après l’autre. 
 */

Console.WriteLine();
Console.WriteLine("Comparaison des temps de traitement :");

Stopwatch sw2 = Stopwatch.StartNew();
var taches2 = etudiantsInscrits.Select(async e => (Etudiant: e, Communes: await CommunesAsync(e.CodePostal)));
var resultats2 = await Task.WhenAll(taches2);
sw2.Stop();
Console.WriteLine($"Temps avec WhenAll (version parallèle) : {sw2.ElapsedMilliseconds} ms");

Stopwatch sw3 = Stopwatch.StartNew();
foreach (var e in etudiantsInscrits)
{
    var communes2 = await CommunesAsync(e.CodePostal);
}
sw3.Stop();
Console.WriteLine($"Temps avec boucle (version séquentielle) : {sw3.ElapsedMilliseconds} ms");

// 4.3 Quand le réseau ne répond pas

/*
    23. Changez temporairement l’adresse de base en https://geo.api.gouv.invalid/. Quelle exception 
obtenez-vous ? Interceptez-la avec un message clair, puis rétablissez l’adresse. 
 */
try
{
    Stopwatch sw4 = Stopwatch.StartNew();
    var taches4 = etudiantsInscrits.Select(async e => (Etudiant: e, Communes: await CommunesAsync(e.CodePostal)));
    var resultats4 = await Task.WhenAll(taches4);
    sw4.Stop();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Erreur lors de la récupération des communes : {ex.Message}");
}

/*
 * 24. Ajoutez un délai maximal de 3 secondes à l’ensemble des requêtes avec un 
CancellationTokenSource. Quelle exception est levée si le délai est dépassé ? Pour tester, réduisez 
le délai à 1 milliseconde. 
 */

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
try
{
    Stopwatch sw5 = Stopwatch.StartNew();
    var taches5 = etudiantsInscrits.Select(async e => (Etudiant: e, Communes: await CommunesAsync(e.CodePostal, cts.Token)));
    var resultats5 = await Task.WhenAll(taches5);
    sw5.Stop();
}
catch (OperationCanceledException ex)
{
    Console.WriteLine($"\"Trop long : abandon au bout de 3 secondes : {ex.Message}");
}   
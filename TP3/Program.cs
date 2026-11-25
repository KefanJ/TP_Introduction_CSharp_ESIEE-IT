using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using TP3;

// ============================================================================
// TP3 C# : Exceptions, JSON, programmation asynchrone et appels HTTP
// ============================================================================

// ============================================================================
// ÉTAPE 0 : Mise en place et lecture des inscrits (Question 1)
// ============================================================================
Console.WriteLine("===== 0. Mise en place =====");

// Lecture de etudiants.csv avec LINQ en intégrant le nouveau champ CodePostal (4e colonne)
var etudiantsInscrits = File.ReadLines("etudiants.csv")
    .Skip(1) // Ignore la ligne d'en-tête
    .Select(ligne =>
    {
        var elements = ligne.Split(',', StringSplitOptions.TrimEntries);
        return new Inscrit(elements[0], elements[1], elements[2], elements[3]);
    })
    .ToList();

Console.WriteLine($"Nombre d'inscrits chargés : {etudiantsInscrits.Count}");
foreach (var etudiant in etudiantsInscrits)
{
    Console.WriteLine($" - {etudiant}");
}

// ============================================================================
// ÉTAPE 1 : Exceptions — valider et signaler les erreurs (Questions 2 à 6)
// ============================================================================
Console.WriteLine("\n===== 1. Exceptions =====");

// Note : l'ancienne méthode Parser (TP2) renvoyait null sans explication ;
// elle est remplacée ci-dessous par ParserStrict (Question 4).

// --- 1.1 Validation dans le record Note (Questions 2 et 3) ---
// Les validations (note entre 0 et 20, coefficient strictement positif via ThrowIfNegativeOrZero)
// sont implémentées directement dans le constructeur du record Note (Modele.cs).
// Tests effectués (commentés pour ne pas interrompre l'exécution du programme) :
// var noteInvalide = new Note("Lovelace", "Ada", "Algo", 25, 3);    // Lève ArgumentOutOfRangeException
// var coefInvalide = new Note("Lovelace", "Ada", "Algo", 19, -3);    // Lève ArgumentOutOfRangeException

// --- 1.2 Analyse stricte et lecture de notes.csv (Questions 4, 5 et 6) ---

// Question 4 : lève une FormatException détaillée (numéro de ligne + raison)
// et laisse remonter l'ArgumentOutOfRangeException du record Note si la valeur/coef est hors bornes.
static Note ParserStrict(string ligne, int numero)
{
    var c = ligne.Split(',', StringSplitOptions.TrimEntries);
    if (c.Length != 5)
        throw new FormatException($"Ligne {numero} : 5 champs attendus ({c.Length} trouvés)");
    if (!double.TryParse(c[3], CultureInfo.InvariantCulture, out var v))
        throw new FormatException($"Ligne {numero} : note '{c[3]}' non numérique");
    if (!int.TryParse(c[4], out var coef))
        throw new FormatException($"Ligne {numero} : coefficient '{c[4]}' non entier");

    return new Note(c[0], c[1], c[2], v, coef);
}

// Questions 5 et 6 : lit notes.csv, isole chaque ligne dans un try/catch ciblé
// et affiche les erreurs sur Console.Error (13 notes valides, 3 lignes rejetées).
static List<Note> ReadNotes()
{
    var notesValides = new List<Note>();
    var lignes = File.ReadAllLines("notes.csv").Skip(1);
    int numero = 2; // La ligne 1 correspond à l'en-tête du CSV

    foreach (var ligne in lignes)
    {
        try
        {
            var note = ParserStrict(ligne, numero);
            notesValides.Add(note);
        }
        catch (FormatException e)
        {
            Console.Error.WriteLine($"[Erreur format] {e.Message}");
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.Error.WriteLine($"[Erreur valeur] Ligne {numero} : {e.Message}");
        }
        numero++;
    }

    return notesValides;
}

// ============================================================================
// ÉTAPES 2 & 3.1 : Sauvegarde JSON et entrées-sorties asynchrones (Questions 7 à 13)
// ============================================================================
Console.WriteLine("\n===== 2 & 3.1. Chargement / Sauvegarde JSON asynchrone =====");

// Note (Étape 2 - Q8/Q9) : Les versions synchrones Stockage.Sauver / Stockage.Charger ont été
// remplacées par leurs équivalents asynchrones ChargerAsync / SauverAsync (Question 12).
// Les options JsonSerializerDefaults.Web écrivent les propriétés JSON en camelCase.

List<Note> notes;
const string cheminJson = "promo.json";

if (File.Exists(cheminJson))
{
    try
    {
        // Question 9 & 12 : chargement asynchrone depuis promo.json s'il existe
        notes = await Stockage.ChargerAsync(cheminJson);
        Console.WriteLine($"Promo chargée depuis {cheminJson} ({notes.Count} notes).");
    }
    catch (Exception e) when (e is JsonException or ArgumentOutOfRangeException)
    {
        // Question 10 (JsonException si JSON abîmé) & Question 11 (ArgumentOutOfRangeException si note = 30) :
        // on signale l'erreur puis on réimporte proprement depuis notes.csv.
        Console.Error.WriteLine($"Fichier {cheminJson} invalide ({e.Message}). Réimportation depuis notes.csv...");
        notes = ReadNotes();
        await Stockage.SauverAsync(cheminJson, notes);
    }
}
else
{
    // Question 9 : premier démarrage (ou après suppression de promo.json) -> import CSV + création JSON
    notes = ReadNotes();
    await Stockage.SauverAsync(cheminJson, notes);
    Console.WriteLine($"Promo importée depuis notes.csv ({notes.Count} notes) et sauvegardée dans {cheminJson}.");
}

// ============================================================================
// ÉTAPE 3.2 : Mesurer l'effet de Task.WhenAll (Questions 14, 15 et 16)
// ============================================================================
Console.WriteLine("\n===== 3.2. Benchmark asynchrone (Task.WhenAll) =====");

static async Task<string> OperationLenteAsync(string nom)
{
    await Task.Delay(1000); // Simule une attente réseau d'1 seconde sans bloquer le thread
    return $"{nom} terminée";
}

// Question 14 : exécution séquentielle (3 appels attendus l'un après l'autre -> ~3000 ms)
string[] noms = ["A", "B", "C"];
var sw = Stopwatch.StartNew();
foreach (var n in noms)
{
    await OperationLenteAsync(n);
}
sw.Stop();
Console.WriteLine($"Temps séquentiel (await en boucle) : {sw.ElapsedMilliseconds} ms");

// Question 15 : exécution parallèle (tâches lancées ensemble puis attendues via Task.WhenAll -> ~1000 ms)
sw.Restart();
var t1 = OperationLenteAsync("A");
var t2 = OperationLenteAsync("B");
var t3 = OperationLenteAsync("C");
await Task.WhenAll(t1, t2, t3);
sw.Stop();
Console.WriteLine($"Temps parallèle (Task.WhenAll)     : {sw.ElapsedMilliseconds} ms");

// Question 16 : test avec .Result (commenté pour respecter la consigne "aucun .Result ni .Wait()")
// Résultat observé : ~3000 ms (bloque le thread appelant et risque un deadlock en environnement avec SynchronizationContext).
/*
sw.Restart();
var r1 = OperationLenteAsync("A").Result;
var r2 = OperationLenteAsync("B").Result;
var r3 = OperationLenteAsync("C").Result;
sw.Stop();
Console.WriteLine($"Temps bloquant (.Result)           : {sw.ElapsedMilliseconds} ms");
*/

// ============================================================================
// ÉTAPE 4 : Interroger l'API geo.api.gouv.fr (Questions 17 à 24)
// ============================================================================
Console.WriteLine("\n===== 4. Interroger l'API geo.api.gouv.fr =====");

// --- 4.1 Une première requête (Questions 18 et 19) ---

// Question 18 : instance unique de HttpClient partagée dans tout le programme (évite l'épuisement des sockets)
using var http = new HttpClient { BaseAddress = new Uri("https://geo.api.gouv.fr/") };

var communesRennes = await http.GetFromJsonAsync<List<Commune>>("communes?codePostal=35000");
Console.WriteLine("Test code postal 35000 :");
foreach (var c in communesRennes ?? [])
{
    Console.WriteLine($" - {c.Nom} (INSEE {c.Code}) : {c.Population} habitants");
}

// Question 19 : retourne toujours une liste (vide [] pour un code inconnu comme 99999, jamais null)
async Task<List<Commune>> CommunesAsync(string codePostal, CancellationToken ct = default)
{
    var resultat = await http.GetFromJsonAsync<List<Commune>>($"communes?codePostal={codePostal}", ct);
    return resultat ?? [];
}

var communesInconnues = await CommunesAsync("99999");
Console.WriteLine($"Test code postal 99999 : {communesInconnues.Count} commune trouvée");

// --- 4.2 La commune de chaque étudiant, en parallèle (Questions 20, 21 et 22) ---
Console.WriteLine("\nCommunes des étudiants :");

// Question 20 & 21 : lancement simultané avec Select + Task.WhenAll.
// Si le code postal ne renvoie aucune commune (ex. Margaret Hamilton - 99999), on affiche "commune inconnue".
var tachesCommunes = etudiantsInscrits.Select(async e =>
    (Etudiant: e, Communes: await CommunesAsync(e.CodePostal)));

var resultatsCommunes = await Task.WhenAll(tachesCommunes);

foreach (var item in resultatsCommunes)
{
    string ville = item.Communes.FirstOrDefault()?.Nom ?? "commune inconnue";
    Console.WriteLine($" - {item.Etudiant.Prenom} {item.Etudiant.Nom} ({item.Etudiant.Promo}) : {ville}");
}

// Question 22 : comparaison des performances entre requêtes parallèles et séquentielles
Console.WriteLine("\nComparaison des temps de requête HTTP :");

var swParallele = Stopwatch.StartNew();
await Task.WhenAll(etudiantsInscrits.Select(e => CommunesAsync(e.CodePostal)));
swParallele.Stop();
Console.WriteLine($"Temps parallèle (Task.WhenAll) : {swParallele.ElapsedMilliseconds} ms");

var swSequentiel = Stopwatch.StartNew();
foreach (var e in etudiantsInscrits)
{
    await CommunesAsync(e.CodePostal);
}
swSequentiel.Stop();
Console.WriteLine($"Temps séquentiel (boucle)      : {swSequentiel.ElapsedMilliseconds} ms");

// --- 4.3 Gestion des pannes réseau et des délais (Questions 23 et 24) ---
Console.WriteLine("\nTests de résilience réseau :");

// Question 23 : interception d'une erreur réseau / DNS (HttpRequestException)
// (Testé en remplaçant temporairement BaseAddress par https://geo.api.gouv.invalid/)
try
{
    await Task.WhenAll(etudiantsInscrits.Select(e => CommunesAsync(e.CodePostal)));
    Console.WriteLine("Requêtes réseau réussies.");
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"Erreur réseau lors de l'appel à l'API : {ex.Message}");
}

// Question 24 : délai maximal global de 3 secondes avec CancellationTokenSource
// (Testé avec TimeSpan.FromMilliseconds(1) -> lève TaskCanceledException / OperationCanceledException)
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
try
{
    await Task.WhenAll(etudiantsInscrits.Select(e => CommunesAsync(e.CodePostal, cts.Token)));
    Console.WriteLine("Requêtes terminées dans le délai imparti (3 s).");
}
catch (OperationCanceledException ex)
{
    Console.Error.WriteLine($"Trop long : abandon après dépassement du délai ({ex.Message})");
}
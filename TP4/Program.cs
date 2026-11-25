using Microsoft.EntityFrameworkCore;
using System.Net;
using TpEf;

using (var db = new PromoContext())
{
    if (!db.Etudiants.Any())
    {
        var promo = Donnees.Promo();
        db.Etudiants.AddRange(promo);
        Console.WriteLine($"Avant : {db.Entry(promo[0]).State}");
        int n = db.SaveChanges();
        Console.WriteLine($"{n} lignes écrites");
        Console.WriteLine($"Après : {db.Entry(promo[0]).State}, Id = {promo[0].Id}");
    }
}


using (var db = new PromoContext())
{
    // 9. Les étudiants de M2, triés par nom. Affichez aussi le SQL de la requête avec ToQueryString(). 
    Console.WriteLine(db.Etudiants.Where(e => e.Promo == "M2").OrderBy(e => e.Nom).ToQueryString());
    var etudiantsM2 = db.Etudiants.Where(e => e.Promo == "M2").OrderBy(e => e.Nom).ToList();


    //10. Pour chaque étudiant, son nom et son nombre de notes, avec une projection Select vers un type anonyme(sans Include).
    var nbNotesEtudiant = db.Etudiants
        .Select(e => new {Etudiant = e.Nom, Note = e.Notes.Count})
        .ToList();

    // 11. La moyenne de chaque matière (GroupBy, puis Average). Lisez le SQL généré : où est fait le calcul ? 
    var moyenneMatiere = db.Notes
        .GroupBy(n => n.Matiere)
        .Select(g => new { Matiere = g.Key, Moyenne = g.Average(n => n.Valeur) })
        .ToList();

    foreach (var item in moyenneMatiere)
    {
        Console.WriteLine($"Matière : {item.Matiere}, Moyenne : {item.Moyenne:F2}");
    }

    Console.WriteLine();

    //12. Parcourez db.Etudiants.ToList() et affichez e.Notes.Count pour chaque étudiant. Recommencez avec.Include(e => e.Notes).Comparez les deux résultats, et le SQL des deux requêtes.

    Console.WriteLine("Sans Include :");
    var etudiants = db.Etudiants.ToList();
    foreach (var e in etudiants)
    {
        Console.WriteLine($"Etudiant : {e.Nom}, Nombre de notes : {e.Notes.Count}");
    }


    Console.WriteLine();
    Console.WriteLine("Avec Include :");
    var etudiantsAvecNotes = db.Etudiants.Include(e => e.Notes).ToList();
    foreach (var e in etudiantsAvecNotes)
    {
        Console.WriteLine($"Etudiant : {e.Nom}, Nombre de notes : {e.Notes.Count}");
    }

    Console.WriteLine();

    // 13. La moyenne pondérée de chaque étudiant qui a au moins une note (somme des valeurs × coefficients, divisée par la somme des coefficients), de la meilleure à la moins bonne, affichée avecdeux décimales.
    var moyennePonderee = db.Etudiants
        .Where(e => e.Notes.Any())
        .Select(e => new { Etudiant = e.Nom, Moyenne = e.Notes.Sum(n => n.Valeur * n.Coef) / e.Notes.Sum(n => n.Coef) })
        .OrderByDescending(m => m.Moyenne)
        .ToList();
    foreach (var m in moyennePonderee)
    {
        Console.WriteLine($"Etudiant : {m.Etudiant}, Moyenne pondérée : {m.Moyenne:F2}");
    }

}

/*
 
4. Modifier, supprimer, faire évoluer (10 min) 
Ces modifications sont enregistrées dans promo.db : relancer le programme les rejoue sur une base déjà 
modifiée. Utilisez FirstOrDefault et testez null avant de modifier ou de supprimer. Pour retrouver les 
résultats attendus, repartez d’une base neuve : supprimez promo.db, puis dotnet ef database update ; la 
promo est réinsérée au lancement suivant.  
 
 */



using (var db = new PromoContext())
{
    // 14. Alan Turing passe en M2
    var alan = db.Etudiants.FirstOrDefault(e => e.Nom == "Turing");
    if (alan != null)
    {
        alan.Promo = "M2";
        Console.WriteLine($"État d'Alan avant SaveChanges : {db.Entry(alan).State}");
        db.SaveChanges();
    }

    // 15. Supprimer Linus Torvalds
    var linus = db.Etudiants.FirstOrDefault(e => e.Nom == "Torvalds");
    if (linus != null)
    {
        db.Etudiants.Remove(linus);
        db.SaveChanges();
    }
    Console.WriteLine($"Notes restantes dans la base : {db.Notes.Count()}");

    // 16. Coefficient des notes de Réseau à 2 sans charger les entités
    int nb = db.Notes
        .Where(n => n.Matiere == "Reseau")
        .ExecuteUpdate(s => s.SetProperty(n => n.Coef, 2));
    Console.WriteLine($"Notes de Réseau modifiées : {nb}");
}
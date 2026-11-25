public class Promo(string nom, IEnumerable<Note> notes)
{
    private readonly List<Note> _notes = notes.ToList();
    public string Nom { get; } = nom;

    // Prénoms distincts
    public IEnumerable<string> Etudiants() =>
        _notes.Select(n => n.Prenom).Distinct();

    // Moyenne pondérée, 0 si aucune note
    public double Moyenne(string prenom) =>
        _notes.Where(n => n.Prenom == prenom) is var notesEtudiant && notesEtudiant.Any()
            ? notesEtudiant.Sum(n => n.Valeur * n.Coef) / notesEtudiant.Sum(n => n.Coef)
            : 0;

    // Moyenne des moyennes des étudiants (0 si promo vide)
    public double MoyenneGenerale() =>
        Etudiants().Any()
            ? Etudiants().Average(Moyenne)
            : 0;

    // Prénom du major, null si promo vide
    public string? Major() =>
        Classement().FirstOrDefault().Prenom;

    // (prénom, moyenne) par moyenne décroissante
    public IEnumerable<(string Prenom, double Moyenne)> Classement() =>
        Etudiants()
            .Select(p => (Prenom: p, Moyenne: Moyenne(p)))
            .OrderByDescending(x => x.Moyenne);

    // Matière -> nombre de notes >= 10
    public IReadOnlyDictionary<string, int> NbAdmisParMatiere() =>
        _notes
            .GroupBy(n => n.Matiere)
            .ToDictionary(g => g.Key, g => g.Count(n => n.Valeur >= 10));

    // Relevé formaté dans l'ordre du classement
    public IEnumerable<string> Releve() =>
        Classement()
            .Select((e, index) => $"{index + 1}. {e.Prenom} {e.Moyenne:F2} {Mention(e.Moyenne)}");


    private static string Mention(double moyenne) => moyenne switch
    {
        >= 16 => "Très bien",
        >= 14 => "Bien",
        >= 12 => "Assez bien",
        >= 10 => "Passable",
        _ => "Insuffisant"
    };
}
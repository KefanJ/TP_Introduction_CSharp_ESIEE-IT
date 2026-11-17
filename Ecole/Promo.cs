namespace Ecole;

class Promo
{
    public string Nom { get; }
    private readonly List<Etudiant> _etudiants = new();


    public Promo(string nom)
    {
        Nom = nom;

    }


    public void Inscrire(Etudiant e)
    {
        _etudiants.Add(e);
    }

    public double MoyenneGenerale() => _etudiants.Count == 0 ? 0 : _etudiants.Average(e => e.Moyenne());




public Etudiant? Chercher(string nom)
    {
        foreach (var e in _etudiants)
            if (e.Nom == nom)
            {
                return e;
            }

        return null;
    }
}
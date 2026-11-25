namespace Ecole;

public class Etudiant
{
    public string Nom{ get; }
    public string Prenom { get; }
    public DateOnly Naissance { get; init; }

    private int _annee;
    public int Annee {
        get => _annee;

        set
        {
            if (value < 1 || value > 5) throw new ArgumentOutOfRangeException("1 à 5");
            _annee = value;
        }    
    }
    public string NomComplet => $"{Prenom} {Nom.ToUpper()}";
    private readonly List<double> _notes = new();

    public IReadOnlyList<double> Notes => _notes;
    
     
    

    public int NbNotes => _notes.Count;

    public Etudiant(string nom, string prenom, int annee)
    {
        Nom = nom;
        Prenom = prenom;
        Annee = annee;
    }



   public void AjouterNote(double n)
    {
        if (n < 0 || n > 20) throw new ArgumentOutOfRangeException("0 à 20");
        _notes.Add(n);
    } 

   public double Moyenne() => _notes.Count == 0 ? 0 : _notes.Average();

    public override string ToString()
    {
        string niveauAnne = _annee switch
        {
            1 => "L1",
            2 => "L2",
            3 => "L3",
            4 => "M1",
            5 => "M2",
            _ => "Diplomé"
        };

        return $"{NomComplet} ({niveauAnne}) : {Moyenne():F2} ";
    }
}

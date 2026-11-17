namespace Geometrie;

// 5.1 La hiérarchie 

// 1. 
public interface IDessinable
{
    void Dessiner();
}

// 2. 
public abstract class Forme : IDessinable, IComparable<Forme>
{
    public string Nom { get; }

    protected Forme(string nom)
    {
        Nom = nom;
    }

    public abstract double Aire();
    public abstract double Perimetre();

    //public virtual string Decrire()
    public string Decrire()
    {
        return $"{Nom} - Aire : {Aire():F2}, Périmètre : {Perimetre():F2}";
    }

    public void Dessiner()
    {
        Console.WriteLine(Decrire());
    }

   
    public int CompareTo(Forme? autre)
    {
        if (autre is null) return 1;
        return this.Aire().CompareTo(autre.Aire());
    }
}

// 3. 
public class Cercle : Forme
{
    public double Rayon { get; }

    public Cercle(double rayon) : base("Cercle")
    {
        if (rayon <= 0) throw new ArgumentOutOfRangeException("Le rayon doit être strictement positif.");
        Rayon = rayon;
    }

    public override double Aire() => Math.PI * Rayon * Rayon;
    public override double Perimetre() => 2 * Math.PI * Rayon;

    // 5.
   // public override string Decrire()
    public new string Decrire()
    {
        // On réutilise la description de la classe parente et on y ajoute le rayon
        return $"{base.Decrire()} - Rayon : {Rayon}";
    }
}


public class Rectangle : Forme
{
    public double Largeur { get; }
    public double Hauteur { get; }

    public Rectangle(double largeur, double hauteur) : base("Rectangle")
    {
        if (largeur <= 0 || hauteur <= 0) throw new ArgumentOutOfRangeException("Dimensions strictement positives requises.");
        Largeur = largeur;
        Hauteur = hauteur;
    }

    public override double Aire() => Largeur * Hauteur;
    public override double Perimetre() => 2 * (Largeur + Hauteur);
}

// 4. 
public sealed class Carre : Rectangle
{
    public Carre(double cote) : base(cote, cote)
    {
    }
}



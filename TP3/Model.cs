using System.Reflection.Metadata.Ecma335;


//##### 0. Mise en place #####


//  0.1 Ajouter le code postal 
public record Inscrit(string Nom, string Prenom, string Promo, string CodePostal);


//##### 1. Exceptions #####

//1.1 Une note toujours valide 

/*
    2. Modifiez le record Note pour qu’une valeur hors de l’intervalle 0–20 lève une 
ArgumentOutOfRangeException dès la création (voir la slide « Lever ses propres exceptions »). 
Vérifiez avec new Note("Lovelace", "Ada", "Algo", 25, 3). 
 */

public record Note(string Nom, string Prenom, string Matiere, double Valeur, int Coef)
{
    private static int ValiderCoef(int coef) {

        if(coef <= 0) {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(coef);
        }
        return coef; 
    }
    public double Valeur { get; } = Valeur is >= 0 and <= 20
        ? Valeur
        : throw new ArgumentOutOfRangeException(nameof(Valeur));

    public int Coef { get; } = ValiderCoef(Coef);


}



public record Bulletin(string Prenom, List<double> Valeurs);

/*
 4.1 Une première requête 
17. Déclarez un record Commune avec seulement les champs utiles : Nom, Code (code INSEE) et Population. 
Quel type choisir pour Population, et pourquoi ? */
public record Commune(string Nom, string Code, int? Population);
using ChocolaterieDeWilly.Enumerations;

namespace ChocolaterieDeWilly.Models
{
    /// <summary>
    /// NOTE : Idéalement, Masse serait un readonly record struct. Une masse se
    /// définit uniquement par sa valeur et son unité : elle n'a pas d'identité propre,
    /// ne change jamais après sa création et deux masses identiques devraient être
    /// considérées comme égales.
    /// 
    /// Une classe a toutefois été choisie, car la syntaxe d'un record struct
    /// contenant des méthodes n'a pas été vue en classe. Pour garder le même
    /// comportement, les propriétés sont en lecture seule (get sans set) et chaque
    /// opération retourne une nouvelle Masse au lieu de modifier celle-ci.
    /// </summary>
    public class Masse
    {
        /// <summary>
        /// Nombre de grammes dans un kilogramme.
        /// </summary>
        public const double GrammesParKilogramme = 1000;

        /// <summary>
        /// Nombre de grammes dans une livre.
        /// </summary>
        public const double GrammesParLivre = 453.59237;

        /// <summary>
        /// Valeur numérique de la masse, exprimée dans son unité.
        /// </summary>
        public double Valeur { get; }

        /// <summary>
        /// Unité dans laquelle la valeur est exprimée.
        /// </summary>
        public UniteMasse Unite { get; }

        public Masse(double valeur, UniteMasse unite)
        {
            Valeur = valeur;
            Unite = unite;
        }

        /// <summary>
        /// Retourne la valeur de cette masse exprimée en grammes.
        /// Utile pour comparer deux masses qui ne sont pas dans la même unité.
        /// </summary>
        /// <returns>La valeur en grammes.</returns>
        /// <exception cref="ArgumentException">L'unité n'est pas prise en charge.</exception>
        public double EnGrammes()
        {
            switch (Unite)
            {
                case UniteMasse.Grammes:
                    return Valeur;
                case UniteMasse.Kilogrammes:
                    return Valeur * GrammesParKilogramme;
                case UniteMasse.Livres:
                    return Valeur * GrammesParLivre;
                default:
                    throw new ArgumentException($"Unité inconnue : {Unite}");
            }
        }

        /// <summary>
        /// Convertit cette masse dans une autre unité. La conversion passe par les grammes,
        /// ce qui permet de convertir n'importe quelle unité vers n'importe quelle autre.
        /// La masse d'origine n'est pas modifiée.
        /// </summary>
        /// <param name="unite">L'unité souhaitée.</param>
        /// <returns>Une nouvelle masse, exprimée dans l'unité souhaitée.</returns>
        /// <exception cref="ArgumentException">L'unité n'est pas prise en charge.</exception>
        public Masse ConvertirEn(UniteMasse unite)
        {
            double grammes = EnGrammes();

            switch (unite)
            {
                case UniteMasse.Grammes:
                    return new Masse(grammes, unite);
                case UniteMasse.Kilogrammes:
                    return new Masse(grammes / GrammesParKilogramme, unite);
                case UniteMasse.Livres:
                    return new Masse(grammes / GrammesParLivre, unite);
                default:
                    throw new ArgumentException($"Unité inconnue : {unite}");
            }
        }

        /// <summary>
        /// Retourne la masse sous forme de texte, avec le symbole de son unité (g, kg ou lb).
        /// </summary>
        /// <returns>La masse formatée, par exemple "50 kg".</returns>
        public override string ToString()
        {
            switch (Unite)
            {
                case UniteMasse.Kilogrammes:
                    return $"{Valeur:0.##} kg";
                case UniteMasse.Livres:
                    return $"{Valeur:0.##} lb";
                default:
                    return $"{Valeur:0.##} g";
            }
        }
    }
}
namespace ChocolaterieDeWilly.Models
{
    /// <summary>
    /// Représente une création de chocolat à fabriquer dans un lot
    /// (par exemple, des feuilles d'érable ou des citrouilles moulées).
    /// </summary>
    public class Creation
    {
        /// <summary>
        /// Nom de la création.
        /// </summary>
        public string Nom { get; }

        /// <summary>
        /// Nombre d'unités à fabriquer.
        /// </summary>
        public int Quantite { get; set; }

        /// <summary>
        /// Poids d'une seule unité de la création.
        /// </summary>
        public Masse PoidsUnitaire { get; }

        public Creation(string nom, int quantite, Masse poidsUnitaire)
        {
            Nom = nom;
            Quantite = quantite;
            PoidsUnitaire = poidsUnitaire;
        }

        /// <summary>
        /// Calcule la quantité totale de chocolat nécessaire pour fabriquer toutes les unités.
        /// </summary>
        /// <returns>Le chocolat requis, exprimé dans la même unité que le poids unitaire.</returns>
        public Masse CalculerChocolatRequis()
        {
            return new Masse(PoidsUnitaire.Valeur * Quantite, PoidsUnitaire.Unite);
        }

    }
}

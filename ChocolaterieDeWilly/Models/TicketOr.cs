namespace ChocolaterieDeWilly.Models
{
    /// <summary>
    /// Représente un ticket d'or caché dans un chocolat. Le client qui le trouve
    /// gagne une visite de la chocolaterie.
    /// </summary>
    public class TicketOr
    {
        /// <summary>
        /// Numéro du ticket, de 1 à MaxTicketsOr, dans l'ordre où les tickets sont cachés.
        /// </summary>
        public int Numero { get; }

        /// <summary>
        /// Numéro du lot dans lequel le ticket a été caché.
        /// </summary>
        public int NumeroLot { get; }

        /// <summary>
        /// Nom de la personne qui a trouvé le ticket. Vide tant que le ticket n'a pas été trouvé.
        /// </summary>
        public string NomGagnant { get; set; } = "";

        public TicketOr(int numero, int numeroLot)
        {
            Numero = numero;
            NumeroLot = numeroLot;
        }
    }
}

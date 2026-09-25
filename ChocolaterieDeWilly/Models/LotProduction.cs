using ChocolaterieDeWilly.Enumerations;

namespace ChocolaterieDeWilly.Models
{
    /// <summary>
    /// Représente un lot de production : une quantité d'une même création 
    /// à fabriquer avant une date limite.
    /// </summary>
    public class LotProduction
    {
        /// <summary>
        /// Numéro unique du lot, attribué par l'atelier.
        /// </summary>
        public int Numero {  get; }

        /// <summary>
        /// Date avant laquelle le lot doit être fabriqué.
        /// </summary>
        public DateTime DateLimite { get; }

        /// <summary>
        /// État du lot : planifié ou terminé (fabriqué)
        /// </summary>
        public StatutLot Statut { get; set; }

        /// <summary>
        /// Création fabriquée dans ce lot (nom, quantité et poids unitaire).
        /// </summary>
        public Creation Creation { get; }

        /// <summary>
        /// Tickets d'or cachés dans ce lot.
        /// </summary>
        public List<TicketOr> Tickets { get; } = new List<TicketOr>();

        /// <summary>
        /// Nombre d'unités du lot qui n'ont pas été vendues.
        /// Doit être entre 0 et la quantité produite.
        /// </summary>
        private int _quantiteInvendue;
        public int QuantiteInvendue
        {
            get { return _quantiteInvendue; }
            set  { _quantiteInvendue = value; }
        }


        public LotProduction(int numero, string nom, int quantite,
                     Masse poidsUnitaire, DateTime dateLimite)
        {
            Numero = numero;
            DateLimite = dateLimite;
            Statut = StatutLot.Planifie;
            Creation = new Creation(nom, quantite, poidsUnitaire);
        }

        /// <summary>
        /// Cache un nouveau ticket d'or dans ce lot.
        /// </summary>
        /// <param name="numeroTicket">Le numéro du ticket à cacher.</param>
        public void AjouterTicket(int numeroTicket)
        {
            Tickets.Add(new TicketOr(numeroTicket, Numero));
        }

        /// <summary>
        /// Enregistre le nom du gagnant d'un ticket caché dans ce lot.
        /// </summary>
        /// <param name="numeroTicket">Le numéro du ticket gagnant.</param>
        /// <param name="nomGagnant">Le nom de la personne qui a trouvé le ticket.</param>
        /// <returns>true si le ticket se trouve dans ce lot, sinon false.</returns>
        public bool EnregistrerGagnant(int numeroTicket, string nomGagnant)
        {
            for (int i = 0; i < Tickets.Count; i++)
            {
                if (Tickets[i].Numero == numeroTicket)
                {
                    Tickets[i].NomGagnant = nomGagnant;
                    return true;
                }
            }

            return false;
        }

    }
}

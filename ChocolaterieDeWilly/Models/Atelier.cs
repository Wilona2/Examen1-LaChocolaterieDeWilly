using ChocolaterieDeWilly.Enumerations;
using ChocolaterieDeWilly.ExceptionsPersonnalisees;

namespace ChocolaterieDeWilly.Models
{

    /// <summary>
    /// Représente l'atelier de chocolaterie : sa réserve de chocolat, ses lots de production et 
    /// les tickets d'or cachés dans les chocolats.
    /// </summary>
    public class Atelier
    {
        /// <summary>
        /// Numéro qui sera attribué au prochain lot planifié. Commence à 1.
        /// </summary>
        private int _prochainNumeroLot = 1;

        /// <summary>
        /// Nombre total d'unités fabriquées depuis le début de la saison, tous lots confondus.
        /// Sert de repère pour savoir quand cacher un ticket d'or.
        /// </summary>
        private int _compteurUnites;

        /// <summary>
        /// Nombre maximum de tickets d'or pouvant être cachés pendant la saison.
        /// </summary>
        public const int MaxTicketsOr = 5;

        /// <summary>
        /// Nombre d'unités fabriquées entre deux tickets d'or.
        /// </summary>
        public const int IntervalleTicket = 500;

        /// <summary>
        /// Quantité de chocolat disponible dans l'atelier
        /// </summary>
        public Masse ReserveChocolat { get; private set; }

        /// <summary>
        /// Date utilisée pour valider les dates limites des lots.
        /// </summary>
        public DateTime DateDuJour { get; set; }

        /// <summary>
        /// Lots de production de l'atelier, planifiés ou terminés.
        /// </summary>
        public List<LotProduction> Lots { get; } = new List<LotProduction>();

        /// <summary>
        /// Nombre de tickets d'or cachés depuis le début de la saison.
        /// </summary>
        public int TicketsCaches { get; private set; }


        public Atelier(Masse reserveChocolat, DateTime dateDuJour)
        {
            ReserveChocolat = reserveChocolat;
            DateDuJour = dateDuJour;
        }

        /// <summary>
        /// Ajoute une livraison de chocolat à la réserve. La livraison est convertie
        /// dans l'unité de la réserve avant d'être ajoutée.
        /// </summary>
        /// <param name="livraison">La quantité de chocolat livrée</param>
        /// <exception cref="ArgumentOutOfRangeException">La quantité livrée est inférieure ou égale à 0.</exception>
        public void Reapprovisionner(Masse livraison)
        {
            if (livraison.Valeur <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(livraison), "La quantité livrée doit être positive.");
            }

            Masse livraisonConvertie = livraison.ConvertirEn(ReserveChocolat.Unite);
            ReserveChocolat = new Masse(ReserveChocolat.Valeur + livraisonConvertie.Valeur, ReserveChocolat.Unite);
        }


        /// <summary>
        /// Planifie un nouveau lot de production (création, validation et ajout aux lots existants). 
        /// </summary>
        /// <param name="nom">Le nom de la création à fabriquer.</param>
        /// <param name="quantite">Le nombre d'unités à fabriquer.</param>
        /// <param name="poidsUnitaire">Le poids d'une unité.</param>
        /// <param name="dateLimite">La date avant laquelle le lot doit être fabriqué.</param>
        public void PlanifierLot(string nom, int quantite, Masse poidsUnitaire, DateTime dateLimite)
        {

        }

        /// <summary>
        /// Vérifie qu'un lot respecte les règles de planification.
        /// Lance une exception à la première règle non respectée.
        /// </summary>
        /// <param name="lot">Le lot à valider.</param>
        /// <exception cref="ArgumentException">La quantité ou le poids est inférieur ou égal à 0.</exception>
        /// <exception cref="DateLimiteDepasseeException">La date limite est déjà passée.</exception>
        /// <exception cref="ReserveInsuffisanteException">La réserve de chocolat ne suffit pas pour ce lot.</exception>
        private void ValiderLot(LotProduction lot)
        {
            if (lot.Creation.Quantite <= 0)
            {
                throw new ArgumentException("La quantité doit être supérieure à 0.");
            }

            if (lot.Creation.PoidsUnitaire.Valeur <= 0)
            {
                throw new ArgumentException("Le poids doit être supérieur à 0.");
            }

            if (lot.DateLimite < DateDuJour)
            {
                throw new DateLimiteDepasseeException("La date limite est déjà passée.");
            }

            Masse chocolatRequis = lot.Creation.CalculerChocolatRequis();

            if (chocolatRequis.EnGrammes() > ReserveChocolat.EnGrammes())
            {
                throw new ReserveInsuffisanteException("La réserve de chocolat est insuffisante.");
            }
        }

        /// <summary>
        /// Modifie la quantité d'un lot encore planifié.
        /// La quantité n'est modifiée que si toutes les vérifications réussissent.
        /// </summary>
        /// <param name="numeroLot">Le numéro du lot à modifier.</param>
        /// <param name="nouvelleQuantite">La nouvelle quantité du lot.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ReserveInsuffisanteException"></exception>
        public void ModifierQuantite(int numeroLot, int nouvelleQuantite)
        {
            LotProduction lot = ObtenirLotPlanifie(numeroLot);

            if (nouvelleQuantite <= 0)
            {
                throw new ArgumentException("La quantité doit être supérieure à 0.");
            }

            double grammesRequis = lot.Creation.PoidsUnitaire.EnGrammes() * nouvelleQuantite;

            if (grammesRequis > ReserveChocolat.EnGrammes())
            {
                throw new ReserveInsuffisanteException("La réserve de chocolat est insuffisante.");
            }

        }

        /// <summary>
        /// Annule un lot encore planifié en le retirant de la liste des lots.
        /// </summary>
        /// <param name="numeroLot">Le numéro du lot à annuler.</param>
        public void AnnulerLot(int numeroLot)
        {
            LotProduction lot = ObtenirLotPlanifie(numeroLot);
            Lots.Remove(lot);
        }

        /// <summary>
        /// Recherche un lot par son numéro.
        /// </summary>
        /// <param name="numero">Le numéro du lot recherché.</param>
        /// <returns>Le lot trouvé, ou null si aucun lot ne porte ce numéro.</returns>
        private LotProduction? TrouverLot(int numero)
        {
            foreach (LotProduction lot in Lots)
            {
                if (lot.Numero == numero)
                {
                    return lot;
                }
            }

            return null;
        }

        /// <summary>
        /// Retourne un lot qui existe et qui est encore planifié.
        /// </summary>
        /// <param name="numero">Le numéro du lot recherché.</param>
        /// <returns></returns>
        /// <exception cref="LotIntrouvableException">Aucun lot ne porte ce numéro.</exception>
        /// <exception cref="InvalidOperationException">Le lot est déjà terminé.</exception>
        public LotProduction ObtenirLotPlanifie(int numero)
        {
            LotProduction? lot = TrouverLot(numero);

            if (lot == null)
            {
                throw new LotIntrouvableException($"Aucun lot numéro {numero}.");
            }

            if (lot.Statut != StatutLot.Planifie)
            {
                throw new InvalidOperationException($"Le lot {numero} est déjà terminé.");
            }

            return lot;
        }


        /// <summary>
        /// Fabrique un lot planifié : retire de la réserve le chocolat utilisé,
        /// fait passer le lot au statut Terminé, puis cache les tickets d'or dans le lot.
        /// </summary>
        /// <param name="numeroLot">Le numéro du lot à terminer.</param>
        /// <exception cref="ReserveInsuffisanteException">La réserve ne suffit plus pour fabriquer ce lot.</exception>
        public void TerminerLot(int numeroLot)
        {
            LotProduction lot = ObtenirLotPlanifie(numeroLot);
            Masse chocolatRequis = lot.Creation.CalculerChocolatRequis();

            // Revérification : d'autres lots ont pu utiliser la réserve depuis la planification.
            if (chocolatRequis.EnGrammes() > ReserveChocolat.EnGrammes())
            {
                throw new ReserveInsuffisanteException("La réserve de chocolat est insuffisante pour terminer ce lot.");
            }

            Masse requisConverti = chocolatRequis.ConvertirEn(ReserveChocolat.Unite);
            ReserveChocolat = new Masse(ReserveChocolat.Valeur - requisConverti.Valeur, ReserveChocolat.Unite);

            CacherTickets(lot);
        }

        /// <summary>
        /// Enregistre le nombre d'unités invendues d'un lot terminé.
        /// </summary>
        /// <param name="numeroLot">Le numéro du lot.</param>
        /// <param name="quantiteInvendue">Le nombre d'unités invendues.</param>
        public void EnregistrerInvendus(int numeroLot, int quantiteInvendue)
        {
            LotProduction lot = ObtenirLotTermine(numeroLot);
            lot.QuantiteInvendue = quantiteInvendue;
        }

        /// <summary>
        /// Retourne un lot qui existe et qui est terminé.
        /// </summary>
        /// <param name="numero">Le numéro du lot recherché.</param>
        /// <returns>Le lot terminé.</returns>
        /// <exception cref="LotIntrouvableException">Aucun lot ne porte ce numéro.</exception>
        /// <exception cref="InvalidOperationException">Le lot n'est pas encore terminé.</exception>
        public LotProduction ObtenirLotTermine(int numero)
        {
            LotProduction? lot = TrouverLot(numero);

            if (lot == null)
            {
                throw new LotIntrouvableException($"Aucun lot numéro {numero}.");
            }

            if (lot.Statut != StatutLot.Termine)
            {
                throw new InvalidOperationException($"Le lot {numero} n'est pas encore terminé.");
            }

            return lot;
        }

        /// <summary>
        /// Enregistre le nom du gagnant d'un ticket d'or, peu importe le lot où il a été caché.
        /// Si le ticket a déjà un gagnant, son nom est remplacé.
        /// </summary>
        /// <param name="numeroTicket">Le numéro du ticket trouvé.</param>
        /// <param name="nomGagnant">Le nom de la personne qui a trouvé le ticket.</param>
        /// <exception cref="ArgumentException">Aucun ticket ne porte ce numéro.</exception>
        public void EnregistrerGagnant(int numeroTicket, string nomGagnant)
        {
            foreach (LotProduction lot in Lots)
            {
                if (lot.EnregistrerGagnant(numeroTicket, nomGagnant))
                {
                    return;
                }
            }

            throw new ArgumentException($"Aucun ticket numéro {numeroTicket}.");
        }

        /// <summary>
        /// Retourne les lots dans lesquels au moins un ticket d'or a été caché.
        /// </summary>
        /// <returns>Une nouvelle liste contenant les lots avec des tickets.</returns>
        public List<LotProduction> ObtenirLotsAvecTickets()
        {
            List<LotProduction> resultat = new List<LotProduction>();

            foreach (LotProduction lot in Lots)
            {
                if (lot.Tickets.Count > 0)
                {
                    resultat.Add(lot);
                }
            }

            return resultat;
        }

        /// <summary>
        /// Cache les tickets d'or dans un lot qui vient d'être terminé.
        /// </summary>
        /// <param name="lot">Le lot qui vient d'être terminé.</param>
        private void CacherTickets(LotProduction lot)
        {
        }
    }
}

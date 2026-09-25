using ChocolaterieDeWilly.Enumerations;
using ChocolaterieDeWilly.ExceptionsPersonnalisees;
using ChocolaterieDeWilly.Models;
using System.Globalization;

namespace ChocolaterieDeWilly
{
    public static class AffichageMenu
    {
        private const string Banniere = @"
  _                  _                     _       _            _      
 | |                | |                   | |     | |          (_)     
 | |     __ _    ___| |__   ___   ___ ___ | | __ _| |_ ___ _ __ _  ___ 
 | |    / _` |  / __| '_ \ / _ \ / __/ _ \| |/ _` | __/ _ | '__| |/ _ \
 | |___| (_| | | (__| | | | (_) | (_| (_) | | (_| | ||  __| |  | |  __/
 |______\__,_|  \___|_| |_|\_____\___\___/|_|\__,_|\__\___|_|  |_|\___|
         | |      \ \        / (_| | |                                 
       __| | ___   \ \  /\  / / _| | |_   _                            
      / _` |/ _ \   \ \/  \/ / | | | | | | |                           
     | (_| |  __/    \  /\  /  | | | | |_| |                           
      \__,_|\___|     \/  \/   |_|_|_|\__, |                           
                                       __/ |                           
                                      |___/                                                                                                          
        ";

        // Le ! après null fait taire l'avertissement que le type est non nullable, en attendant qu'on lui assigne un objet.
        private static Atelier _atelier = null!;

        public static void Demarrer(Atelier atelier)
        {
            _atelier = atelier;
            bool quitter = false;

            while (!quitter)
            {
                AfficherMenuPrincipal();
                int choix = LireEntier("Votre choix : ", 0, 4);

                Console.WriteLine();

                switch (choix)
                {
                    case 1:
                        Reapprovisionner();
                        Pause();
                        break;
                    case 2:
                        MenuLots();
                        break;
                    case 3:
                        MenuProduction();
                        break;
                    case 4:
                        MenuTickets();
                        break;
                    case 0:
                        quitter = true;
                        break;
                }

            }
            Console.WriteLine("Bonne saison, et que le meilleur gagne son ticket d'or !");
        }

        // ----- Menu principal -----
        private static void AfficherMenuPrincipal()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Banniere);
            Console.ResetColor();
            Console.WriteLine($"Date du jour : {_atelier.DateDuJour:d MMMM yyyy}");
            Console.WriteLine($"Réserve de chocolat : {_atelier.ReserveChocolat}");
            Console.WriteLine();
            Console.WriteLine("1. Réapprovisionner la réserve");
            Console.WriteLine("2. Planifier / modifier / annuler un lot");
            Console.WriteLine("3. Terminer un lot / enregistrer les invendus");
            Console.WriteLine("4. Tickets d'or");
            Console.WriteLine("0. Quitter");
            Console.WriteLine();
        }

        // ----- Réserve -----

        private static void Reapprovisionner()
        {
            Masse livraison = LireMasse("Quantité livrée : ");

            try
            {
                _atelier.Reapprovisionner(livraison);
                Console.WriteLine($"Nouvelle réserve : {_atelier.ReserveChocolat}");
            }
            catch (ArgumentException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        // ----- Lots -----

        private static void MenuLots()
        {
            bool retour = false;

            while (!retour)
            {
                Console.Clear();
                Console.WriteLine("=== Lots de production ===");
                Console.WriteLine();
                Console.WriteLine("1. Lister les lots");
                Console.WriteLine("2. Planifier un lot");
                Console.WriteLine("3. Modifier la quantité d'un lot");
                Console.WriteLine("4. Annuler un lot");
                Console.WriteLine("0. Retour");
                Console.WriteLine();

                int choix = LireEntier("Votre choix : ", 0, 4);
                Console.WriteLine();

                switch (choix)
                {
                    case 1:
                        ListerLots();
                        Pause();
                        break;
                    case 2:
                        PlanifierLot();
                        Pause();
                        break;
                    case 3:
                        ModifierQuantiteLot();
                        Pause();
                        break;
                    case 4:
                        AnnulerLot();
                        Pause();
                        break;
                    case 0:
                        retour = true;
                        break;
                }
            }
        }

        private static void ListerLots()
        {
            List<LotProduction> lots = _atelier.Lots;


            if (lots.Count == 0)
            {
                Console.WriteLine("Aucun lot.");
                return;
            }

            foreach (LotProduction lot in lots)
            {
                Creation creation = lot.Creation;
                Console.WriteLine($"Lot {lot.Numero} : {creation.Nom} x {creation.Quantite}");
                Console.WriteLine($"   Chocolat requis : {creation.CalculerChocolatRequis()}");
                Console.WriteLine($"   Date limite : {lot.DateLimite:d MMMM yyyy}   Statut : {lot.Statut}");

                if (lot.Statut == StatutLot.Termine)
                {
                    Console.WriteLine($"   Invendus : {lot.QuantiteInvendue}");
                }
            }
        }


        private static void PlanifierLot()
        {
            string nom = LireTexte("Nom de la création : ");
            int quantite = LireEntier("Quantité : ");
            Masse poids = LireMasse("Poids d'une unité : ");
            DateTime dateLimite = LireDate("Date limite (aaaa-mm-jj) : ");

            try
            {
                _atelier.PlanifierLot(nom, quantite, poids, dateLimite);
                Console.WriteLine("Le lot a été planifié.");
            }
            catch (ArgumentException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (DateLimiteDepasseeException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (ReserveInsuffisanteException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        private static void ModifierQuantiteLot()
        {
            ListerLots();
            Console.WriteLine();

            int numero = LireEntier("Numéro du lot : ");

            try
            {
                LotProduction lot = _atelier.ObtenirLotPlanifie(numero);
                Console.WriteLine($"Quantité actuelle : {lot.Creation.Quantite}");

                int quantite = LireEntier("Nouvelle quantité : ");
                _atelier.ModifierQuantite(numero, quantite);
                Console.WriteLine("La quantité a été modifiée.");
            }
            catch (LotIntrouvableException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (ArgumentException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (ReserveInsuffisanteException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        private static void AnnulerLot()
        {
            ListerLots();
            Console.WriteLine();

            int numero = LireEntier("Numéro du lot à annuler : ");

            try
            {
                LotProduction lot = _atelier.ObtenirLotPlanifie(numero);

                if (!LireOuiNon($"Confirmer l'annulation du lot {numero} ({lot.Creation.Nom}) (o/n) ? "))
                {
                    Console.WriteLine("Annulation abandonnée.");
                    return;
                }

                _atelier.AnnulerLot(numero);
                Console.WriteLine($"Le lot {numero} a été annulé.");
            }
            catch (LotIntrouvableException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        // ----- Affichage -----

        private static void AfficherErreur(string message)
        {
            Console.WriteLine($"Refusé : {message}");
        }

        // ----- Saisies -----

        private static int LireEntier(string question)
        {
            int valeur;
            Console.Write(question);

            // out valeur veut dire : le résultat de transtypage (TryParse) est sauvegardé dans la variable valeur si TryParse a réussi la conversion.
            while (!int.TryParse(Console.ReadLine(), out valeur))
            {
                Console.WriteLine("Veuillez entrer un nombre entier");
                Console.Write(question);
            }

            return valeur;
        }

        private static int LireEntier(string question, int min, int max)
        {
            int valeur = LireEntier(question);

            while (valeur < min || valeur > max)
            {
                Console.WriteLine($"Veuillez entrer un nombre entre {min} et {max}");
                valeur = LireEntier(question);
            }

            return valeur;
        }

        private static double LireDouble(string question)
        {
            double valeur;
            Console.Write(question);

            // out valeur veut dire : le résultat de transtypage (TryParse) est sauvegardé dans la variable valeur si TryParse a réussi la conversion.
            while (!double.TryParse(Console.ReadLine(), out valeur))
            {
                Console.WriteLine("Veuillez entrer un nombre.");
                Console.Write(question);
            }

            return valeur;
        }


        private static string LireTexte(string question)
        {
            Console.Write(question);

            // ?? : si le Console.ReadLine() est null, remplacer par ""
            string texte = (Console.ReadLine() ?? "").Trim();

            while (texte == "")
            {
                Console.WriteLine("Ce champ ne peut pas être vide.");
                Console.Write(question);
                texte = (Console.ReadLine() ?? "").Trim();
            }

            return texte;
        }

        private static Masse LireMasse(string question)
        {
            double valeur = LireDouble(question);
            Console.WriteLine("Unité : 1. Grammes   2. Kilogrammes   3. Livres");
            int choixUnite = LireEntier("Votre choix : ", 1, 3);

            UniteMasse unite = UniteMasse.Grammes;
            if (choixUnite == 2)
            {
                unite = UniteMasse.Kilogrammes;
            }
            else if (choixUnite == 3)
            {
                unite = UniteMasse.Livres;
            }

            return new Masse(valeur, unite);
        }

        private static DateTime LireDate(string question)
        {
            DateTime date;
            Console.Write(question);

            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out date))
            {
                Console.WriteLine("Veuillez entrer une date au format aaaa-mm-jj.");
                Console.Write(question);
            }

            return date;
        }


        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Appuyez sur Entrée pour continuer...");
            Console.ReadLine();
        }

        private static bool LireOuiNon(string question)
        {
            Console.Write(question);
            string reponse = (Console.ReadLine() ?? "").Trim().ToLower();

            while (reponse != "o" && reponse != "n")
            {
                Console.WriteLine("Veuillez répondre par o ou n.");
                Console.Write(question);
                reponse = (Console.ReadLine() ?? "").Trim().ToLower();
            }

            return reponse == "o";
        }

        // ----- Production -----

        private static void MenuProduction()
        {
            bool retour = false;

            while (!retour)
            {
                Console.Clear();
                Console.WriteLine("=== Production ===");
                Console.WriteLine($"Réserve de chocolat : {_atelier.ReserveChocolat}");
                Console.WriteLine();
                Console.WriteLine("1. Terminer un lot");
                Console.WriteLine("2. Enregistrer les invendus d'un lot");
                Console.WriteLine("0. Retour");
                Console.WriteLine();

                int choix = LireEntier("Votre choix : ", 0, 2);
                Console.WriteLine();

                switch (choix)
                {
                    case 1:
                        TerminerLot();
                        Pause();
                        break;
                    case 2:
                        EnregistrerInvendus();
                        Pause();
                        break;
                    case 0:
                        retour = true;
                        break;
                }
            }
        }

        private static void TerminerLot()
        {
            ListerLots();
            Console.WriteLine();

            int numero = LireEntier("Numéro du lot à terminer : ");

            try
            {
                _atelier.TerminerLot(numero);
                Console.WriteLine($"Le lot {numero} est terminé.");
                Console.WriteLine($"Réserve restante : {_atelier.ReserveChocolat}");
                int nombreTickets = _atelier.ObtenirLotTermine(numero).Tickets.Count;
                if (nombreTickets > 1)
                {
                    Console.WriteLine($"{nombreTickets} ticket(s) d'or caché(s) dans ce lot !");
                }
            }
            catch (LotIntrouvableException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (ReserveInsuffisanteException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        private static void EnregistrerInvendus()
        {
            ListerLots();
            Console.WriteLine();

            int numero = LireEntier("Numéro du lot : ");

            try
            {
                LotProduction lot = _atelier.ObtenirLotTermine(numero);
                Console.WriteLine($"Quantité produite : {lot.Creation.Quantite}");

                int invendus = LireEntier("Nombre d'invendus : ");
                _atelier.EnregistrerInvendus(numero, invendus);
                Console.WriteLine("Les invendus ont été enregistrés.");
            }
            catch (LotIntrouvableException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AfficherErreur(ex.Message);
            }
            catch (ArgumentException ex)
            {
                AfficherErreur(ex.Message);
            }
        }

        // ----- Tickets d'or -----

        private static void MenuTickets()
        {
            bool retour = false;

            while (!retour)
            {
                Console.Clear();
                Console.WriteLine("=== Tickets d'or ===");
                Console.WriteLine($"Tickets cachés : {_atelier.TicketsCaches} / {Atelier.MaxTicketsOr}");
                Console.WriteLine();
                Console.WriteLine("1. Afficher les tickets");
                Console.WriteLine("2. Enregistrer un gagnant");
                Console.WriteLine("0. Retour");
                Console.WriteLine();

                int choix = LireEntier("Votre choix : ", 0, 2);
                Console.WriteLine();

                switch (choix)
                {
                    case 1:
                        AfficherTickets();
                        Pause();
                        break;
                    case 2:
                        EnregistrerGagnant();
                        Pause();
                        break;
                    case 0:
                        retour = true;
                        break;
                }
            }
        }

        private static void AfficherTickets()
        {
            List<LotProduction> lots = _atelier.ObtenirLotsAvecTickets();

            if (lots.Count == 0)
            {
                Console.WriteLine("Aucun ticket d'or n'a encore été caché.");
            }

            foreach (LotProduction lot in lots)
            {
                Console.WriteLine($"Lot {lot.Numero} : {lot.Creation.Nom}");

                foreach (TicketOr ticket in lot.Tickets)
                {
                    string gagnant = "non trouvé";
                    if (ticket.NomGagnant != "")
                    {
                        gagnant = ticket.NomGagnant;
                    }
                    Console.WriteLine($"   Ticket {ticket.Numero} : {gagnant}");
                }
            }

            int restants = Atelier.MaxTicketsOr - _atelier.TicketsCaches;
            Console.WriteLine();
            Console.WriteLine($"Tickets restant à cacher : {restants}");
        }

        private static void EnregistrerGagnant()
        {
            AfficherTickets();
            Console.WriteLine();

            int numero = LireEntier("Numéro du ticket : ");
            string nom = LireTexte("Nom du gagnant : ");

            try
            {
                _atelier.EnregistrerGagnant(numero, nom);
                Console.WriteLine($"Félicitations à {nom} !");
            }
            catch (ArgumentException ex)
            {
                AfficherErreur(ex.Message);
            }
        }
    }
}

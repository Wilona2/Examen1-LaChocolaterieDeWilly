using ChocolaterieDeWilly.Enumerations;
using ChocolaterieDeWilly.ExceptionsPersonnalisees;
using ChocolaterieDeWilly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChocolaterieDeWillyTests
{
    public class AtelierTests
    {

        [Fact]
        public void Reapprovisionner_LivraisonEnGrammes_ConvertitDansUniteDeLaReserve()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);

            atelier.Reapprovisionner(new Masse(500, UniteMasse.Grammes));

            Assert.Equal(50.5, atelier.ReserveChocolat.Valeur, 3);
            Assert.Equal(UniteMasse.Kilogrammes, atelier.ReserveChocolat.Unite);
        }

        [Fact]
        public void PlanifierLot_ValeursValides_AjouteLeLot()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);

            atelier.PlanifierLot("Feuille d'érable", 100, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            Assert.Single(atelier.Lots);
            Assert.Equal("Feuille d'érable", atelier.Lots[0].Creation.Nom);
            Assert.Equal(StatutLot.Planifie, atelier.Lots[0].Statut);
        }

        [Fact]
        public void ObtenirLotPlanifie_NumeroInexistant_DeclencheException()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);

            LotIntrouvableException exception = Assert.Throws<LotIntrouvableException>(() =>
                atelier.ObtenirLotPlanifie(99));

            Assert.Equal("Aucun lot numéro 99.", exception.Message);
        }

        [Fact]
        public void ModifierQuantite_QuantiteValide_ModifieLaQuantite()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 100, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            atelier.ModifierQuantite(1, 150);

            Assert.Equal(150, atelier.Lots[0].Creation.Quantite);
        }

        [Fact]
        public void ModifierQuantite_QuantiteNulle_DeclencheExceptionSansModifier()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 100, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                atelier.ModifierQuantite(1, 0));

            Assert.Equal("La quantité doit être supérieure à 0.", exception.Message);
            Assert.Equal(100, atelier.Lots[0].Creation.Quantite);
        }

        [Fact]
        public void AnnulerLot_LotPlanifie_RetireLeLotDeLaListe()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 100, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));
            atelier.PlanifierLot("Citrouille moulée", 10, new Masse(200, UniteMasse.Grammes),
                                 dateDuJour.AddDays(20));

            Assert.Fail();
        }

        [Fact]
        public void EnregistrerInvendus_QuantiteValide_EnregistreLesInvendus()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Citrouille moulée", 10, new Masse(200, UniteMasse.Grammes),
                                 dateDuJour.AddDays(20));
            atelier.TerminerLot(1);

            Assert.Fail();
        }

        [Fact]
        public void TerminerLot_LotPlanifie_PasseAuStatutTermine()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Citrouille moulée", 10, new Masse(200, UniteMasse.Grammes),
                                 dateDuJour.AddDays(20));

            atelier.TerminerLot(1);

            Assert.Equal(StatutLot.Termine, atelier.Lots[0].Statut);
        }

        [Fact]
        public void EnregistrerInvendus_PlusQueLaQuantiteProduite_DeclencheException()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Citrouille moulée", 10, new Masse(200, UniteMasse.Grammes),
                                 dateDuJour.AddDays(20));
            atelier.TerminerLot(1);

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                atelier.EnregistrerInvendus(1, 11));

            Assert.Equal("Les invendus doivent être entre 0 et 10.", exception.Message);
            Assert.Equal(0, atelier.Lots[0].QuantiteInvendue);
        }


        [Fact]
        public void TerminerLot_MoinsDe500Unites_AucunTicketCache()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 499, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            atelier.TerminerLot(1);

            Assert.Equal(0, atelier.TicketsCaches);
            Assert.Empty(atelier.Lots[0].Tickets);
        }

        [Fact]
        public void TerminerLot_1200Unites_CacheDeuxTickets()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 1200, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            atelier.TerminerLot(1);

            Assert.Equal(2, atelier.TicketsCaches);
            Assert.Equal(2, atelier.Lots[0].Tickets.Count);
            Assert.Equal(1, atelier.Lots[0].Tickets[0].Numero);
            Assert.Equal(2, atelier.Lots[0].Tickets[1].Numero);
        }

        [Fact]
        public void TerminerLot_CompteurContinueDUnLotALAutre()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 400, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));
            atelier.PlanifierLot("Feuille d'érable", 200, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(12));

            atelier.TerminerLot(1);
            atelier.TerminerLot(2);

            Assert.Empty(atelier.Lots[0].Tickets);
            Assert.Single(atelier.Lots[1].Tickets);
            Assert.Equal(2, atelier.Lots[1].Tickets[0].NumeroLot);
        }

        [Fact]
        public void TerminerLot_4000Unites_Maximum5Tickets()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Bouchée", 4000, new Masse(1, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));

            atelier.TerminerLot(1);

            Assert.Equal(5, atelier.TicketsCaches);
            Assert.Equal(5, atelier.Lots[0].Tickets.Count);
        }

        [Fact]
        public void EnregistrerGagnant_TicketExistant_EnregistreLeNom()
        {
            DateTime dateDuJour = new DateTime(2026, 10, 1);
            Atelier atelier = new Atelier(new Masse(50, UniteMasse.Kilogrammes), dateDuJour);
            atelier.PlanifierLot("Feuille d'érable", 1200, new Masse(25, UniteMasse.Grammes),
                                 dateDuJour.AddDays(10));
            atelier.TerminerLot(1);

            atelier.EnregistrerGagnant(2, "Charlie");

            Assert.Equal("", atelier.Lots[0].Tickets[0].NomGagnant);
            Assert.Equal("Charlie", atelier.Lots[0].Tickets[1].NomGagnant);
        }
    }
}

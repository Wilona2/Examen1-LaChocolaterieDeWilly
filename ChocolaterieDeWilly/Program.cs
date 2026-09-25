using ChocolaterieDeWilly;
using ChocolaterieDeWilly.Enumerations;
using ChocolaterieDeWilly.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Masse reserveDepartChocolat = new Masse(50, UniteMasse.Kilogrammes);
DateTime date = DateTime.Today;

Atelier atelier = new Atelier(reserveDepartChocolat, date);

// Données d'essai
atelier.PlanifierLot("Feuille d'érable", 1200, new Masse(25, UniteMasse.Grammes),
                     date.AddDays(10));
atelier.PlanifierLot("Citrouille moulée", 40, new Masse(200, UniteMasse.Grammes),
                     date.AddDays(20));

AffichageMenu.Demarrer(atelier);
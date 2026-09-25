using ChocolaterieDeWilly.Enumerations;
using ChocolaterieDeWilly.Models;

namespace ChocolaterieDeWillyTests
{
    public class MasseTests
    {
        [Fact]
        public void Kilogrammes_EnGrammes_MultipliePar1000()
        {
            Masse masse = new Masse(2.5, UniteMasse.Kilogrammes);
            Assert.Equal(2500, masse.EnGrammes());
        }

        [Fact]
        public void ConvertirEn_GrammesVersKilogrammes_DiviseParMille()
        {
            Masse masse = new Masse(2500, UniteMasse.Grammes);

            Masse convertie = masse.ConvertirEn(UniteMasse.Kilogrammes);

            Assert.Equal(2.5, convertie.Valeur, 3);
            Assert.Equal(UniteMasse.Kilogrammes, convertie.Unite);
        }
    }
}

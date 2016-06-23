using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LPSloepke;

namespace LPSloepkeUnitTests
{
    [TestClass]
    public class UnitTestSloepke
    {
        [TestMethod]
        public void TestTotalePrijsBereken()
        {
            Administratie.contract = new Huurcontract();
            Administratie.contract.IJsselmeer = true;
            Administratie.contract.Noordzee = true;
            Administratie.contract.Artikelen.Add(new Boot("boot", 15, BootType.Kruiser, true, 12));
            Administratie.contract.Artikelen.Add(new Accessoire("vest", 1.25, 3));
            Assert.AreEqual(3 * 1.25 + 15 + 2 + 2, Administratie.contract.BerekenPrijs());
        }

        [TestMethod]
        public void TestBudgetMerenBerekenen()
        {
            Administratie.contract = new Huurcontract();
            Administratie.contract.IJsselmeer = true;
            Administratie.contract.Noordzee = true;
            Administratie.contract.Artikelen.Add(new Boot("boot", 15, BootType.Kruiser, true, 12));
            Administratie.contract.Artikelen.Add(new Boot("boot", 10, BootType.Kruiser, true, 12));
            Administratie.contract.Artikelen.Add(new Accessoire("vest", 1.25, 6));
            // 15 + 10 + 6 * 1.25 + 2 + 2 = 40.5
            // 48 - 40.5 = 7.5
            // 7.5 / 1.5 = 5, een 6e zou 9 euro kosten en dat zit niet in het budget, verwachte meren is 5
            Assert.AreEqual(5, Administratie.contract.BerekenMeren(48));
        }

        [TestMethod]
        public void TestInsertContract()
        {
            Huurcontract contract = new Huurcontract();
            contract.Email = "naam@mail.nl";
            contract.Naam = "naam";
            contract.IJsselmeer = true;
            contract.Noordzee = false;
            contract.FrieseMeren = 6;
            contract.Begin = new DateTime(2016, 12, 12);
            contract.Einde = new DateTime(2016, 12, 17);
            contract.Artikelen.Add(new Boot("Boaty mcBoat", 15, BootType.Kruiser, true, 12));
            contract.Artikelen.Add(new Accessoire("Reddingsvest", 1.25, 4));
            contract.Artikelen.Add(new Accessoire("Peddel", 1.25, 8));
            // als alle inserts goed gaan, returned deze true
            Assert.AreEqual(true, Database.InsertContract(contract));
        }
    }
}

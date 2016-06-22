using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPSloepke
{
    public class Huurcontract
    {
        public int ID;
        public string Naam;
        public string Email;
        public DateTime Begin;
        public DateTime Einde;
        public bool IJsselmeer;
        public bool Noordzee;
        public int FrieseMeren;
        public List<Artikel> Artikelen;

        public Huurcontract()
        {

        }

        public string ExportContract()
        {
            return "Huurder: " + Naam + Environment.NewLine + "Email: " + Email;
        }

        public double BerekenPrijs()
        {
            double artikelPrijs = 0;
            foreach (Artikel a in Artikelen)
            {
                artikelPrijs += a.Prijs;
            }
            return (IJsselmeer ? 2 : 0) + (Noordzee ? 2 : 0) + FrieseMeren + (FrieseMeren > 5? 0.5 * FrieseMeren : 0) + artikelPrijs;
        }
    }
}

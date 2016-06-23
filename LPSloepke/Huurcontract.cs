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
            Artikelen = new List<Artikel>();
        }

        public string ExportContract()
        {
            return "Huurder: " + Naam + Environment.NewLine + "Email: " + Email;
        }

        public double BerekenPrijs()
        {
            double totalePrijs = 0;
            foreach (Artikel a in Artikelen.Where(x => x is Accessoire))
            {
                totalePrijs += a.Prijs;
            }
            foreach (Artikel a in Artikelen.Where(x => x is Boot))
            {
                if (((Boot)a).BootType != BootType.Kano)
                {
                    totalePrijs += (IJsselmeer ? 2 : 0) + (Noordzee ? 2 : 0) + FrieseMeren + (FrieseMeren > 5 ? 0.5 * FrieseMeren : 0);
                }
                else
                {
                    totalePrijs += FrieseMeren;
                }
            }
            return totalePrijs;
        }

        public int BerekenMeren(int budget)
        {
            return 0;
        }
    }
}

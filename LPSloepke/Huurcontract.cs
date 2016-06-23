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

        public int BerekenMeren(int budget)
        {
            TimeSpan span = Einde.Subtract(Begin);
            int dagen = (int)Math.Round(span.TotalDays);
            double bud = budget;
            if (dagen > 1)
            {
                bud = budget / dagen;
            }
            double kosten = 0;
            foreach (Artikel a in Artikelen.Where(x => x is Accessoire))
            {
                kosten += a.Prijs * (a as Accessoire).Aantal;
            }
            foreach (Artikel a in Artikelen.Where(x => x is Boot))
            {
                if (((Boot)a).BootType != BootType.Kano)
                {
                    kosten += (IJsselmeer ? 2 : 0) + (Noordzee ? 2 : 0);
                    kosten += a.Prijs;
                }
            }
            bud = bud - kosten;
            return (bud > 7.5 ? (int)Math.Round(bud/1.5) : (int)Math.Round(bud));
        }
    }
}

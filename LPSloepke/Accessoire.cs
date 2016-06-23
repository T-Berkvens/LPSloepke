using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPSloepke
{
    public class Accessoire : Artikel
    {
        public int Aantal;

        public Accessoire(string naam, double prijs, int aantal): base(naam, prijs)
        {
            Aantal = aantal;
        }

        public string ToString(bool selected)
        {
            if (selected)
            {
                return "Naam: " + Naam + Environment.NewLine +
                    "Aantal: " + Aantal + Environment.NewLine +
                    "Prijs: " + (Aantal * Prijs).ToString("C");
            }
            else
            {
                return "Naam: " + Naam + Environment.NewLine +
                    "Prijs per stuk: " + Prijs.ToString("C") + Environment.NewLine +
                    "Aantal: " + Aantal;
            }
        }

        public Accessoire Clone()
        {
            return (Accessoire)this.MemberwiseClone();
        }
    }
}

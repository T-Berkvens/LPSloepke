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
    }
}

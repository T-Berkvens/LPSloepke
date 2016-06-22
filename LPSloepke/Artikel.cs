using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPSloepke
{
    public abstract class Artikel
    {
        public string Naam;
        public double Prijs;

        public Artikel(string naam, double prijs)
        {
            Naam = naam;
            Prijs = prijs;
        }
    }
}

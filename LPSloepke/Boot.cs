using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPSloepke
{
    public class Boot : Artikel
    {
        BootType BootType;
        public bool Motor;
        public double Tankinhoud;

        public Boot(string naam, double prijs, BootType boottype, bool motor, double tankinhoud): base(naam, prijs)
        {
            BootType = boottype;
            Motor = motor;
            Tankinhoud = tankinhoud;
        }
    }

    public enum BootType
    {
        Kano,
        Valk,
        Laser,
        Kruiser
    };
}

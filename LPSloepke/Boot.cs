using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPSloepke
{
    public class Boot : Artikel
    {
        public BootType BootType;
        public bool Motor;
        public double Tankinhoud;

        public Boot(string naam, double prijs, BootType boottype, bool motor, double tankinhoud): base(naam, prijs)
        {
            BootType = boottype;
            Motor = motor;
            Tankinhoud = tankinhoud;
        }

        public override string ToString()
        {
            return "Boot: " + Naam + Environment.NewLine +
                "Type: " + BootType.ToString() + Environment.NewLine +
                "Prijs: " + Prijs.ToString("C") + (Motor ? Environment.NewLine + "Actieradius: " + Tankinhoud*15 + "km": "");
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

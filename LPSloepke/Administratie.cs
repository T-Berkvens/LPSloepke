using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LPSloepke
{
    public static class Administratie
    {
        public static Huurcontract contract = new Huurcontract();

        /// <summary>
        /// Vraagt het pad om bestand op te slaan van het contract dat zojuist is ingevuld
        /// </summary>
        public static void ExportToText()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                WriteFile(sfd.FileName, contract);
            }
        }

        /// <summary>
        /// Vraagt het pad om bestand op te slaan van het contract dat rechts is geselecteerd
        /// </summary>
        public static void ExportToText(Huurcontract hc)
        {
            hc.Artikelen =  GetHuurContractDetails(hc);
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                WriteFile(sfd.FileName, hc);
            }
        }

        public static void ContractToDB()
        {
            Database.InsertContract(contract);
        }

        /// <summary>
        /// Maakt het bestand aan met de contractgegevens
        /// </summary>
        private static void WriteFile(string path, Huurcontract hc)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Huurcontract 't Sloepke");
                sw.WriteLine("\n");
                sw.WriteLine("Huurder: " + hc.Naam);
                sw.WriteLine("Van: " + hc.Begin.ToString());
                sw.WriteLine("Tot: " + hc.Einde.ToString());
                sw.WriteLine("\n");
                sw.WriteLine("Gehuurde boten:");
                foreach (Artikel a in hc.Artikelen.Where(x => x is Boot))
                {
                    Boot boot = a as Boot;
                    sw.WriteLine(boot.Naam);
                    sw.WriteLine(boot.BootType.ToString());
                    sw.WriteLine("Prijs: " + boot.Prijs.ToString("C"));
                }
                sw.WriteLine("\n");
                sw.WriteLine("Gehuurde artikelen:");
                foreach (Artikel a in hc.Artikelen.Where(x => x is Accessoire))
                {
                    Accessoire accessoire = a as Accessoire;
                    sw.WriteLine(accessoire.Naam);
                    sw.WriteLine("Aantal: " + accessoire.Aantal.ToString());
                    sw.WriteLine("Totale prijs: " + (accessoire.Prijs * accessoire.Aantal).ToString("C"));
                    sw.WriteLine("\n");
                }
                sw.WriteLine("\n");
                sw.WriteLine("'t Sloepke");
                sw.WriteLine("Bij het IJsselmeerstraat 12");
                sw.WriteLine("3882HZ");
                sw.WriteLine("06123456789");
            }
        }

        public static List<Boot> LaadBoten()
        {
            return Database.LaadBoten();
        }

        public static List<Accessoire> LaadAccessoires()
        {
            return Database.LaadAccessoires();
        }

        public static List<Huurcontract> LaadContracten()
        {
            return Database.LaadContracten().OrderBy(x => x.Begin).ToList();   
        }

        public static List<Artikel> GetHuurContractDetails(int id)
        {
            contract.Artikelen = Database.GetHuurContractDetails(id);
            return contract.Artikelen;
        }

        public static List<Artikel> GetHuurContractDetails(Huurcontract hc)
        {
            return Database.GetHuurContractDetails(hc.ID);
        }

        public static int BerekenMeren(int budget)
        {
            return contract.BerekenMeren(budget);
        }

        public static string BerekenGevoelsTemperatuur()
        {
            string txt = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line = "";
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        string[] parts = line.Split(';');
                        if (Convert.ToDateTime(parts[0]).Ticks <= contract.Einde.Ticks && Convert.ToDateTime(parts[0]).Ticks >= contract.Begin.Ticks - 864000000000)
                        {
                            int tLucht = Convert.ToInt32(parts[1]);
                            int luchtV = Convert.ToInt32(parts[2]);
                            double gevoelsTemp = Math.Round((33 + (tLucht - 33) * (0.474 + 0.454 * Math.Sqrt((luchtV)) - 0.0454 * luchtV)), 2);
                            txt += "Op: " + parts[0] + " wordt de gevoelstemperatuur: " + gevoelsTemp + " graden Celcius" + Environment.NewLine ;
                        }
                    }
                }
            }
            else
            {
                txt = "Kies een juist bestand om de temperatuur uit te rekenen.";
            }

            return txt;
        }
    }
}

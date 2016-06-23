using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LPSloepke
{
    public static class Administratie
    {
        public static Huurcontract contract = new Huurcontract();

        public static void ExportToText()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WriteFile(ofd.FileName, contract);
            }
        }

        public static void ExportToText(Huurcontract hc)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                WriteFile(ofd.FileName, hc);
            }
        }

        public static void ContractToDB()
        {
            Database.InsertContract(contract);
        }

        private static void WriteFile(string path, Huurcontract hc)
        {

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
            return Database.GetHuurContractDetails(id);
        }

        public static int BerekenMeren(int budget)
        {
            return contract.BerekenMeren(budget);
        }
    }
}

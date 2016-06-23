using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LPSloepke
{
    public partial class Sloepke : Form
    {

        public Sloepke()
        {
            InitializeComponent();
            LaadBoten();
            LaadAccessoires();
            LaadContracten();
        }

        private void LaadBoten()
        {
            dgvBotenTotaal.Rows.Clear();
            foreach (Boot boot in Administratie.LaadBoten())
            {
                int rowId = dgvBotenTotaal.Rows.Add();
                DataGridViewRow row = dgvBotenTotaal.Rows[rowId];
                row.Cells["colBotenTotaalNaam"].Value = boot.Naam;
                row.Cells["colBotenTotaalPrijs"].Value = boot.Prijs;
                row.Tag = boot;
            }
        }

        private void LaadAccessoires()
        {
            dgvArtikelenTotaal.Rows.Clear();
            foreach (Accessoire accessoire in Administratie.LaadAccessoires())
            {
                int rowId = dgvArtikelenTotaal.Rows.Add();
                DataGridViewRow row = dgvArtikelenTotaal.Rows[rowId];
                row.Cells["colArtikelTotaalNaam"].Value = accessoire.Naam;
                row.Cells["colArtikelTotaalPrijs"].Value = accessoire.Prijs;
                row.Tag = accessoire;
            }
        }

        private void LaadContracten()
        {
            dgvTotaalContracten.Rows.Clear();
            foreach (Huurcontract hc in Administratie.LaadContracten())
            {
                int rowId = dgvTotaalContracten.Rows.Add();
                DataGridViewRow row = dgvTotaalContracten.Rows[rowId];
                row.Cells["colContractenHuurder"].Value = hc.Naam;
                row.Cells["colContractenBegin"].Value = hc.Begin.ToString();
                row.Cells["colContractenEinde"].Value = hc.Einde.ToString();
                row.Tag = hc;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }

        private void btnAddBoot_Click(object sender, EventArgs e)
        {
            if (dgvBotenTotaal.SelectedRows[0] != null)
            {
                Boot boot = (Boot)dgvBotenTotaal.SelectedRows[0].Tag;
                int rowId = dgvBotenSelected.Rows.Add();
                DataGridViewRow row = dgvBotenSelected.Rows[rowId];
                row.Cells["colBotenSelectedNaam"].Value = boot.Naam;
                row.Cells["colBotenSelectedPrijs"].Value = boot.Prijs;
                row.Tag = boot;
                Administratie.contract.Artikelen.Add(boot);
                dgvBotenTotaal.Rows.RemoveAt(dgvBotenTotaal.SelectedRows[0].Index);
            }
        }

        private void btnDelBoot_Click(object sender, EventArgs e)
        {
            if (dgvBotenSelected.SelectedRows[0] != null)
            {
                Administratie.contract.Artikelen.RemoveAt(dgvBotenSelected.SelectedRows[0].Index);//fout
                dgvBotenSelected.Rows.RemoveAt(dgvBotenSelected.SelectedRows[0].Index);
            }
            LaadBoten();
        }

        private void btnAddArtikel_Click(object sender, EventArgs e)
        {
            if (dgvArtikelenTotaal.SelectedRows[0] != null)
            {
                Accessoire accessoireTotaal = (Accessoire)dgvArtikelenTotaal.SelectedRows[0].Tag;
                accessoireTotaal.Aantal--;
                bool rowBestaat = false;
                foreach (DataGridViewRow row in dgvArtikelenSelected.Rows)
                {
                    if (row.Cells["colArtikelSelectedNaam"].Value.ToString() == accessoireTotaal.Naam)
                    {
                        rowBestaat = true;
                        Accessoire accessoire = (Accessoire)row.Tag;
                        accessoire.Aantal++;
                    }
                }
                if (!rowBestaat)
                {
                    int rowId = dgvArtikelenSelected.Rows.Add();
                    DataGridViewRow row = dgvArtikelenSelected.Rows[rowId];
                    row.Cells["colArtikelSelectedNaam"].Value = accessoireTotaal.Naam;
                    row.Cells["colArtikelSelectedPrijs"].Value = accessoireTotaal.Prijs;
                    Accessoire accessoire = accessoireTotaal.Clone();
                    accessoire.Aantal = 1;
                    row.Tag = accessoire;
                    Administratie.contract.Artikelen.Add(accessoire);
                }
                if (accessoireTotaal.Aantal < 1)
                {
                    dgvArtikelenTotaal.Rows.RemoveAt(dgvArtikelenTotaal.SelectedRows[0].Index);
                }
            }
        }

        private void btnDelArtikel_Click(object sender, EventArgs e)
        {
            if (dgvArtikelenSelected.SelectedRows[0] != null)
            {
                Administratie.contract.Artikelen.RemoveAt(dgvArtikelenSelected.SelectedRows[0].Index);//fout
                dgvArtikelenSelected.Rows.RemoveAt(dgvArtikelenSelected.SelectedRows[0].Index);
            }
            LaadAccessoires();
        }

        private void cbContractNoordzee_CheckedChanged(object sender, EventArgs e)
        {
            Administratie.contract.Noordzee = cbContractNoordzee.Checked;
        }

        private void cbContractIJsselmeer_CheckedChanged(object sender, EventArgs e)
        {
            Administratie.contract.IJsselmeer = cbContractIJsselmeer.Checked;
        }

        private void btnContractNieuw_Click(object sender, EventArgs e)
        {
            dgvArtikelenSelected.Rows.Clear();
            dgvBotenSelected.Rows.Clear();
            cbContractIJsselmeer.Checked = false;
            cbContractNoordzee.Checked = false;
            tbContractMailAdres.Text = "";
            tbContractNaam.Text = "";
            numContractBudget.Value = 15;
            numContractMeren.Value = 0;
            dtpBegin.Value = DateTime.Now;
            dtpEind.Value = DateTime.Now;
            cbContractExport.Checked = false;
            LaadBoten();
            LaadAccessoires();
            LaadContracten();
        }

        private void btnSaveContract_Click(object sender, EventArgs e)
        {
            Administratie.contract.Naam = tbContractNaam.Text;
            Administratie.contract.Email = tbContractMailAdres.Text;
            Administratie.contract.Begin = dtpBegin.Value;
            Administratie.contract.Einde = dtpEind.Value;
            Administratie.contract.FrieseMeren = Convert.ToInt32(numContractMeren.Value);
            Administratie.ContractToDB();
            if (cbContractExport.Checked)
            {
                Administratie.ExportToText();
            }
            LaadBoten();
            LaadAccessoires();
            LaadContracten();
        }

        private void numContractMeren_ValueChanged(object sender, EventArgs e)
        {
            Administratie.contract.FrieseMeren = Convert.ToInt32(numContractMeren.Value);
        }

        private void dgvBotenTotaal_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Boot boot = (Boot)dgvBotenTotaal.SelectedRows[0].Tag;
            MessageBox.Show(boot.ToString());
        }

        private void dgvArtikelenTotaal_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Accessoire accessoire = (Accessoire)dgvArtikelenTotaal.SelectedRows[0].Tag;
            MessageBox.Show(accessoire.ToString(false));
        }

        private void dgvBotenSelected_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Boot boot = (Boot)dgvBotenSelected.SelectedRows[0].Tag;
            MessageBox.Show(boot.ToString());
        }

        private void dgvArtikelenSelected_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Accessoire accessoire = (Accessoire)dgvArtikelenSelected.SelectedRows[0].Tag;
            MessageBox.Show(accessoire.ToString(true));
        }

        private void btnToonDetailsHuurcontract_Click(object sender, EventArgs e)
        {
            GetHuurContractDetails();
        }

        private void dgvTotaalContracten_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            GetHuurContractDetails();
        }

        private void GetHuurContractDetails()
        {
            int hcID = ((Huurcontract)dgvTotaalContracten.SelectedRows[0].Tag).ID;
            foreach (Artikel a in Administratie.GetHuurContractDetails(hcID))
            {
                if (a is Boot)
                {
                    Boot boot = a as Boot;
                    int rowId = dgvBotenSelected.Rows.Add();
                    DataGridViewRow row = dgvBotenSelected.Rows[rowId];
                    row.Cells["colBotenSelectedNaam"].Value = boot.Naam;
                    row.Cells["colBotenSelectedPrijs"].Value = boot.Prijs;
                    row.Tag = boot;
                }
                else if (a is Accessoire)
                {
                    Accessoire accessoire = a as Accessoire;
                    int rowId = dgvArtikelenSelected.Rows.Add();
                    DataGridViewRow row = dgvArtikelenSelected.Rows[rowId];
                    row.Cells["colArtikelSelectedNaam"].Value = accessoire.Naam;
                    row.Cells["colArtikelSelectedPrijs"].Value = accessoire.Prijs;
                    row.Tag = accessoire;
                }
            }
        }

        private void btnExportContract_Click(object sender, EventArgs e)
        {
            Administratie.ExportToText((Huurcontract)dgvTotaalContracten.SelectedRows[0].Tag);
        }

        private void btnContractBereken_Click(object sender, EventArgs e)
        {
            Administratie.contract.Begin = dtpBegin.Value;
            Administratie.contract.Einde = dtpEind.Value;
            numContractMeren.Value = Administratie.BerekenMeren(Convert.ToInt32(numContractBudget.Value));
        }
    }
}

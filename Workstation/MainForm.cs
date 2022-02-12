using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workstation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeLVs();
            CheckButtons();
        }

        private void CheckButtons()
        {
            отправитьToolStripMenuItem.Enabled = lvSuggestions.Items.Count > 0;
            bEditS.Enabled = bDeleteS.Enabled = lvSuggestions.SelectedItems.Count > 0;
            bEditM.Enabled = bDeleteM.Enabled = lvMain.SelectedItems.Count > 0;
        }

        private void InitializeLVs()
        {


            //resize
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeLVs();
            CheckButtons();
        }

        private void отправитьToolStripMenuItem_Click(object sender, EventArgs e)
        {


            CheckButtons();
        }

        private void bAddS_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord("");

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;



            CheckButtons();
        }

        private void bEditS_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord(lvSuggestions.SelectedItems[0].Text);

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void bDeleteS_Click(object sender, EventArgs e)
        {
            CheckButtons();
        }

        private void bAddM_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord("");

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;


            CheckButtons();
        }

        private void bEditM_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord(lvMain.SelectedItems[0].Text);

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void bDeleteM_Click(object sender, EventArgs e)
        {
            CheckButtons();
        }

        private void lvSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckButtons();
        }

        private void lvMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckButtons();
        }
    }
}

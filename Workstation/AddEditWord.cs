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
    public partial class AddEditWord : Form
    {
        public string NewWord { get; set; }

        public AddEditWord(string word)
        {
            InitializeComponent();
            tbWord.Text = word;
            NewWord = word;
            CheckButtons();
        }

        private void CheckButtons()
        {
            bOk.Enabled = tbWord.Text != "";
        }

        private void tbWord_TextChanged(object sender, EventArgs e)
        {
            CheckButtons();
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            NewWord = tbWord.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

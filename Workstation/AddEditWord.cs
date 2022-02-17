using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workstation
{
    public partial class AddEditWord : Form
    {
        public string NewWord { get; set; }

        List<string> existingWords; 

        public AddEditWord(string word, List<string> existing)
        {
            InitializeComponent();
            tbWord.Text = word;
            NewWord = word;
            CheckButtons();
            tbWord.Select();

            if (existing != null)
                existingWords = existing;
        }

        public static string DeleteExtraSpaces(string str)
        {
            return Regex.Replace(DeleteBorderSpaces(str), "\\s+", " ");
        }

        public static string DeleteBorderSpaces(string str)
        {
            if (str is null)
                return "";
            return Regex.Replace(Regex.Replace(str, "^\\s+", ""), "\\s+$", "");
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
            string new_word = DeleteExtraSpaces(tbWord.Text.ToLower());

            if (existingWords != null && existingWords.Contains(new_word))
            {
                MessageBox.Show("Слово уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NewWord = new_word;
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

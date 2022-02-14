using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workstation.WordServiceReference;

namespace Workstation
{
    public partial class MainForm : Form
    {
        WordServiceClient client;

        public MainForm()
        {
            InitializeComponent();

            client = new WordServiceClient();

            InitializeLVs();
            CheckButtons();
        }

        private void CheckButtons()
        {
            отправитьToolStripMenuItem.Enabled = lvSuggestions.Items.Count > 0;
            bEditS.Enabled = bDeleteS.Enabled = lvSuggestions.SelectedItems.Count > 0;
            bEditM.Enabled = bDeleteM.Enabled = lvMain.SelectedItems.Count > 0;
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

        private void InitializeLV(List<Word> words, ListView lv)
        {
            if (words is null)
                return;

            lv.Items.Clear();

            foreach (Word w in words)
            {
                var lvi = new ListViewItem(w.Value);
                lvi.Tag = w.Id;
                lv.Items.Add(lvi);
            }

            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        List<Word> GetWords(bool isMain)
        {

            List<Word> w = new List<Word>();
            int count = client.GetWordsCount(isMain);
            for (int i = 0; i < count; i++)
                w.Add(client.GetWord(i));
            return w;
        }

        private void InitializeLVs()
        { 
            try
            {
                InitializeLV(GetWords(true), lvMain);
                InitializeLV(GetWords(false), lvSuggestions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
}

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeLVs();
            CheckButtons();
        }

        private void отправитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lvSuggestions.Items.Count; i++)
                {
                    var lvi = lvSuggestions.Items[i];
                    client.MoveFromSuggestions((int)lvi.Tag);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            InitializeLVs();

            CheckButtons();
        }

        private void bAddS_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord("", null);

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                var lvi = new ListViewItem(aew.NewWord);
                lvi.Tag = client.InsertWord(aew.NewWord, false);
                lvSuggestions.Items.Add(lvi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            CheckButtons();
        }

        private void bEditS_Click(object sender, EventArgs e)
        {
            var lvi = lvSuggestions.SelectedItems[0];

            AddEditWord aew = new AddEditWord(lvi.Text, null);

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                client.UpdateWord((int)lvi.Tag, aew.NewWord, false);
                lvi.Text = aew.NewWord;
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            CheckButtons();
        }

        private void bDeleteS_Click(object sender, EventArgs e)
        {
            var lvi = lvSuggestions.SelectedItems[0];

            try
            {
                client.DeleteWord((int)lvi.Tag, false);
                lvSuggestions.Items.Remove(lvi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            CheckButtons();
        }

        private void bAddM_Click(object sender, EventArgs e)
        {
            AddEditWord aew = new AddEditWord("", lvMain.Items.Cast<ListViewItem>().Select(x => x.Text).ToList());

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                var lvi = new ListViewItem(aew.NewWord);
                lvi.Tag = client.InsertWord(aew.NewWord, true);
                lvMain.Items.Add(lvi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            CheckButtons();
        }

        private void bEditM_Click(object sender, EventArgs e)
        {
            var lvi = lvMain.SelectedItems[0];

            AddEditWord aew = new AddEditWord(lvi.Text, lvMain.Items.Cast<ListViewItem>().Select(x => x.Text).ToList());

            if (aew.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                client.UpdateWord((int)lvi.Tag, aew.NewWord, true);
                lvi.Text = aew.NewWord;
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            CheckButtons();
        }

        private void bDeleteM_Click(object sender, EventArgs e)
        {
            var lvi = lvMain.SelectedItems[0];

            try
            {
                client.DeleteWord((int)lvi.Tag, true);
                lvMain.Items.Remove(lvi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }

        private void рандомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(client.GetRandomWord(), "рандом");
            }
            catch (Exception ex)
            {
                MessageBox.Show("База данных словарей временно недоступна. Повторите попытку позже", "База данных словарей недоступна", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

        }
    }
}

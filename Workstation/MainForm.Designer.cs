
namespace Workstation
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.обновитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.отправитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvSuggestions = new System.Windows.Forms.ListView();
            this.bAddS = new System.Windows.Forms.Button();
            this.bEditS = new System.Windows.Forms.Button();
            this.bDeleteS = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bDeleteM = new System.Windows.Forms.Button();
            this.bEditM = new System.Windows.Forms.Button();
            this.bAddM = new System.Windows.Forms.Button();
            this.lvMain = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bDeleteS);
            this.groupBox1.Controls.Add(this.bEditS);
            this.groupBox1.Controls.Add(this.bAddS);
            this.groupBox1.Controls.Add(this.lvSuggestions);
            this.groupBox1.Location = new System.Drawing.Point(10, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 277);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Предложенные слова";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.обновитьToolStripMenuItem,
            this.отправитьToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(770, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // обновитьToolStripMenuItem
            // 
            this.обновитьToolStripMenuItem.Name = "обновитьToolStripMenuItem";
            this.обновитьToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.обновитьToolStripMenuItem.Text = "Обновить";
            this.обновитьToolStripMenuItem.Click += new System.EventHandler(this.обновитьToolStripMenuItem_Click);
            // 
            // отправитьToolStripMenuItem
            // 
            this.отправитьToolStripMenuItem.Name = "отправитьToolStripMenuItem";
            this.отправитьToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.отправитьToolStripMenuItem.Text = "Отправить";
            this.отправитьToolStripMenuItem.Click += new System.EventHandler(this.отправитьToolStripMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bDeleteM);
            this.groupBox2.Controls.Add(this.bEditM);
            this.groupBox2.Controls.Add(this.lvMain);
            this.groupBox2.Controls.Add(this.bAddM);
            this.groupBox2.Location = new System.Drawing.Point(387, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(371, 277);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Основной словарь";
            // 
            // lvSuggestions
            // 
            this.lvSuggestions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvSuggestions.FullRowSelect = true;
            this.lvSuggestions.GridLines = true;
            this.lvSuggestions.HideSelection = false;
            this.lvSuggestions.Location = new System.Drawing.Point(6, 46);
            this.lvSuggestions.MultiSelect = false;
            this.lvSuggestions.Name = "lvSuggestions";
            this.lvSuggestions.Size = new System.Drawing.Size(359, 225);
            this.lvSuggestions.TabIndex = 0;
            this.lvSuggestions.UseCompatibleStateImageBehavior = false;
            this.lvSuggestions.View = System.Windows.Forms.View.Details;
            this.lvSuggestions.SelectedIndexChanged += new System.EventHandler(this.lvSuggestions_SelectedIndexChanged);
            // 
            // bAddS
            // 
            this.bAddS.Location = new System.Drawing.Point(6, 19);
            this.bAddS.Name = "bAddS";
            this.bAddS.Size = new System.Drawing.Size(113, 23);
            this.bAddS.TabIndex = 1;
            this.bAddS.Text = "Добавить";
            this.bAddS.UseVisualStyleBackColor = true;
            this.bAddS.Click += new System.EventHandler(this.bAddS_Click);
            // 
            // bEditS
            // 
            this.bEditS.Location = new System.Drawing.Point(129, 19);
            this.bEditS.Name = "bEditS";
            this.bEditS.Size = new System.Drawing.Size(113, 23);
            this.bEditS.TabIndex = 2;
            this.bEditS.Text = "Изменить";
            this.bEditS.UseVisualStyleBackColor = true;
            this.bEditS.Click += new System.EventHandler(this.bEditS_Click);
            // 
            // bDeleteS
            // 
            this.bDeleteS.Location = new System.Drawing.Point(252, 19);
            this.bDeleteS.Name = "bDeleteS";
            this.bDeleteS.Size = new System.Drawing.Size(113, 23);
            this.bDeleteS.TabIndex = 3;
            this.bDeleteS.Text = "Удалить";
            this.bDeleteS.UseVisualStyleBackColor = true;
            this.bDeleteS.Click += new System.EventHandler(this.bDeleteS_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Слово";
            // 
            // bDeleteM
            // 
            this.bDeleteM.Location = new System.Drawing.Point(252, 19);
            this.bDeleteM.Name = "bDeleteM";
            this.bDeleteM.Size = new System.Drawing.Size(113, 23);
            this.bDeleteM.TabIndex = 7;
            this.bDeleteM.Text = "Удалить";
            this.bDeleteM.UseVisualStyleBackColor = true;
            this.bDeleteM.Click += new System.EventHandler(this.bDeleteM_Click);
            // 
            // bEditM
            // 
            this.bEditM.Location = new System.Drawing.Point(129, 19);
            this.bEditM.Name = "bEditM";
            this.bEditM.Size = new System.Drawing.Size(113, 23);
            this.bEditM.TabIndex = 6;
            this.bEditM.Text = "Изменить";
            this.bEditM.UseVisualStyleBackColor = true;
            this.bEditM.Click += new System.EventHandler(this.bEditM_Click);
            // 
            // bAddM
            // 
            this.bAddM.Location = new System.Drawing.Point(6, 19);
            this.bAddM.Name = "bAddM";
            this.bAddM.Size = new System.Drawing.Size(113, 23);
            this.bAddM.TabIndex = 5;
            this.bAddM.Text = "Добавить";
            this.bAddM.UseVisualStyleBackColor = true;
            this.bAddM.Click += new System.EventHandler(this.bAddM_Click);
            // 
            // lvMain
            // 
            this.lvMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvMain.FullRowSelect = true;
            this.lvMain.GridLines = true;
            this.lvMain.HideSelection = false;
            this.lvMain.Location = new System.Drawing.Point(6, 46);
            this.lvMain.MultiSelect = false;
            this.lvMain.Name = "lvMain";
            this.lvMain.Size = new System.Drawing.Size(359, 225);
            this.lvMain.TabIndex = 4;
            this.lvMain.UseCompatibleStateImageBehavior = false;
            this.lvMain.View = System.Windows.Forms.View.Details;
            this.lvMain.SelectedIndexChanged += new System.EventHandler(this.lvMain_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Слово";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 316);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Workstation";
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem обновитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem отправитьToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bDeleteS;
        private System.Windows.Forms.Button bEditS;
        private System.Windows.Forms.Button bAddS;
        private System.Windows.Forms.ListView lvSuggestions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button bDeleteM;
        private System.Windows.Forms.Button bEditM;
        private System.Windows.Forms.ListView lvMain;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button bAddM;
    }
}


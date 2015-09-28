namespace MapsEditor
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.grbScale = new System.Windows.Forms.GroupBox();
            this.btnScale = new System.Windows.Forms.Button();
            this.tbScale = new System.Windows.Forms.TextBox();
            this.grbTransparency = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarTransparency = new System.Windows.Forms.TrackBar();
            this.chbAllowTransparency = new System.Windows.Forms.CheckBox();
            this.gbGrid = new System.Windows.Forms.GroupBox();
            this.pnlGridCellSize = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbGridCellSize = new System.Windows.Forms.TextBox();
            this.chbShowGrid = new System.Windows.Forms.CheckBox();
            this.gbSolid = new System.Windows.Forms.GroupBox();
            this.btnDeleteSolid = new System.Windows.Forms.Button();
            this.btnCreateSolid = new System.Windows.Forms.Button();
            this.btnSaveSolid = new System.Windows.Forms.Button();
            this.grbEdit = new System.Windows.Forms.GroupBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAngle = new System.Windows.Forms.TextBox();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPosY = new System.Windows.Forms.TextBox();
            this.tbPosX = new System.Windows.Forms.TextBox();
            this.grbSolids = new System.Windows.Forms.GroupBox();
            this.lbSolids = new System.Windows.Forms.ListBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cbObjectType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.grbScale.SuspendLayout();
            this.grbTransparency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTransparency)).BeginInit();
            this.gbGrid.SuspendLayout();
            this.pnlGridCellSize.SuspendLayout();
            this.gbSolid.SuspendLayout();
            this.grbEdit.SuspendLayout();
            this.grbSolids.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 588);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(731, 588);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.grbScale);
            this.flowLayoutPanel1.Controls.Add(this.grbTransparency);
            this.flowLayoutPanel1.Controls.Add(this.gbGrid);
            this.flowLayoutPanel1.Controls.Add(this.gbSolid);
            this.flowLayoutPanel1.Controls.Add(this.grbEdit);
            this.flowLayoutPanel1.Controls.Add(this.grbSolids);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(263, 588);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // grbScale
            // 
            this.grbScale.Controls.Add(this.btnScale);
            this.grbScale.Controls.Add(this.tbScale);
            this.grbScale.Location = new System.Drawing.Point(3, 3);
            this.grbScale.Name = "grbScale";
            this.grbScale.Size = new System.Drawing.Size(254, 50);
            this.grbScale.TabIndex = 11;
            this.grbScale.TabStop = false;
            this.grbScale.Text = "Масштаб";
            // 
            // btnScale
            // 
            this.btnScale.Location = new System.Drawing.Point(115, 16);
            this.btnScale.Name = "btnScale";
            this.btnScale.Size = new System.Drawing.Size(75, 23);
            this.btnScale.TabIndex = 1;
            this.btnScale.Text = "Применить";
            this.btnScale.UseVisualStyleBackColor = true;
            this.btnScale.Click += new System.EventHandler(this.btnScale_Click);
            // 
            // tbScale
            // 
            this.tbScale.Location = new System.Drawing.Point(9, 19);
            this.tbScale.Name = "tbScale";
            this.tbScale.Size = new System.Drawing.Size(100, 20);
            this.tbScale.TabIndex = 0;
            // 
            // grbTransparency
            // 
            this.grbTransparency.Controls.Add(this.label1);
            this.grbTransparency.Controls.Add(this.trackBarTransparency);
            this.grbTransparency.Controls.Add(this.chbAllowTransparency);
            this.grbTransparency.Location = new System.Drawing.Point(3, 59);
            this.grbTransparency.Name = "grbTransparency";
            this.grbTransparency.Size = new System.Drawing.Size(254, 109);
            this.grbTransparency.TabIndex = 9;
            this.grbTransparency.TabStop = false;
            this.grbTransparency.Text = "Прозрачность";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Прозрачность формы";
            // 
            // trackBarTransparency
            // 
            this.trackBarTransparency.Location = new System.Drawing.Point(11, 32);
            this.trackBarTransparency.Maximum = 60;
            this.trackBarTransparency.Name = "trackBarTransparency";
            this.trackBarTransparency.Size = new System.Drawing.Size(104, 45);
            this.trackBarTransparency.TabIndex = 5;
            this.trackBarTransparency.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTransparency.Scroll += new System.EventHandler(this.trackBarTransparency_Scroll);
            // 
            // chbAllowTransparency
            // 
            this.chbAllowTransparency.AutoSize = true;
            this.chbAllowTransparency.Location = new System.Drawing.Point(6, 83);
            this.chbAllowTransparency.Name = "chbAllowTransparency";
            this.chbAllowTransparency.Size = new System.Drawing.Size(148, 17);
            this.chbAllowTransparency.TabIndex = 0;
            this.chbAllowTransparency.Text = "Включить прозрачность";
            this.chbAllowTransparency.UseVisualStyleBackColor = true;
            this.chbAllowTransparency.CheckedChanged += new System.EventHandler(this.chbAllowTransparency_CheckedChanged);
            // 
            // gbGrid
            // 
            this.gbGrid.Controls.Add(this.pnlGridCellSize);
            this.gbGrid.Controls.Add(this.chbShowGrid);
            this.gbGrid.Location = new System.Drawing.Point(3, 174);
            this.gbGrid.Name = "gbGrid";
            this.gbGrid.Size = new System.Drawing.Size(254, 66);
            this.gbGrid.TabIndex = 12;
            this.gbGrid.TabStop = false;
            this.gbGrid.Text = "Сетка";
            // 
            // pnlGridCellSize
            // 
            this.pnlGridCellSize.Controls.Add(this.label6);
            this.pnlGridCellSize.Controls.Add(this.tbGridCellSize);
            this.pnlGridCellSize.Location = new System.Drawing.Point(121, 13);
            this.pnlGridCellSize.Name = "pnlGridCellSize";
            this.pnlGridCellSize.Size = new System.Drawing.Size(130, 47);
            this.pnlGridCellSize.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Размер";
            // 
            // tbGridCellSize
            // 
            this.tbGridCellSize.Location = new System.Drawing.Point(10, 20);
            this.tbGridCellSize.Name = "tbGridCellSize";
            this.tbGridCellSize.Size = new System.Drawing.Size(100, 20);
            this.tbGridCellSize.TabIndex = 3;
            this.tbGridCellSize.TextChanged += new System.EventHandler(this.tbGridCellSize_TextChanged);
            // 
            // chbShowGrid
            // 
            this.chbShowGrid.AutoSize = true;
            this.chbShowGrid.Location = new System.Drawing.Point(9, 19);
            this.chbShowGrid.Name = "chbShowGrid";
            this.chbShowGrid.Size = new System.Drawing.Size(106, 17);
            this.chbShowGrid.TabIndex = 1;
            this.chbShowGrid.Text = "Включить сетку";
            this.chbShowGrid.UseVisualStyleBackColor = true;
            this.chbShowGrid.CheckedChanged += new System.EventHandler(this.chbShowGrid_CheckedChanged);
            // 
            // gbSolid
            // 
            this.gbSolid.Controls.Add(this.cbObjectType);
            this.gbSolid.Controls.Add(this.btnDeleteSolid);
            this.gbSolid.Controls.Add(this.btnCreateSolid);
            this.gbSolid.Controls.Add(this.btnSaveSolid);
            this.gbSolid.Location = new System.Drawing.Point(3, 246);
            this.gbSolid.Name = "gbSolid";
            this.gbSolid.Size = new System.Drawing.Size(254, 78);
            this.gbSolid.TabIndex = 3;
            this.gbSolid.TabStop = false;
            this.gbSolid.Text = "Объект";
            // 
            // btnDeleteSolid
            // 
            this.btnDeleteSolid.Enabled = false;
            this.btnDeleteSolid.Location = new System.Drawing.Point(90, 46);
            this.btnDeleteSolid.Name = "btnDeleteSolid";
            this.btnDeleteSolid.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteSolid.TabIndex = 3;
            this.btnDeleteSolid.Text = "Удалить";
            this.btnDeleteSolid.UseVisualStyleBackColor = true;
            this.btnDeleteSolid.Click += new System.EventHandler(this.btnDeleteSolid_Click);
            // 
            // btnCreateSolid
            // 
            this.btnCreateSolid.Location = new System.Drawing.Point(140, 19);
            this.btnCreateSolid.Name = "btnCreateSolid";
            this.btnCreateSolid.Size = new System.Drawing.Size(75, 23);
            this.btnCreateSolid.TabIndex = 1;
            this.btnCreateSolid.Text = "Создать";
            this.btnCreateSolid.UseVisualStyleBackColor = true;
            this.btnCreateSolid.Click += new System.EventHandler(this.btnCreateSolid_Click);
            // 
            // btnSaveSolid
            // 
            this.btnSaveSolid.Enabled = false;
            this.btnSaveSolid.Location = new System.Drawing.Point(9, 46);
            this.btnSaveSolid.Name = "btnSaveSolid";
            this.btnSaveSolid.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSolid.TabIndex = 2;
            this.btnSaveSolid.Text = "Сохранить";
            this.btnSaveSolid.UseVisualStyleBackColor = true;
            this.btnSaveSolid.Click += new System.EventHandler(this.btnSaveSolid_Click);
            // 
            // grbEdit
            // 
            this.grbEdit.Controls.Add(this.btnApply);
            this.grbEdit.Controls.Add(this.label2);
            this.grbEdit.Controls.Add(this.label5);
            this.grbEdit.Controls.Add(this.tbAngle);
            this.grbEdit.Controls.Add(this.tbHeight);
            this.grbEdit.Controls.Add(this.label4);
            this.grbEdit.Controls.Add(this.tbWidth);
            this.grbEdit.Controls.Add(this.label3);
            this.grbEdit.Controls.Add(this.tbPosY);
            this.grbEdit.Controls.Add(this.tbPosX);
            this.grbEdit.Location = new System.Drawing.Point(3, 330);
            this.grbEdit.Name = "grbEdit";
            this.grbEdit.Size = new System.Drawing.Size(254, 165);
            this.grbEdit.TabIndex = 7;
            this.grbEdit.TabStop = false;
            this.grbEdit.Text = "Редактирование";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(9, 136);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Применить";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "градусов";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Угол поворота";
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(9, 110);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(100, 20);
            this.tbAngle.TabIndex = 0;
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(115, 71);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(100, 20);
            this.tbHeight.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Размеры";
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(9, 71);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(100, 20);
            this.tbWidth.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Позиция";
            // 
            // tbPosY
            // 
            this.tbPosY.Location = new System.Drawing.Point(115, 32);
            this.tbPosY.Name = "tbPosY";
            this.tbPosY.Size = new System.Drawing.Size(100, 20);
            this.tbPosY.TabIndex = 1;
            // 
            // tbPosX
            // 
            this.tbPosX.Location = new System.Drawing.Point(9, 32);
            this.tbPosX.Name = "tbPosX";
            this.tbPosX.Size = new System.Drawing.Size(100, 20);
            this.tbPosX.TabIndex = 0;
            // 
            // grbSolids
            // 
            this.grbSolids.Controls.Add(this.lbSolids);
            this.grbSolids.Location = new System.Drawing.Point(3, 501);
            this.grbSolids.Name = "grbSolids";
            this.grbSolids.Size = new System.Drawing.Size(254, 80);
            this.grbSolids.TabIndex = 10;
            this.grbSolids.TabStop = false;
            this.grbSolids.Text = "Объекты";
            // 
            // lbSolids
            // 
            this.lbSolids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSolids.FormattingEnabled = true;
            this.lbSolids.Location = new System.Drawing.Point(3, 16);
            this.lbSolids.Name = "lbSolids";
            this.lbSolids.Size = new System.Drawing.Size(248, 61);
            this.lbSolids.TabIndex = 0;
            this.lbSolids.SelectedIndexChanged += new System.EventHandler(this.lbSolids_SelectedIndexChanged);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sam";
            this.saveFileDialog1.Filter = "StillAlive Map|*.sam|Все файлы|*.*";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpen,
            this.tsmiSave,
            this.tsmiSaveAs});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.Size = new System.Drawing.Size(153, 22);
            this.tsmiOpen.Text = "Открыть";
            this.tsmiOpen.Click += new System.EventHandler(this.tsmiOpen_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(153, 22);
            this.tsmiSave.Text = "Сохранить";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.Size = new System.Drawing.Size(153, 22);
            this.tsmiSaveAs.Text = "Сохранить как";
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "sam";
            this.openFileDialog1.Filter = "StillAlive Map|*.sam|Все файлы|*.*";
            // 
            // cbObjectType
            // 
            this.cbObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbObjectType.FormattingEnabled = true;
            this.cbObjectType.Location = new System.Drawing.Point(9, 19);
            this.cbObjectType.Name = "cbObjectType";
            this.cbObjectType.Size = new System.Drawing.Size(121, 21);
            this.cbObjectType.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 612);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Редактор карт";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.grbScale.ResumeLayout(false);
            this.grbScale.PerformLayout();
            this.grbTransparency.ResumeLayout(false);
            this.grbTransparency.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTransparency)).EndInit();
            this.gbGrid.ResumeLayout(false);
            this.gbGrid.PerformLayout();
            this.pnlGridCellSize.ResumeLayout(false);
            this.pnlGridCellSize.PerformLayout();
            this.gbSolid.ResumeLayout(false);
            this.grbEdit.ResumeLayout(false);
            this.grbEdit.PerformLayout();
            this.grbSolids.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbSolid;
        private System.Windows.Forms.Button btnCreateSolid;
        private System.Windows.Forms.Button btnSaveSolid;
        private System.Windows.Forms.Button btnDeleteSolid;
        private System.Windows.Forms.GroupBox grbEdit;
        private System.Windows.Forms.TextBox tbPosY;
        private System.Windows.Forms.TextBox tbPosX;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAngle;
        private System.Windows.Forms.GroupBox grbSolids;
        private System.Windows.Forms.ListBox lbSolids;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox grbScale;
        private System.Windows.Forms.Button btnScale;
        private System.Windows.Forms.TextBox tbScale;
        private System.Windows.Forms.GroupBox grbTransparency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarTransparency;
        private System.Windows.Forms.CheckBox chbAllowTransparency;
        private System.Windows.Forms.GroupBox gbGrid;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbGridCellSize;
        private System.Windows.Forms.CheckBox chbShowGrid;
        private System.Windows.Forms.Panel pnlGridCellSize;
        private System.Windows.Forms.ComboBox cbObjectType;
    }
}


namespace WinForm.UserControls
{
    partial class ucDesktop
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tlpMain = new TableLayoutPanel();
            breadcrumbBar1 = new WinForm.Controls.BreadcrumbBar.BreadcrumbBar();
            customTab1 = new VideoWall.WinForms.UserControls.CustomeControls.CustomeTabControl.CustomTab();
            txtSearchTiles = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            tlpMain.SuspendLayout();
            SuspendLayout();
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(breadcrumbBar1, 0, 0);
            tlpMain.Controls.Add(customTab1, 0, 1);
            tlpMain.Controls.Add(txtSearchTiles, 0, 2);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 0);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 5;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpMain.Size = new Size(838, 625);
            tlpMain.TabIndex = 2;
            // 
            // breadcrumbBar1
            // 
            breadcrumbBar1.BackColor = Color.Transparent;
            breadcrumbBar1.Dock = DockStyle.Fill;
            breadcrumbBar1.Font = new Font("Segoe UI", 10F);
            breadcrumbBar1.ForeColor = Color.Black;
            breadcrumbBar1.Location = new Point(3, 3);
            breadcrumbBar1.Name = "breadcrumbBar1";
            breadcrumbBar1.Padding = new Padding(10, 6, 10, 6);
            breadcrumbBar1.RightToLeft = RightToLeft.Yes;
            breadcrumbBar1.Size = new Size(832, 39);
            breadcrumbBar1.TabIndex = 3;
            breadcrumbBar1.Text = "breadcrumbBar1";
            // 
            // customTab1
            // 
            customTab1.BackColor = Color.Transparent;
            customTab1.Dock = DockStyle.Fill;
            customTab1.Location = new Point(3, 48);
            customTab1.MinimumSize = new Size(150, 40);
            customTab1.Name = "customTab1";
            customTab1.Size = new Size(832, 40);
            customTab1.TabIndex = 7;
            // 
            // txtSearchTiles
            // 
            txtSearchTiles.BackColor = SystemColors.Window;
            txtSearchTiles.Dock = DockStyle.Fill;
            txtSearchTiles.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtSearchTiles.Location = new Point(3, 93);
            txtSearchTiles.Name = "txtSearchTiles";
            txtSearchTiles.Padding = new Padding(10, 7, 10, 7);
            txtSearchTiles.PlaceholderText = "عنوان مورد نظر بنویسید";
            txtSearchTiles.RightToLeft = RightToLeft.Yes;
            txtSearchTiles.Size = new Size(832, 34);
            txtSearchTiles.TabIndex = 8;
            txtSearchTiles.Text = "";
            // 
            // ucDesktop
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlpMain);
            Name = "ucDesktop";
            Size = new Size(838, 625);
            tlpMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpMain;
        private Controls.BreadcrumbBar.BreadcrumbBar breadcrumbBar1;
        private VideoWall.WinForms.UserControls.CustomeControls.CustomeTabControl.CustomTab customTab1;
        private Controls.RoundedTextBox.RoundedTextBox txtSearchTiles;
    }
}

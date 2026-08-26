namespace WinForm.Forms
{
    partial class Dashboard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dashboardSidebar1 = new WinForm.Controls.DashboardMenu.DashboardSidebar();
            panelLeft = new Panel();
            SuspendLayout();
            // 
            // dashboardSidebar1
            // 
            dashboardSidebar1.BackColor = Color.FromArgb(28, 39, 70);
            dashboardSidebar1.Dock = DockStyle.Right;
            dashboardSidebar1.FooterText = "تولید شده مساجد";
            dashboardSidebar1.Location = new Point(617, 0);
            dashboardSidebar1.Logo = Properties.Resources.TopLogo;
            dashboardSidebar1.MinimumSize = new Size(72, 250);
            dashboardSidebar1.Name = "dashboardSidebar1";
            dashboardSidebar1.RightToLeft = RightToLeft.Yes;
            dashboardSidebar1.Size = new Size(280, 606);
            dashboardSidebar1.TabIndex = 0;
            dashboardSidebar1.Theme.BackgroundColor = Color.FromArgb(28, 39, 70);
            dashboardSidebar1.Theme.GroupFont = new Font("Segoe UI", 8.5F);
            dashboardSidebar1.Theme.GroupTitleColor = Color.FromArgb(150, 162, 190);
            dashboardSidebar1.Theme.ItemFont = new Font("Segoe UI", 10F);
            dashboardSidebar1.Theme.ItemIconColor = Color.FromArgb(150, 162, 190);
            dashboardSidebar1.Theme.ItemTextColor = Color.FromArgb(169, 181, 209);
            dashboardSidebar1.Theme.SelectedItemBackgroundColor = Color.FromArgb(35, 53, 92);
            dashboardSidebar1.Theme.SelectedItemIconColor = Color.FromArgb(48, 157, 255);
            dashboardSidebar1.Theme.SelectedItemTextColor = Color.FromArgb(48, 157, 255);
            dashboardSidebar1.Theme.SeparatorColor = Color.FromArgb(53, 68, 104);
            dashboardSidebar1.Theme.UserNameColor = Color.White;
            dashboardSidebar1.Theme.UserNameFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            dashboardSidebar1.Theme.UserRoleColor = Color.FromArgb(152, 164, 192);
            dashboardSidebar1.Theme.UserRoleFont = new Font("Segoe UI", 9F);
            dashboardSidebar1.UserName = "Mojtaba";
            dashboardSidebar1.UserRole = "مدیر داخلی";
            // 
            // panelLeft
            // 
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(617, 606);
            panelLeft.TabIndex = 1;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(897, 606);
            Controls.Add(panelLeft);
            Controls.Add(dashboardSidebar1);
            Name = "Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
        }

        #endregion

        private Controls.DashboardMenu.DashboardSidebar dashboardSidebar1;
        private Panel panelLeft;
    }
}
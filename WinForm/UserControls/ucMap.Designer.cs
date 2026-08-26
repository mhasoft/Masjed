namespace WinForm.UserControls
{
    partial class ucMap
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
            flpTop = new FlowLayoutPanel();
            panelSearchBox = new WinForm.Controls.ModernRoundedPanel.ModernRoundedPanel();
            cmbPlaceType = new ComboBox();
            lblSearchPlace = new Label();
            txtSearchLocation = new WinForm.Controls.ModernRoundedSearchBox.ModernRoundedSearchBox();
            btnSaveAll = new WinForm.Controls.ModernButton.ModernButton();
            btnAddNewArea = new WinForm.Controls.ModernButton.ModernButton();
            btnPickLocation = new WinForm.Controls.ModernButton.ModernButton();
            panelShowNames = new WinForm.Controls.ModernRoundedPanel.ModernRoundedPanel();
            chbShowMakan = new CheckBox();
            chbShowMasjed = new CheckBox();
            chbShowMahale = new CheckBox();
            chbShowNames = new CheckBox();
            tlpMapAndFields = new TableLayoutPanel();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            panelFields = new Panel();
            tabControlFields = new TabControl();
            tabPageMahale = new TabPage();
            panelTabPageMahale = new Panel();
            txtMahaleName = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            lblMahaleName = new Label();
            lblMahaleId = new Label();
            txtMahaleId = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            tabPageMasjed = new TabPage();
            panelTabPageMasjed = new Panel();
            txtMasjedId = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            lblMasjedPhoneNumber = new Label();
            lblMasjedLocation = new Label();
            dgvMasjedPhoneNumber = new DataGridView();
            Masjed_Tel_Title = new DataGridViewTextBoxColumn();
            Masjed_Tel_Value = new DataGridViewTextBoxColumn();
            lblMasjedId = new Label();
            dgvMasjedAddress = new DataGridView();
            Masjed_Address_Title = new DataGridViewTextBoxColumn();
            Masjed_Address_Value = new DataGridViewTextBoxColumn();
            txtMasjedName = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            lblMasjedName = new Label();
            tabPageAmaken = new TabPage();
            panelTabPageAmaken = new Panel();
            txtAmakenId = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            label5 = new Label();
            lblAmakenLocation = new Label();
            dgvMakanPhoneNumber = new DataGridView();
            lblManakId = new Label();
            dgvMakanAddress = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            txtAmakenName = new WinForm.Controls.RoundedTextBox.RoundedTextBox();
            lblMakanName = new Label();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            tlpMain.SuspendLayout();
            flpTop.SuspendLayout();
            panelSearchBox.SuspendLayout();
            panelShowNames.SuspendLayout();
            tlpMapAndFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            panelFields.SuspendLayout();
            tabControlFields.SuspendLayout();
            tabPageMahale.SuspendLayout();
            panelTabPageMahale.SuspendLayout();
            tabPageMasjed.SuspendLayout();
            panelTabPageMasjed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMasjedPhoneNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMasjedAddress).BeginInit();
            tabPageAmaken.SuspendLayout();
            panelTabPageAmaken.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMakanPhoneNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMakanAddress).BeginInit();
            SuspendLayout();
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(flpTop, 0, 0);
            tlpMain.Controls.Add(tlpMapAndFields, 0, 1);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 0);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 2;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpMain.Size = new Size(1182, 668);
            tlpMain.TabIndex = 1;
            // 
            // flpTop
            // 
            flpTop.AutoScroll = true;
            flpTop.Controls.Add(panelSearchBox);
            flpTop.Controls.Add(btnSaveAll);
            flpTop.Controls.Add(btnAddNewArea);
            flpTop.Controls.Add(btnPickLocation);
            flpTop.Controls.Add(panelShowNames);
            flpTop.Controls.Add(chbShowNames);
            flpTop.Dock = DockStyle.Fill;
            flpTop.Location = new Point(3, 3);
            flpTop.Name = "flpTop";
            flpTop.Size = new Size(1176, 72);
            flpTop.TabIndex = 4;
            // 
            // panelSearchBox
            // 
            panelSearchBox.Controls.Add(cmbPlaceType);
            panelSearchBox.Controls.Add(lblSearchPlace);
            panelSearchBox.Controls.Add(txtSearchLocation);
            panelSearchBox.Location = new Point(3, 3);
            panelSearchBox.Name = "panelSearchBox";
            panelSearchBox.Padding = new Padding(2);
            panelSearchBox.Size = new Size(276, 44);
            panelSearchBox.TabIndex = 4;
            // 
            // cmbPlaceType
            // 
            cmbPlaceType.FormattingEnabled = true;
            cmbPlaceType.Items.AddRange(new object[] { "محله", "مسجد", "اماکن" });
            cmbPlaceType.Location = new Point(151, 10);
            cmbPlaceType.Name = "cmbPlaceType";
            cmbPlaceType.Size = new Size(69, 24);
            cmbPlaceType.TabIndex = 7;
            cmbPlaceType.Text = "محله";
            // 
            // lblSearchPlace
            // 
            lblSearchPlace.AutoSize = true;
            lblSearchPlace.Font = new Font("Tahoma", 9F);
            lblSearchPlace.ForeColor = Color.SteelBlue;
            lblSearchPlace.Location = new Point(226, 14);
            lblSearchPlace.Name = "lblSearchPlace";
            lblSearchPlace.Size = new Size(45, 14);
            lblSearchPlace.TabIndex = 6;
            lblSearchPlace.Text = "جستجو";
            // 
            // txtSearchLocation
            // 
            txtSearchLocation.BackColor = SystemColors.Window;
            txtSearchLocation.Location = new Point(5, 7);
            txtSearchLocation.Name = "txtSearchLocation";
            txtSearchLocation.Padding = new Padding(10, 7, 10, 7);
            txtSearchLocation.Size = new Size(140, 30);
            txtSearchLocation.TabIndex = 5;
            // 
            // btnSaveAll
            // 
            btnSaveAll.BackColor = Color.Transparent;
            btnSaveAll.BorderRadius = 9;
            btnSaveAll.ButtonIcon = WinForm.Controls.ModernButton.FontAwesomeIcon.Download;
            btnSaveAll.Font = new Font("Segoe UI", 9F);
            btnSaveAll.IconSize = 19;
            btnSaveAll.Location = new Point(285, 3);
            btnSaveAll.Name = "btnSaveAll";
            btnSaveAll.RightToLeft = RightToLeft.Yes;
            btnSaveAll.Size = new Size(141, 44);
            btnSaveAll.TabIndex = 1;
            btnSaveAll.Text = "ذخیره تغییرات";
            // 
            // btnAddNewArea
            // 
            btnAddNewArea.BackColor = Color.Transparent;
            btnAddNewArea.BorderRadius = 9;
            btnAddNewArea.ButtonIcon = WinForm.Controls.ModernButton.FontAwesomeIcon.Plus;
            btnAddNewArea.Font = new Font("Segoe UI", 9F);
            btnAddNewArea.IconSize = 19;
            btnAddNewArea.Location = new Point(432, 3);
            btnAddNewArea.Name = "btnAddNewArea";
            btnAddNewArea.RightToLeft = RightToLeft.Yes;
            btnAddNewArea.Size = new Size(141, 44);
            btnAddNewArea.TabIndex = 0;
            btnAddNewArea.Text = "رسم محدوده جدید";
            // 
            // btnPickLocation
            // 
            btnPickLocation.BackColor = Color.Transparent;
            btnPickLocation.BorderRadius = 9;
            btnPickLocation.ButtonIcon = WinForm.Controls.ModernButton.FontAwesomeIcon.MapMarkerAlt;
            btnPickLocation.Font = new Font("Segoe UI", 9F);
            btnPickLocation.Location = new Point(579, 3);
            btnPickLocation.Name = "btnPickLocation";
            btnPickLocation.Size = new Size(180, 45);
            btnPickLocation.TabIndex = 6;
            btnPickLocation.Text = "انتخاب موقعیت";
            // 
            // panelShowNames
            // 
            panelShowNames.Controls.Add(chbShowMakan);
            panelShowNames.Controls.Add(chbShowMasjed);
            panelShowNames.Controls.Add(chbShowMahale);
            panelShowNames.Location = new Point(765, 3);
            panelShowNames.Name = "panelShowNames";
            panelShowNames.Padding = new Padding(2);
            panelShowNames.Size = new Size(206, 44);
            panelShowNames.TabIndex = 7;
            // 
            // chbShowMakan
            // 
            chbShowMakan.AutoSize = true;
            chbShowMakan.Location = new Point(145, 12);
            chbShowMakan.Name = "chbShowMakan";
            chbShowMakan.Size = new Size(54, 21);
            chbShowMakan.TabIndex = 9;
            chbShowMakan.Text = "مکان";
            chbShowMakan.UseVisualStyleBackColor = true;
            // 
            // chbShowMasjed
            // 
            chbShowMasjed.AutoSize = true;
            chbShowMasjed.Location = new Point(72, 12);
            chbShowMasjed.Name = "chbShowMasjed";
            chbShowMasjed.Size = new Size(67, 21);
            chbShowMasjed.TabIndex = 8;
            chbShowMasjed.Text = "مسجد";
            chbShowMasjed.UseVisualStyleBackColor = true;
            // 
            // chbShowMahale
            // 
            chbShowMahale.AutoSize = true;
            chbShowMahale.Checked = true;
            chbShowMahale.CheckState = CheckState.Checked;
            chbShowMahale.Location = new Point(9, 12);
            chbShowMahale.Name = "chbShowMahale";
            chbShowMahale.Size = new Size(57, 21);
            chbShowMahale.TabIndex = 7;
            chbShowMahale.Text = "محله";
            chbShowMahale.UseVisualStyleBackColor = true;
            // 
            // chbShowNames
            // 
            chbShowNames.Appearance = Appearance.Button;
            chbShowNames.Checked = true;
            chbShowNames.CheckState = CheckState.Checked;
            chbShowNames.FlatAppearance.BorderSize = 0;
            chbShowNames.Location = new Point(977, 3);
            chbShowNames.Name = "chbShowNames";
            chbShowNames.Size = new Size(102, 44);
            chbShowNames.TabIndex = 8;
            chbShowNames.Text = "نمایش عنوانها";
            chbShowNames.UseVisualStyleBackColor = true;
            // 
            // tlpMapAndFields
            // 
            tlpMapAndFields.ColumnCount = 2;
            tlpMapAndFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMapAndFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 390F));
            tlpMapAndFields.Controls.Add(webView, 0, 0);
            tlpMapAndFields.Controls.Add(panelFields, 1, 0);
            tlpMapAndFields.Dock = DockStyle.Fill;
            tlpMapAndFields.Location = new Point(3, 81);
            tlpMapAndFields.Name = "tlpMapAndFields";
            tlpMapAndFields.RowCount = 1;
            tlpMapAndFields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMapAndFields.Size = new Size(1176, 584);
            tlpMapAndFields.TabIndex = 3;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Dock = DockStyle.Fill;
            webView.Location = new Point(3, 3);
            webView.Name = "webView";
            webView.Size = new Size(780, 578);
            webView.TabIndex = 2;
            webView.ZoomFactor = 1D;
            // 
            // panelFields
            // 
            panelFields.AutoScroll = true;
            panelFields.BackColor = Color.White;
            panelFields.Controls.Add(tabControlFields);
            panelFields.Controls.Add(button3);
            panelFields.Controls.Add(button2);
            panelFields.Controls.Add(button1);
            panelFields.Dock = DockStyle.Fill;
            panelFields.Location = new Point(789, 3);
            panelFields.Name = "panelFields";
            panelFields.Padding = new Padding(2);
            panelFields.Size = new Size(384, 578);
            panelFields.TabIndex = 3;
            // 
            // tabControlFields
            // 
            tabControlFields.Appearance = TabAppearance.FlatButtons;
            tabControlFields.Controls.Add(tabPageMahale);
            tabControlFields.Controls.Add(tabPageMasjed);
            tabControlFields.Controls.Add(tabPageAmaken);
            tabControlFields.Location = new Point(8, 5);
            tabControlFields.Name = "tabControlFields";
            tabControlFields.SelectedIndex = 0;
            tabControlFields.Size = new Size(371, 503);
            tabControlFields.TabIndex = 24;
            // 
            // tabPageMahale
            // 
            tabPageMahale.Controls.Add(panelTabPageMahale);
            tabPageMahale.Location = new Point(4, 28);
            tabPageMahale.Name = "tabPageMahale";
            tabPageMahale.Padding = new Padding(3);
            tabPageMahale.Size = new Size(363, 471);
            tabPageMahale.TabIndex = 0;
            tabPageMahale.Tag = "mahale";
            tabPageMahale.Text = "محله";
            tabPageMahale.UseVisualStyleBackColor = true;
            // 
            // panelTabPageMahale
            // 
            panelTabPageMahale.Controls.Add(txtMahaleName);
            panelTabPageMahale.Controls.Add(lblMahaleName);
            panelTabPageMahale.Controls.Add(lblMahaleId);
            panelTabPageMahale.Controls.Add(txtMahaleId);
            panelTabPageMahale.Dock = DockStyle.Fill;
            panelTabPageMahale.Location = new Point(3, 3);
            panelTabPageMahale.Name = "panelTabPageMahale";
            panelTabPageMahale.Size = new Size(357, 465);
            panelTabPageMahale.TabIndex = 24;
            panelTabPageMahale.Visible = false;
            // 
            // txtMahaleName
            // 
            txtMahaleName.BackColor = SystemColors.Window;
            txtMahaleName.Location = new Point(0, 34);
            txtMahaleName.Name = "txtMahaleName";
            txtMahaleName.Padding = new Padding(10, 7, 10, 7);
            txtMahaleName.RightToLeft = RightToLeft.Yes;
            txtMahaleName.Size = new Size(290, 30);
            txtMahaleName.TabIndex = 23;
            // 
            // lblMahaleName
            // 
            lblMahaleName.AutoSize = true;
            lblMahaleName.Location = new Point(296, 40);
            lblMahaleName.Name = "lblMahaleName";
            lblMahaleName.Size = new Size(58, 17);
            lblMahaleName.TabIndex = 21;
            lblMahaleName.Text = "نام محله";
            // 
            // lblMahaleId
            // 
            lblMahaleId.AutoSize = true;
            lblMahaleId.Location = new Point(296, 3);
            lblMahaleId.Name = "lblMahaleId";
            lblMahaleId.Size = new Size(57, 17);
            lblMahaleId.TabIndex = 20;
            lblMahaleId.Text = "کد محله";
            // 
            // txtMahaleId
            // 
            txtMahaleId.BackColor = SystemColors.Control;
            txtMahaleId.Location = new Point(0, 0);
            txtMahaleId.Name = "txtMahaleId";
            txtMahaleId.Padding = new Padding(10, 7, 10, 7);
            txtMahaleId.ReadOnly = true;
            txtMahaleId.RightToLeft = RightToLeft.Yes;
            txtMahaleId.Size = new Size(290, 30);
            txtMahaleId.TabIndex = 22;
            // 
            // tabPageMasjed
            // 
            tabPageMasjed.Controls.Add(panelTabPageMasjed);
            tabPageMasjed.Location = new Point(4, 28);
            tabPageMasjed.Name = "tabPageMasjed";
            tabPageMasjed.Padding = new Padding(3);
            tabPageMasjed.Size = new Size(363, 471);
            tabPageMasjed.TabIndex = 1;
            tabPageMasjed.Tag = "masjed";
            tabPageMasjed.Text = "مسجد";
            tabPageMasjed.UseVisualStyleBackColor = true;
            // 
            // panelTabPageMasjed
            // 
            panelTabPageMasjed.Controls.Add(txtMasjedId);
            panelTabPageMasjed.Controls.Add(lblMasjedPhoneNumber);
            panelTabPageMasjed.Controls.Add(lblMasjedLocation);
            panelTabPageMasjed.Controls.Add(dgvMasjedPhoneNumber);
            panelTabPageMasjed.Controls.Add(lblMasjedId);
            panelTabPageMasjed.Controls.Add(dgvMasjedAddress);
            panelTabPageMasjed.Controls.Add(txtMasjedName);
            panelTabPageMasjed.Controls.Add(lblMasjedName);
            panelTabPageMasjed.Dock = DockStyle.Fill;
            panelTabPageMasjed.Location = new Point(3, 3);
            panelTabPageMasjed.Name = "panelTabPageMasjed";
            panelTabPageMasjed.Size = new Size(357, 465);
            panelTabPageMasjed.TabIndex = 16;
            panelTabPageMasjed.Visible = false;
            // 
            // txtMasjedId
            // 
            txtMasjedId.BackColor = SystemColors.Control;
            txtMasjedId.Location = new Point(0, 0);
            txtMasjedId.Name = "txtMasjedId";
            txtMasjedId.Padding = new Padding(10, 7, 10, 7);
            txtMasjedId.ReadOnly = true;
            txtMasjedId.RightToLeft = RightToLeft.Yes;
            txtMasjedId.Size = new Size(280, 30);
            txtMasjedId.TabIndex = 13;
            // 
            // lblMasjedPhoneNumber
            // 
            lblMasjedPhoneNumber.AutoSize = true;
            lblMasjedPhoneNumber.Location = new Point(274, 235);
            lblMasjedPhoneNumber.Name = "lblMasjedPhoneNumber";
            lblMasjedPhoneNumber.Size = new Size(79, 17);
            lblMasjedPhoneNumber.TabIndex = 6;
            lblMasjedPhoneNumber.Text = "تلفن مسجد";
            // 
            // lblMasjedLocation
            // 
            lblMasjedLocation.BackColor = Color.FromArgb(255, 192, 192);
            lblMasjedLocation.Location = new Point(0, 401);
            lblMasjedLocation.Name = "lblMasjedLocation";
            lblMasjedLocation.Size = new Size(353, 32);
            lblMasjedLocation.TabIndex = 15;
            lblMasjedLocation.Text = "موقعیت مسجد ثبت نشده است";
            lblMasjedLocation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dgvMasjedPhoneNumber
            // 
            dgvMasjedPhoneNumber.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvMasjedPhoneNumber.BorderStyle = BorderStyle.None;
            dgvMasjedPhoneNumber.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMasjedPhoneNumber.Columns.AddRange(new DataGridViewColumn[] { Masjed_Tel_Title, Masjed_Tel_Value });
            dgvMasjedPhoneNumber.Location = new Point(0, 255);
            dgvMasjedPhoneNumber.Name = "dgvMasjedPhoneNumber";
            dgvMasjedPhoneNumber.Size = new Size(351, 141);
            dgvMasjedPhoneNumber.TabIndex = 9;
            // 
            // Masjed_Tel_Title
            // 
            Masjed_Tel_Title.HeaderText = "عنوان";
            Masjed_Tel_Title.Name = "Masjed_Tel_Title";
            // 
            // Masjed_Tel_Value
            // 
            Masjed_Tel_Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Masjed_Tel_Value.HeaderText = "تلفن";
            Masjed_Tel_Value.Name = "Masjed_Tel_Value";
            // 
            // lblMasjedId
            // 
            lblMasjedId.AutoSize = true;
            lblMasjedId.Location = new Point(286, 3);
            lblMasjedId.Name = "lblMasjedId";
            lblMasjedId.Size = new Size(67, 17);
            lblMasjedId.TabIndex = 0;
            lblMasjedId.Text = "کد مسجد";
            // 
            // dgvMasjedAddress
            // 
            dgvMasjedAddress.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvMasjedAddress.BorderStyle = BorderStyle.None;
            dgvMasjedAddress.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMasjedAddress.Columns.AddRange(new DataGridViewColumn[] { Masjed_Address_Title, Masjed_Address_Value });
            dgvMasjedAddress.Location = new Point(0, 70);
            dgvMasjedAddress.Name = "dgvMasjedAddress";
            dgvMasjedAddress.Size = new Size(351, 162);
            dgvMasjedAddress.TabIndex = 8;
            // 
            // Masjed_Address_Title
            // 
            Masjed_Address_Title.HeaderText = "عنوان";
            Masjed_Address_Title.Name = "Masjed_Address_Title";
            // 
            // Masjed_Address_Value
            // 
            Masjed_Address_Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Masjed_Address_Value.HeaderText = "آدرس";
            Masjed_Address_Value.Name = "Masjed_Address_Value";
            // 
            // txtMasjedName
            // 
            txtMasjedName.BackColor = SystemColors.Window;
            txtMasjedName.Location = new Point(0, 34);
            txtMasjedName.Name = "txtMasjedName";
            txtMasjedName.Padding = new Padding(10, 7, 10, 7);
            txtMasjedName.RightToLeft = RightToLeft.Yes;
            txtMasjedName.Size = new Size(280, 30);
            txtMasjedName.TabIndex = 14;
            // 
            // lblMasjedName
            // 
            lblMasjedName.AutoSize = true;
            lblMasjedName.Location = new Point(286, 40);
            lblMasjedName.Name = "lblMasjedName";
            lblMasjedName.Size = new Size(68, 17);
            lblMasjedName.TabIndex = 2;
            lblMasjedName.Text = "نام مسجد";
            // 
            // tabPageAmaken
            // 
            tabPageAmaken.Controls.Add(panelTabPageAmaken);
            tabPageAmaken.Location = new Point(4, 27);
            tabPageAmaken.Name = "tabPageAmaken";
            tabPageAmaken.Padding = new Padding(3);
            tabPageAmaken.Size = new Size(363, 472);
            tabPageAmaken.TabIndex = 2;
            tabPageAmaken.Tag = "amaken";
            tabPageAmaken.Text = "اماکن";
            tabPageAmaken.UseVisualStyleBackColor = true;
            // 
            // panelTabPageAmaken
            // 
            panelTabPageAmaken.Controls.Add(txtAmakenId);
            panelTabPageAmaken.Controls.Add(label5);
            panelTabPageAmaken.Controls.Add(lblAmakenLocation);
            panelTabPageAmaken.Controls.Add(dgvMakanPhoneNumber);
            panelTabPageAmaken.Controls.Add(lblManakId);
            panelTabPageAmaken.Controls.Add(dgvMakanAddress);
            panelTabPageAmaken.Controls.Add(txtAmakenName);
            panelTabPageAmaken.Controls.Add(lblMakanName);
            panelTabPageAmaken.Dock = DockStyle.Fill;
            panelTabPageAmaken.Location = new Point(3, 3);
            panelTabPageAmaken.Name = "panelTabPageAmaken";
            panelTabPageAmaken.Size = new Size(357, 466);
            panelTabPageAmaken.TabIndex = 24;
            panelTabPageAmaken.Visible = false;
            // 
            // txtAmakenId
            // 
            txtAmakenId.BackColor = SystemColors.Control;
            txtAmakenId.Location = new Point(0, 0);
            txtAmakenId.Name = "txtAmakenId";
            txtAmakenId.Padding = new Padding(10, 7, 10, 7);
            txtAmakenId.ReadOnly = true;
            txtAmakenId.RightToLeft = RightToLeft.Yes;
            txtAmakenId.Size = new Size(280, 30);
            txtAmakenId.TabIndex = 21;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(285, 235);
            label5.Name = "label5";
            label5.Size = new Size(66, 17);
            label5.TabIndex = 18;
            label5.Text = "تلفن مکان";
            // 
            // lblAmakenLocation
            // 
            lblAmakenLocation.BackColor = Color.FromArgb(255, 192, 192);
            lblAmakenLocation.Location = new Point(0, 401);
            lblAmakenLocation.Name = "lblAmakenLocation";
            lblAmakenLocation.Size = new Size(353, 32);
            lblAmakenLocation.TabIndex = 23;
            lblAmakenLocation.Text = "موقعیت مکان ثبت نشده است";
            lblAmakenLocation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dgvMakanPhoneNumber
            // 
            dgvMakanPhoneNumber.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvMakanPhoneNumber.BorderStyle = BorderStyle.None;
            dgvMakanPhoneNumber.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMakanPhoneNumber.Location = new Point(0, 255);
            dgvMakanPhoneNumber.Name = "dgvMakanPhoneNumber";
            dgvMakanPhoneNumber.Size = new Size(351, 141);
            dgvMakanPhoneNumber.TabIndex = 20;
            // 
            // lblManakId
            // 
            lblManakId.AutoSize = true;
            lblManakId.Location = new Point(286, 3);
            lblManakId.Name = "lblManakId";
            lblManakId.Size = new Size(54, 17);
            lblManakId.TabIndex = 16;
            lblManakId.Text = "کد مکان";
            // 
            // dgvMakanAddress
            // 
            dgvMakanAddress.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvMakanAddress.BorderStyle = BorderStyle.None;
            dgvMakanAddress.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMakanAddress.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dgvMakanAddress.Location = new Point(0, 70);
            dgvMakanAddress.Name = "dgvMakanAddress";
            dgvMakanAddress.Size = new Size(351, 162);
            dgvMakanAddress.TabIndex = 19;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "عنوان";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "آدرس";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // txtAmakenName
            // 
            txtAmakenName.BackColor = SystemColors.Window;
            txtAmakenName.Location = new Point(0, 34);
            txtAmakenName.Name = "txtAmakenName";
            txtAmakenName.Padding = new Padding(10, 7, 10, 7);
            txtAmakenName.RightToLeft = RightToLeft.Yes;
            txtAmakenName.Size = new Size(280, 30);
            txtAmakenName.TabIndex = 22;
            // 
            // lblMakanName
            // 
            lblMakanName.AutoSize = true;
            lblMakanName.Location = new Point(286, 40);
            lblMakanName.Name = "lblMakanName";
            lblMakanName.Size = new Size(55, 17);
            lblMakanName.TabIndex = 17;
            lblMakanName.Text = "نام مکان";
            // 
            // button3
            // 
            button3.Location = new Point(5, 514);
            button3.Name = "button3";
            button3.Size = new Size(75, 25);
            button3.TabIndex = 12;
            button3.Text = "search";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            // 
            // button2
            // 
            button2.Location = new Point(86, 514);
            button2.Name = "button2";
            button2.Size = new Size(75, 25);
            button2.TabIndex = 11;
            button2.Text = "hide";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            // 
            // button1
            // 
            button1.Location = new Point(167, 514);
            button1.Name = "button1";
            button1.Size = new Size(75, 25);
            button1.TabIndex = 10;
            button1.Text = "show";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ucMap
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tlpMain);
            Font = new Font("Tahoma", 10F);
            Name = "ucMap";
            Size = new Size(1182, 668);
            tlpMain.ResumeLayout(false);
            flpTop.ResumeLayout(false);
            panelSearchBox.ResumeLayout(false);
            panelSearchBox.PerformLayout();
            panelShowNames.ResumeLayout(false);
            panelShowNames.PerformLayout();
            tlpMapAndFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            panelFields.ResumeLayout(false);
            tabControlFields.ResumeLayout(false);
            tabPageMahale.ResumeLayout(false);
            panelTabPageMahale.ResumeLayout(false);
            panelTabPageMahale.PerformLayout();
            tabPageMasjed.ResumeLayout(false);
            panelTabPageMasjed.ResumeLayout(false);
            panelTabPageMasjed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMasjedPhoneNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMasjedAddress).EndInit();
            tabPageAmaken.ResumeLayout(false);
            panelTabPageAmaken.ResumeLayout(false);
            panelTabPageAmaken.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMakanPhoneNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMakanAddress).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpMain;
        private TableLayoutPanel tlpMapAndFields;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Panel panelFields;
        //private TextBox txtMasjedName;
        private Label lblMasjedName;
        //private TextBox txtMasjedId;
        private Label lblMasjedId;
        private Label lblMasjedPhoneNumber;
        private DataGridViewImageColumn dataGridViewImageColumn4;
        private DataGridViewImageColumn dataGridViewImageColumn3;
        private DataGridViewImageColumn dataGridViewImageColumn2;
        private DataGridViewTextBoxColumn Title;
        private DataGridViewTextBoxColumn Address;
        private DataGridView dgvMasjedAddress;
        private DataGridView dgvMasjedPhoneNumber;
        private Button button1;
        private Button button2;
        private Button button3;
        private Controls.RoundedTextBox.RoundedTextBox txtMasjedId;
        private Controls.RoundedTextBox.RoundedTextBox txtMasjedName;
        private FlowLayoutPanel flpTop;
        private Controls.ModernButton.ModernButton btnAddNewArea;
        private Controls.ModernButton.ModernButton btnSaveAll;
        private Controls.ModernRoundedPanel.ModernRoundedPanel panelSearchBox;
        //private Controls.RoundedTextBox.RoundedTextBox txtSearchLocation;
        private Controls.ModernRoundedSearchBox.ModernRoundedSearchBox txtSearchLocation;
        private Controls.ModernButton.ModernButton btnPickLocation;
        private Label lblSearchPlace;
        private Label lblMasjedLocation;
        private Controls.RoundedTextBox.RoundedTextBox txtMahaleName;
        private Controls.RoundedTextBox.RoundedTextBox txtMahaleId;
        private Label lblMahaleName;
        private Label lblMahaleId;
        private TabControl tabControlFields;
        private TabPage tabPageMahale;
        private TabPage tabPageMasjed;
        private TabPage tabPageAmaken;
        private Controls.RoundedTextBox.RoundedTextBox txtAmakenId;
        private Label lblAmakenLocation;
        private Label lblManakId;
        private Controls.RoundedTextBox.RoundedTextBox txtAmakenName;
        private Label lblMakanName;
        private DataGridView dgvMakanAddress;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridView dgvMakanPhoneNumber;
        private Label label5;
        private Controls.ModernRoundedPanel.ModernRoundedPanel panelShowNames;
        private CheckBox chbShowMahale;
        private CheckBox chbShowMakan;
        private CheckBox chbShowMasjed;
        private CheckBox chbShowNames;
        private ComboBox cmbPlaceType;
        private Panel panelTabPageMahale;
        private Panel panelTabPageMasjed;
        private Panel panelTabPageAmaken;
        private DataGridViewTextBoxColumn Masjed_Tel_Title;
        private DataGridViewTextBoxColumn Masjed_Tel_Value;
        private DataGridViewTextBoxColumn Masjed_Address_Title;
        private DataGridViewTextBoxColumn Masjed_Address_Value;
    }
}

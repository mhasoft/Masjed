using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace WinForm.Controls.ModernDataGridView
{
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class ModernDataGridView : DataGridView
    {
        private const string DeleteColumnName = "__delete_column__";

        private Color _surfaceBackColor = Color.FromArgb(148, 148, 148);
        private Color _gridBackColor = Color.White;
        private Color _headerBackColor = Color.White;
        private Color _headerForeColor = Color.FromArgb(46, 148, 219);
        private Color _cellForeColor = Color.FromArgb(70, 70, 70);
        private Color _gridLineColor = Color.FromArgb(230, 230, 230);
        private Color _headerShadowColor = Color.FromArgb(22, 0, 0, 0);
        private Color _selectionBackColor = Color.FromArgb(245, 250, 255);
        private Color _selectionForeColor = Color.FromArgb(70, 70, 70);
        private Color _deleteIconColor = Color.FromArgb(255, 42, 42);
        private Color _deleteHoverBackColor = Color.FromArgb(255, 245, 245);

        private int _cornerRadius = 16;
        private int _headerHeightCustom = 72;
        private int _rowHeightCustom = 54;
        private int _outerPadding = 18;
        private int _deleteColumnWidth = 54;
        private int _headerFontSize = 16;
        private int _cellFontSize = 10;
        private int _shadowHeight = 10;
        private int _iconSize = 20;
        private int _iconBitmapSize = 28;

        private bool _showDeleteColumn = true;
        private bool _allowInlineAddRows = true;
        private bool _showDeleteOnlyForFilledRows = true;
        private bool _enableRoundedRegion = true;

        private string _deleteToolTipText = "حذف سطر";

        private Bitmap _deleteIconBitmap;

        public ModernDataGridView()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            DoubleBuffered = true;
            RightToLeft = RightToLeft.Yes;

            InitializeModernGrid();
            RebuildDeleteIcon();
            UpdateDeleteColumn();
            ApplyRoundedRegion();
        }

        #region Properties

        [Category("Modern Appearance")]
        [Description("رنگ زمینه بیرونی کنترل.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SurfaceBackColor
        {
            get => _surfaceBackColor;
            set
            {
                if (_surfaceBackColor == value)
                    return;

                _surfaceBackColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ بدنه اصلی گرید.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernGridBackColor
        {
            get => _gridBackColor;
            set
            {
                if (_gridBackColor == value)
                    return;

                _gridBackColor = value;
                BackgroundColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ زمینه هدر.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernHeaderBackColor
        {
            get => _headerBackColor;
            set
            {
                if (_headerBackColor == value)
                    return;

                _headerBackColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ متن هدر.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernHeaderForeColor
        {
            get => _headerForeColor;
            set
            {
                if (_headerForeColor == value)
                    return;

                _headerForeColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ متن سلول‌ها.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernCellForeColor
        {
            get => _cellForeColor;
            set
            {
                if (_cellForeColor == value)
                    return;

                _cellForeColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ خطوط گرید.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernGridLineColor
        {
            get => _gridLineColor;
            set
            {
                if (_gridLineColor == value)
                    return;

                _gridLineColor = value;
                GridColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ سایه زیر هدر.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernHeaderShadowColor
        {
            get => _headerShadowColor;
            set
            {
                if (_headerShadowColor == value)
                    return;

                _headerShadowColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ انتخاب سلول.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernSelectionBackColor
        {
            get => _selectionBackColor;
            set
            {
                if (_selectionBackColor == value)
                    return;

                _selectionBackColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ متن انتخاب‌شده.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernSelectionForeColor
        {
            get => _selectionForeColor;
            set
            {
                if (_selectionForeColor == value)
                    return;

                _selectionForeColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ آیکن حذف.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernDeleteIconColor
        {
            get => _deleteIconColor;
            set
            {
                if (_deleteIconColor == value)
                    return;

                _deleteIconColor = value;
                RebuildDeleteIcon();
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        [Description("رنگ Hover سلول حذف.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ModernDeleteHoverBackColor
        {
            get => _deleteHoverBackColor;
            set
            {
                if (_deleteHoverBackColor == value)
                    return;

                _deleteHoverBackColor = value;
                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("شعاع گردی گوشه‌ها.")]
        [DefaultValue(16)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                value = Math.Max(0, value);

                if (_cornerRadius == value)
                    return;

                _cornerRadius = value;
                ApplyRoundedRegion();
                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("ارتفاع هدر.")]
        [DefaultValue(72)]
        public int HeaderHeightCustom
        {
            get => _headerHeightCustom;
            set
            {
                value = Math.Max(32, value);

                if (_headerHeightCustom == value)
                    return;

                _headerHeightCustom = value;
                ColumnHeadersHeight = _headerHeightCustom;
                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("ارتفاع ردیف‌ها.")]
        [DefaultValue(54)]
        public int RowHeightCustom
        {
            get => _rowHeightCustom;
            set
            {
                value = Math.Max(28, value);

                if (_rowHeightCustom == value)
                    return;

                _rowHeightCustom = value;
                RowTemplate.Height = _rowHeightCustom;

                foreach (DataGridViewRow row in Rows)
                    row.Height = _rowHeightCustom;

                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("فاصله داخلی از لبه‌ها.")]
        [DefaultValue(18)]
        public int OuterPadding
        {
            get => _outerPadding;
            set
            {
                value = Math.Max(0, value);

                if (_outerPadding == value)
                    return;

                _outerPadding = value;
                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("عرض ستون حذف.")]
        [DefaultValue(54)]
        public int DeleteColumnWidth
        {
            get => _deleteColumnWidth;
            set
            {
                value = Math.Max(36, value);

                if (_deleteColumnWidth == value)
                    return;

                _deleteColumnWidth = value;
                UpdateDeleteColumn();
                Invalidate();
            }
        }

        [Category("Modern Layout")]
        [Description("ارتفاع سایه زیر هدر.")]
        [DefaultValue(10)]
        public int HeaderShadowHeight
        {
            get => _shadowHeight;
            set
            {
                value = Math.Max(0, value);

                if (_shadowHeight == value)
                    return;

                _shadowHeight = value;
                Invalidate();
            }
        }

        [Category("Modern Font")]
        [Description("اندازه فونت هدر.")]
        [DefaultValue(16)]
        public int HeaderFontSize
        {
            get => _headerFontSize;
            set
            {
                value = Math.Max(8, value);

                if (_headerFontSize == value)
                    return;

                _headerFontSize = value;

                ColumnHeadersDefaultCellStyle.Font =
                    new Font("Tahoma", _headerFontSize, FontStyle.Bold);

                Invalidate();
            }
        }

        [Category("Modern Font")]
        [Description("اندازه فونت سلول‌ها.")]
        [DefaultValue(10)]
        public int CellFontSize
        {
            get => _cellFontSize;
            set
            {
                value = Math.Max(7, value);

                if (_cellFontSize == value)
                    return;

                _cellFontSize = value;

                DefaultCellStyle.Font =
                    new Font("Tahoma", _cellFontSize, FontStyle.Regular);

                Invalidate();
            }
        }

        [Category("Modern Behavior")]
        [Description("نمایش ستون حذف.")]
        [DefaultValue(true)]
        public bool ShowDeleteColumn
        {
            get => _showDeleteColumn;
            set
            {
                if (_showDeleteColumn == value)
                    return;

                _showDeleteColumn = value;
                UpdateDeleteColumn();
                Invalidate();
            }
        }

        [Category("Modern Behavior")]
        [Description("امکان افزودن سطر جدید از طریق سطر خالی پایین.")]
        [DefaultValue(true)]
        public bool AllowInlineAddRows
        {
            get => _allowInlineAddRows;
            set
            {
                if (_allowInlineAddRows == value)
                    return;

                _allowInlineAddRows = value;
                AllowUserToAddRows = value;
                Invalidate();
            }
        }

        [Category("Modern Behavior")]
        [Description("آیکن حذف فقط برای سطرهای دارای مقدار نمایش داده شود.")]
        [DefaultValue(true)]
        public bool ShowDeleteOnlyForFilledRows
        {
            get => _showDeleteOnlyForFilledRows;
            set
            {
                if (_showDeleteOnlyForFilledRows == value)
                    return;

                _showDeleteOnlyForFilledRows = value;
                Invalidate();
            }
        }

        [Category("Modern Behavior")]
        [Description("کنترل به صورت Region گرد شود.")]
        [DefaultValue(true)]
        public bool EnableRoundedRegion
        {
            get => _enableRoundedRegion;
            set
            {
                if (_enableRoundedRegion == value)
                    return;

                _enableRoundedRegion = value;
                ApplyRoundedRegion();
                Invalidate();
            }
        }

        [Category("Modern Text")]
        [Description("متن Tooltip ستون حذف.")]
        [DefaultValue("حذف سطر")]
        public string DeleteToolTipText
        {
            get => _deleteToolTipText;
            set
            {
                value ??= string.Empty;

                if (_deleteToolTipText == value)
                    return;

                _deleteToolTipText = value;
                UpdateDeleteColumn();
            }
        }

        #endregion


        #region Init

        private void InitializeModernGrid()
        {
            DoubleBuffered = true;

            AllowUserToAddRows = _allowInlineAddRows;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            AllowUserToResizeColumns = false;
            AllowUserToOrderColumns = false;

            AutoGenerateColumns = false;
            MultiSelect = false;
            EditMode = DataGridViewEditMode.EditOnEnter;
            SelectionMode = DataGridViewSelectionMode.CellSelect;

            BorderStyle = BorderStyle.None;
            BackgroundColor = _gridBackColor;
            GridColor = _gridLineColor;

            RowHeadersVisible = false;
            EnableHeadersVisualStyles = false;

            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = _headerHeightCustom;

            RowTemplate.Height = _rowHeightCustom;

            CellBorderStyle = DataGridViewCellBorderStyle.Single;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = _headerBackColor,
                ForeColor = _headerForeColor,
                SelectionBackColor = _headerBackColor,
                SelectionForeColor = _headerForeColor,
                Font = new Font("Tahoma", _headerFontSize, FontStyle.Bold),
                WrapMode = DataGridViewTriState.False,
                Padding = new Padding(4, 0, 4, 0)
            };

            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = _gridBackColor,
                ForeColor = _cellForeColor,
                SelectionBackColor = _selectionBackColor,
                SelectionForeColor = _selectionForeColor,
                Font = new Font("Tahoma", _cellFontSize, FontStyle.Regular),
                WrapMode = DataGridViewTriState.False,
                Padding = new Padding(6, 0, 6, 0)
            };

            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = _gridBackColor,
                ForeColor = _cellForeColor,
                SelectionBackColor = _selectionBackColor,
                SelectionForeColor = _selectionForeColor
            };

            RowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = _gridBackColor,
                ForeColor = _cellForeColor,
                SelectionBackColor = _selectionBackColor,
                SelectionForeColor = _selectionForeColor
            };

            CellPainting -= ModernDataGridView_CellPainting;
            CellPainting += ModernDataGridView_CellPainting;

            CellClick -= ModernDataGridView_CellClick;
            CellClick += ModernDataGridView_CellClick;

            CellContentClick -= ModernDataGridView_CellClick;
            CellContentClick += ModernDataGridView_CellClick;

            CellMouseEnter -= ModernDataGridView_CellMouseEnter;
            CellMouseEnter += ModernDataGridView_CellMouseEnter;

            CellMouseLeave -= ModernDataGridView_CellMouseLeave;
            CellMouseLeave += ModernDataGridView_CellMouseLeave;

            CellValueChanged -= ModernDataGridView_CellValueChanged;
            CellValueChanged += ModernDataGridView_CellValueChanged;

            CurrentCellDirtyStateChanged -= ModernDataGridView_CurrentCellDirtyStateChanged;
            CurrentCellDirtyStateChanged += ModernDataGridView_CurrentCellDirtyStateChanged;

            EditingControlShowing -= ModernDataGridView_EditingControlShowing;
            EditingControlShowing += ModernDataGridView_EditingControlShowing;

            Resize -= ModernDataGridView_Resize;
            Resize += ModernDataGridView_Resize;
        }

        #endregion

        #region Events

        private void ModernDataGridView_Resize(object sender, EventArgs e)
        {
            ApplyRoundedRegion();
            Invalidate();
        }

        private void ModernDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                InvalidateRow(e.RowIndex);
        }

        private void ModernDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!IsCurrentCellDirty)
                return;

            CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (CurrentCell != null)
                InvalidateRow(CurrentCell.RowIndex);
        }

        private void ModernDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.TextChanged -= EditingTextBox_TextChanged;
                textBox.TextChanged += EditingTextBox_TextChanged;
            }
        }

        private void EditingTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CurrentCell != null)
                InvalidateRow(CurrentCell.RowIndex);
        }

        private void ModernDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (Columns[e.ColumnIndex].Name != DeleteColumnName)
                return;

            Cursor = Cursors.Hand;
            InvalidateCell(e.ColumnIndex, e.RowIndex);
        }

        private void ModernDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            Cursor = Cursors.Default;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                InvalidateCell(e.ColumnIndex, e.RowIndex);
        }

        private void ModernDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (!Columns.Contains(DeleteColumnName))
                return;

            if (Columns[e.ColumnIndex].Name != DeleteColumnName)
                return;

            DataGridViewRow row = Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            EndEdit();

            if (_showDeleteOnlyForFilledRows && !RowHasAnyValue(row, e.RowIndex))
                return;

            Rows.RemoveAt(e.RowIndex);
        }

        private void ModernDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                PaintHeaderCell(e);
                return;
            }

            if (e.ColumnIndex < 0)
                return;

            if (Columns[e.ColumnIndex].Name == DeleteColumnName)
            {
                PaintDeleteCell(e);
                return;
            }

            PaintNormalCell(e);
        }

        #endregion

        #region Paint

        private void PaintHeaderCell(DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush backBrush = new SolidBrush(_headerBackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.CellBounds);
            }

            Rectangle shadowRect = new Rectangle(
                e.CellBounds.Left,
                e.CellBounds.Bottom - 1,
                e.CellBounds.Width,
                _shadowHeight);

            using (LinearGradientBrush shadowBrush = new LinearGradientBrush(
                       shadowRect,
                       _headerShadowColor,
                       Color.Transparent,
                       LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(shadowBrush, shadowRect);
            }

            TextRenderer.DrawText(
                e.Graphics,
                Convert.ToString(e.FormattedValue),
                new Font("Tahoma", _headerFontSize, FontStyle.Bold),
                e.CellBounds,
                _headerForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);

            using (Pen pen = new Pen(_gridLineColor))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Top,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Bottom);

                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1);
            }

            e.Handled = true;
        }

        private void PaintNormalCell(DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color backColor = (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected
                ? _selectionBackColor
                : _gridBackColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.CellBounds);
            }

            TextRenderer.DrawText(
                e.Graphics,
                Convert.ToString(e.FormattedValue),
                new Font("Tahoma", _cellFontSize, FontStyle.Regular),
                Rectangle.Inflate(e.CellBounds, -8, 0),
                _cellForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);

            using (Pen pen = new Pen(_gridLineColor))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Top,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Bottom);

                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1);
            }

            e.Handled = true;
        }

        private void PaintDeleteCell(DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DataGridViewRow row = Rows[e.RowIndex];
            bool hasValue = RowHasAnyValue(row, e.RowIndex);
            bool canShow = !row.IsNewRow && (!_showDeleteOnlyForFilledRows || hasValue);
            bool isHover = IsMouseOverCell(e.RowIndex, e.ColumnIndex);

            Color backColor = isHover && canShow
                ? _deleteHoverBackColor
                : _gridBackColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.CellBounds);
            }

            using (Pen pen = new Pen(_gridLineColor))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Top,
                    e.CellBounds.Right - 1,
                    e.CellBounds.Bottom);

                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1);
            }

            if (canShow && _deleteIconBitmap != null)
            {
                int x = e.CellBounds.Left + (e.CellBounds.Width - _deleteIconBitmap.Width) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - _deleteIconBitmap.Height) / 2;

                e.Graphics.DrawImage(
                    _deleteIconBitmap,
                    x,
                    y,
                    _deleteIconBitmap.Width,
                    _deleteIconBitmap.Height);
            }

            e.Handled = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(_surfaceBackColor))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            base.OnPaint(e);
        }

        #endregion

        #region Helpers

        private void UpdateDeleteColumn()
        {
            if (_showDeleteColumn)
            {
                if (!Columns.Contains(DeleteColumnName))
                {
                    DataGridViewImageColumn deleteColumn = new DataGridViewImageColumn
                    {
                        Name = DeleteColumnName,
                        HeaderText = string.Empty,
                        ToolTipText = _deleteToolTipText,
                        Width = _deleteColumnWidth,
                        MinimumWidth = _deleteColumnWidth,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        SortMode = DataGridViewColumnSortMode.NotSortable,
                        Resizable = DataGridViewTriState.False,
                        ReadOnly = true,
                        Image = null
                    };

                    Columns.Add(deleteColumn);
                }

                Columns[DeleteColumnName].Width = _deleteColumnWidth;
                Columns[DeleteColumnName].MinimumWidth = _deleteColumnWidth;
                Columns[DeleteColumnName].ToolTipText = _deleteToolTipText;
            }
            else
            {
                if (Columns.Contains(DeleteColumnName))
                    Columns.Remove(DeleteColumnName);
            }
        }

        private void RebuildDeleteIcon()
        {
            _deleteIconBitmap?.Dispose();
            _deleteIconBitmap = CreateFontAwesomeIcon(
                IconChar.TrashCan,
                _deleteIconColor,
                _iconSize,
                _iconBitmapSize);
        }

        private Bitmap CreateFontAwesomeIcon(
            IconChar iconChar,
            Color color,
            int iconSize,
            int bitmapSize)
        {
            using IconPictureBox iconPictureBox = new IconPictureBox
            {
                IconChar = iconChar,
                IconColor = color,
                IconSize = iconSize,
                BackColor = Color.Transparent,
                Size = new Size(bitmapSize, bitmapSize)
            };

            Bitmap bitmap = new Bitmap(bitmapSize, bitmapSize);
            iconPictureBox.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));

            return bitmap;
        }

        private bool RowHasAnyValue(DataGridViewRow row, int rowIndex)
        {
            if (row == null)
                return false;

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.OwningColumn == null)
                    continue;

                if (cell.OwningColumn.Name == DeleteColumnName)
                    continue;

                string value = cell.Value?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(value))
                    return true;

                if (CurrentCell != null &&
                    CurrentCell.RowIndex == rowIndex &&
                    CurrentCell.ColumnIndex == cell.ColumnIndex)
                {
                    string edited = CurrentCell.EditedFormattedValue?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(edited))
                        return true;

                    if (EditingControl is TextBox textBox)
                    {
                        string text = textBox.Text?.Trim();
                        if (!string.IsNullOrWhiteSpace(text))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool IsMouseOverCell(int rowIndex, int columnIndex)
        {
            Point pt = PointToClient(Cursor.Position);

            if (!ClientRectangle.Contains(pt))
                return false;

            HitTestInfo hit = HitTest(pt.X, pt.Y);
            return hit.RowIndex == rowIndex && hit.ColumnIndex == columnIndex;
        }

        private void ApplyRoundedRegion()
        {
            if (!IsHandleCreated && Width <= 0 && Height <= 0)
                return;

            if (!_enableRoundedRegion || _cornerRadius <= 0)
            {
                Region = null;
                return;
            }

            using GraphicsPath path = CreateRoundedRectanglePath(
                new Rectangle(0, 0, Width - 1, Height - 1),
                _cornerRadius);

            Region = new Region(path);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            base.OnRowsAdded(e);

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                if (i >= 0 && i < Rows.Count)
                    Rows[i].Height = _rowHeightCustom;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedRegion();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _deleteIconBitmap?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Designer Serialization

        public void ResetSurfaceBackColor()
        {
            SurfaceBackColor = Color.FromArgb(148, 148, 148);
        }

        public bool ShouldSerializeSurfaceBackColor()
        {
            return _surfaceBackColor != Color.FromArgb(148, 148, 148);
        }

        public void ResetModernGridBackColor()
        {
            ModernGridBackColor = Color.White;
        }

        public bool ShouldSerializeModernGridBackColor()
        {
            return _gridBackColor != Color.White;
        }

        public void ResetModernHeaderBackColor()
        {
            ModernHeaderBackColor = Color.White;
        }

        public bool ShouldSerializeModernHeaderBackColor()
        {
            return _headerBackColor != Color.White;
        }

        public void ResetModernHeaderForeColor()
        {
            ModernHeaderForeColor = Color.FromArgb(46, 148, 219);
        }

        public bool ShouldSerializeModernHeaderForeColor()
        {
            return _headerForeColor != Color.FromArgb(46, 148, 219);
        }

        public void ResetModernCellForeColor()
        {
            ModernCellForeColor = Color.FromArgb(70, 70, 70);
        }

        public bool ShouldSerializeModernCellForeColor()
        {
            return _cellForeColor != Color.FromArgb(70, 70, 70);
        }

        public void ResetModernGridLineColor()
        {
            ModernGridLineColor = Color.FromArgb(230, 230, 230);
        }

        public bool ShouldSerializeModernGridLineColor()
        {
            return _gridLineColor != Color.FromArgb(230, 230, 230);
        }

        public void ResetModernHeaderShadowColor()
        {
            ModernHeaderShadowColor = Color.FromArgb(22, 0, 0, 0);
        }

        public bool ShouldSerializeModernHeaderShadowColor()
        {
            return _headerShadowColor != Color.FromArgb(22, 0, 0, 0);
        }

        public void ResetModernSelectionBackColor()
        {
            ModernSelectionBackColor = Color.FromArgb(245, 250, 255);
        }

        public bool ShouldSerializeModernSelectionBackColor()
        {
            return _selectionBackColor != Color.FromArgb(245, 250, 255);
        }

        public void ResetModernSelectionForeColor()
        {
            ModernSelectionForeColor = Color.FromArgb(70, 70, 70);
        }

        public bool ShouldSerializeModernSelectionForeColor()
        {
            return _selectionForeColor != Color.FromArgb(70, 70, 70);
        }

        public void ResetModernDeleteIconColor()
        {
            ModernDeleteIconColor = Color.FromArgb(255, 42, 42);
        }

        public bool ShouldSerializeModernDeleteIconColor()
        {
            return _deleteIconColor != Color.FromArgb(255, 42, 42);
        }

        public void ResetModernDeleteHoverBackColor()
        {
            ModernDeleteHoverBackColor = Color.FromArgb(255, 245, 245);
        }

        public bool ShouldSerializeModernDeleteHoverBackColor()
        {
            return _deleteHoverBackColor != Color.FromArgb(255, 245, 245);
        }

        #endregion

    }
}

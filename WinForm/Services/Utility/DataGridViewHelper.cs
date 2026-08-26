using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace WinForm.Services.DataGridViewHelper
{
    public static class DataGridViewHelper
    {
        private const string DeleteColumnName = "colDeleteRow";

        private static readonly Color HeaderBackColor = Color.FromArgb(245, 247, 250);
        private static readonly Color HeaderForeColor = Color.FromArgb(45, 48, 56);

        private static readonly Color GridBackColor = Color.White;
        private static readonly Color RowBackColor = Color.White;
        private static readonly Color AlternatingRowBackColor = Color.FromArgb(250, 251, 253);

        private static readonly Color CellForeColor = Color.FromArgb(45, 48, 56);

        // کمی تیره‌تر از نسخه قبل تا محدوده ستون‌ها مشخص باشد
        private static readonly Color GridLineColor = Color.FromArgb(214, 219, 228);
        private static readonly Color HeaderGridLineColor = Color.FromArgb(198, 205, 216);

        private static readonly Color SelectionBackColor = Color.FromArgb(225, 239, 255);
        private static readonly Color SelectionForeColor = Color.FromArgb(20, 35, 55);

        private static readonly Color CurrentCellBorderColor = Color.FromArgb(66, 133, 244);

        private static readonly Color DeleteIconColor = Color.FromArgb(235, 87, 87);
        private static readonly Color DeleteIconHoverColor = Color.FromArgb(210, 45, 62);

        private static readonly Color DeleteCellHoverBackColor = Color.FromArgb(255, 238, 238);

        /// <summary>
        /// تنظیم DataGridView برای ورود مستقیم اطلاعات و حذف ردیف با کلیک روی آیکن حذف.
        /// کاربر با وارد کردن مقدار در ردیف خالی انتهای گرید، یک ردیف جدید ایجاد می‌کند.
        /// نیازی به دکمه Add یا Remove روی فرم نیست.
        /// </summary>
        /// <param name="dgv">کنترل DataGridView مورد نظر</param>
        public static void ConfigureDataGridWithDeleteColumn(DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            ConfigureBaseStyle(dgv);
            EnsureDeleteColumn(dgv);
            RegisterEvents(dgv);

            dgv.Invalidate();
        }

        /// <summary>
        /// تنظیمات پایه و ظاهری DataGridView.
        /// </summary>
        private static void ConfigureBaseStyle(DataGridView dgv)
        {
            dgv.SuspendLayout();

            dgv.AutoGenerateColumns = false;

            // امکان افزودن ردیف جدید بدون دکمه جداگانه
            dgv.AllowUserToAddRows = true;

            // حذف فقط از طریق آیکن انجام می‌شود
            dgv.AllowUserToDeleteRows = false;

            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = true;

            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;

            dgv.RightToLeft = RightToLeft.Yes;
            dgv.RowHeadersVisible = false;

            dgv.BackgroundColor = GridBackColor;
            dgv.GridColor = GridLineColor;

            dgv.BorderStyle = BorderStyle.FixedSingle;

            /*
             * در این نسخه Border عمودی و افقی فعال است
             * تا محدوده هر ستون دقیقاً مشخص باشد.
             */
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgv.EnableHeadersVisualStyles = false;

            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.RowTemplate.Height = 40;

            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            /*
             * خطوط سلول‌ها:
             * قبلاً Left و Right روی None بود و مرز ستون‌ها معلوم نمی‌شد.
             * اینجا همه Borderها فعال شده‌اند.
             */
            dgv.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            dgv.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = HeaderBackColor,
                ForeColor = HeaderForeColor,
                SelectionBackColor = HeaderBackColor,
                SelectionForeColor = HeaderForeColor,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                Padding = new Padding(5, 0, 5, 0),
                WrapMode = DataGridViewTriState.False
            };

            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                BackColor = RowBackColor,
                ForeColor = CellForeColor,
                SelectionBackColor = SelectionBackColor,
                SelectionForeColor = SelectionForeColor,
                Font = new Font("Tahoma", 9F, FontStyle.Regular),
                Padding = new Padding(7, 0, 7, 0),
                WrapMode = DataGridViewTriState.False
            };

            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AlternatingRowBackColor,
                ForeColor = CellForeColor,
                SelectionBackColor = SelectionBackColor,
                SelectionForeColor = SelectionForeColor
            };

            dgv.RowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = RowBackColor,
                ForeColor = CellForeColor,
                SelectionBackColor = SelectionBackColor,
                SelectionForeColor = SelectionForeColor
            };

            dgv.ResumeLayout();
        }

        /// <summary>
        /// افزودن ستون حذف در صورت عدم وجود.
        /// </summary>
        private static void EnsureDeleteColumn(DataGridView dgv)
        {
            if (dgv.Columns.Contains(DeleteColumnName))
                return;

            DataGridViewImageColumn deleteColumn = new DataGridViewImageColumn
            {
                Name = DeleteColumnName,
                HeaderText = string.Empty,
                ToolTipText = "حذف این ردیف",
                Image = null,
                ImageLayout = DataGridViewImageCellLayout.Normal,
                Width = 46,
                MinimumWidth = 46,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Resizable = DataGridViewTriState.False,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    NullValue = null,
                    Padding = new Padding(0),
                    BackColor = RowBackColor,
                    SelectionBackColor = SelectionBackColor
                }
            };

            /*
             * برای فرم‌های فارسی معمولاً ستون عملیات در سمت چپ ظاهر زیباتری دارد.
             * با توجه به RightToLeft بودن گرید، Insert(0) معمولاً جای مناسبی ایجاد می‌کند.
             */
            dgv.Columns.Insert(0, deleteColumn);
        }

        /// <summary>
        /// ثبت Eventها با جلوگیری از ثبت تکراری.
        /// </summary>
        private static void RegisterEvents(DataGridView dgv)
        {
            dgv.CellContentClick -= DataGridView_DeleteRow_CellContentClick;
            dgv.CellContentClick += DataGridView_DeleteRow_CellContentClick;

            dgv.CellClick -= DataGridView_DeleteRow_CellClick;
            dgv.CellClick += DataGridView_DeleteRow_CellClick;

            dgv.CellMouseEnter -= DataGridView_CellMouseEnter;
            dgv.CellMouseEnter += DataGridView_CellMouseEnter;

            dgv.CellMouseLeave -= DataGridView_CellMouseLeave;
            dgv.CellMouseLeave += DataGridView_CellMouseLeave;

            dgv.CellPainting -= DataGridView_CellPainting;
            dgv.CellPainting += DataGridView_CellPainting;

            dgv.RowPostPaint -= DataGridView_RowPostPaint;
            dgv.RowPostPaint += DataGridView_RowPostPaint;

            dgv.CellValueChanged -= DataGridView_CellValueChanged;
            dgv.CellValueChanged += DataGridView_CellValueChanged;

            dgv.CurrentCellDirtyStateChanged -= DataGridView_CurrentCellDirtyStateChanged;
            dgv.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;

            dgv.EditingControlShowing -= DataGridView_EditingControlShowing;
            dgv.EditingControlShowing += DataGridView_EditingControlShowing;

            dgv.SelectionChanged -= DataGridView_SelectionChanged;
            dgv.SelectionChanged += DataGridView_SelectionChanged;

            dgv.DataError -= DataGridView_DataError;
            dgv.DataError += DataGridView_DataError;
        }

        /// <summary>
        /// کلیک روی آیکن حذف.
        /// CellContentClick برای ImageColumn همیشه در همه شرایط قابل اعتماد نیست،
        /// بنابراین CellClick هم ثبت شده است.
        /// </summary>
        private static void DataGridView_DeleteRow_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            TryDeleteRow(sender, e);
        }

        /// <summary>
        /// کلیک معمولی روی سلول حذف.
        /// </summary>
        private static void DataGridView_DeleteRow_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            TryDeleteRow(sender, e);
        }

        /// <summary>
        /// تلاش برای حذف ردیف در صورت کلیک روی ستون حذف.
        /// </summary>
        private static void TryDeleteRow(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgv.Columns[e.ColumnIndex].Name != DeleteColumnName)
                return;

            DataGridViewRow row = dgv.Rows[e.RowIndex];

            // ردیف خالی مخصوص افزودن ردیف جدید نباید حذف شود.
            if (row.IsNewRow)
                return;

            dgv.EndEdit();

            // بعد از EndEdit مقدار نهایی سلول‌ها بررسی می‌شود.
            if (!RowHasAnyValue(dgv, row, e.RowIndex))
                return;

            dgv.Rows.RemoveAt(e.RowIndex);
        }

        /// <summary>
        /// تغییر Cursor هنگام حرکت روی ستون حذف.
        /// </summary>
        private static void DataGridView_CellMouseEnter(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgv.Columns[e.ColumnIndex].Name != DeleteColumnName)
                return;

            if (dgv.Rows[e.RowIndex].IsNewRow)
                return;

            if (!RowHasAnyValue(dgv, dgv.Rows[e.RowIndex], e.RowIndex))
                return;

            dgv.Cursor = Cursors.Hand;
            dgv.InvalidateCell(e.ColumnIndex, e.RowIndex);
        }

        /// <summary>
        /// برگشت Cursor به حالت عادی.
        /// </summary>
        private static void DataGridView_CellMouseLeave(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            dgv.Cursor = Cursors.Default;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                dgv.InvalidateCell(e.ColumnIndex, e.RowIndex);
        }

        /// <summary>
        /// نقاشی اختصاصی سلول‌ها.
        /// برای ستون حذف، آیکن FontAwesome رسم می‌شود.
        /// برای بقیه سلول‌ها، Border و CurrentCell واضح‌تر رسم می‌شود.
        /// </summary>
        private static void DataGridView_CellPainting(
            object sender,
            DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (e.RowIndex < 0)
            {
                PaintHeaderCell(dgv, e);
                return;
            }

            if (e.ColumnIndex < 0)
                return;

            if (dgv.Columns[e.ColumnIndex].Name == DeleteColumnName)
            {
                PaintDeleteCell(dgv, e);
                return;
            }

            PaintNormalCellBorder(dgv, e);
        }

        /// <summary>
        /// نقاشی Header برای اینکه مرزبندی ستون‌ها در عنوان هم مشخص باشد.
        /// </summary>
        private static void PaintHeaderCell(
            DataGridView dgv,
            DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            e.Paint(
                e.CellBounds,
                DataGridViewPaintParts.Background |
                DataGridViewPaintParts.Border |
                DataGridViewPaintParts.ContentForeground);

            using (Pen pen = new Pen(HeaderGridLineColor))
            {
                Rectangle rect = e.CellBounds;
                rect.Width -= 1;
                rect.Height -= 1;

                e.Graphics.DrawRectangle(pen, rect);
            }

            e.Handled = true;
        }

        /// <summary>
        /// نقاشی سلول‌های معمولی برای داشتن مرز مشخص و سلول فعال مشخص‌تر.
        /// </summary>
        private static void PaintNormalCellBorder(
            DataGridView dgv,
            DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(
                e.CellBounds,
                DataGridViewPaintParts.All);

            using (Pen gridPen = new Pen(GridLineColor))
            {
                Rectangle rect = e.CellBounds;
                rect.Width -= 1;
                rect.Height -= 1;

                e.Graphics.DrawRectangle(gridPen, rect);
            }

            if (dgv.CurrentCell != null &&
                dgv.CurrentCell.RowIndex == e.RowIndex &&
                dgv.CurrentCell.ColumnIndex == e.ColumnIndex)
            {
                using (Pen focusPen = new Pen(CurrentCellBorderColor, 2F))
                {
                    Rectangle focusRect = e.CellBounds;
                    focusRect.X += 1;
                    focusRect.Y += 1;
                    focusRect.Width -= 3;
                    focusRect.Height -= 3;

                    e.Graphics.DrawRectangle(focusPen, focusRect);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// نقاشی اختصاصی ستون حذف.
        /// </summary>
        private static void PaintDeleteCell(
            DataGridView dgv,
            DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewRow row = dgv.Rows[e.RowIndex];

            bool rowHasValue = RowHasAnyValue(dgv, row, e.RowIndex);

            bool isMouseOverCell = IsMouseOverCell(dgv, e.RowIndex, e.ColumnIndex);

            Color backColor = GetRowBackColor(dgv, e.RowIndex, e.State);

            if (isMouseOverCell && rowHasValue && !row.IsNewRow)
                backColor = DeleteCellHoverBackColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.CellBounds);
            }

            using (Pen gridPen = new Pen(GridLineColor))
            {
                Rectangle rect = e.CellBounds;
                rect.Width -= 1;
                rect.Height -= 1;

                e.Graphics.DrawRectangle(gridPen, rect);
            }

            /*
             * نکته مهم:
             * قبلاً وقتی کاربر تازه شروع به تایپ می‌کرد، مقدار هنوز در cell.Value نبود؛
             * بنابراین RowHasAnyValue مقدار را false می‌داد و آیکن نمایش داده نمی‌شد.
             * حالا RowHasAnyValue مقدار EditingControl و EditedFormattedValue را هم بررسی می‌کند.
             */
            if (!row.IsNewRow && rowHasValue)
            {
                Color iconColor = isMouseOverCell
                    ? DeleteIconHoverColor
                    : DeleteIconColor;

                using Bitmap icon = CreateFontAwesomeIcon(
                    IconChar.TrashCan,
                    iconColor,
                    18,
                    28);

                int x = e.CellBounds.Left + (e.CellBounds.Width - icon.Width) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - icon.Height) / 2;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                e.Graphics.DrawImage(icon, x, y, icon.Width, icon.Height);
            }

            e.Handled = true;
        }

        /// <summary>
        /// رسم خط پایین ردیف‌ها برای ظاهر تمیزتر.
        /// با وجود CellBorderStyle.Single، این خط باعث خوانایی بهتر می‌شود.
        /// </summary>
        private static void DataGridView_RowPostPaint(
            object sender,
            DataGridViewRowPostPaintEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            using Pen pen = new Pen(GridLineColor);

            int y = e.RowBounds.Bottom - 1;

            e.Graphics.DrawLine(
                pen,
                e.RowBounds.Left,
                y,
                e.RowBounds.Right,
                y);
        }

        /// <summary>
        /// وقتی مقدار سلول تغییر کرد، گرید مجدداً نقاشی شود
        /// تا آیکن حذف فوراً نمایش داده شود.
        /// </summary>
        private static void DataGridView_CellValueChanged(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (e.RowIndex < 0)
                return;

            InvalidateRow(dgv, e.RowIndex);
        }

        /// <summary>
        /// برای بعضی سلول‌ها که تغییر آنی دارند.
        /// </summary>
        private static void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (!dgv.IsCurrentCellDirty)
                return;

            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (dgv.CurrentCell != null)
                InvalidateRow(dgv, dgv.CurrentCell.RowIndex);
        }

        /// <summary>
        /// هنگام نمایش کنترل ویرایش، تغییر متن آن را دنبال می‌کنیم
        /// تا حین تایپ هم آیکن حذف ظاهر شود.
        /// </summary>
        private static void DataGridView_EditingControlShowing(
            object sender,
            DataGridViewEditingControlShowingEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            if (e.Control is TextBox textBox)
            {
                textBox.TextChanged -= EditingTextBox_TextChanged;
                textBox.TextChanged += EditingTextBox_TextChanged;

                textBox.Tag = dgv;
            }
        }

        /// <summary>
        /// هنگام تایپ در سلول، همان ردیف دوباره نقاشی می‌شود.
        /// این بخش مشکل ظاهر نشدن آیکن هنگام تایپ را حل می‌کند.
        /// </summary>
        private static void EditingTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (textBox.Tag is not DataGridView dgv)
                return;

            if (dgv.CurrentCell == null)
                return;

            InvalidateRow(dgv, dgv.CurrentCell.RowIndex);
        }

        /// <summary>
        /// با تغییر سلول انتخاب‌شده، کادر سلول فعال به‌روزرسانی می‌شود.
        /// </summary>
        private static void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            dgv.Invalidate();
        }

        /// <summary>
        /// جلوگیری از نمایش خطاهای سیستمی هنگام ورود مقدار نامعتبر.
        /// </summary>
        private static void DataGridView_DataError(
            object sender,
            DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        /// <summary>
        /// بررسی اینکه آیا ردیف حداقل یک مقدار واقعی دارد یا نه.
        /// ستون حذف در این بررسی نادیده گرفته می‌شود.
        /// این متد علاوه بر cell.Value، مقدار در حال ویرایش را هم بررسی می‌کند.
        /// </summary>
        private static bool RowHasAnyValue(
            DataGridView dgv,
            DataGridViewRow row,
            int rowIndex)
        {
            if (dgv == null || row == null)
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

                /*
                 * مقدار سلولی که هم‌اکنون در حالت Edit است
                 * ممکن است هنوز داخل cell.Value ثبت نشده باشد.
                 */
                if (dgv.CurrentCell != null &&
                    dgv.CurrentCell.RowIndex == rowIndex &&
                    dgv.CurrentCell.ColumnIndex == cell.ColumnIndex)
                {
                    string editedValue = dgv.CurrentCell.EditedFormattedValue
                        ?.ToString()
                        ?.Trim();

                    if (!string.IsNullOrWhiteSpace(editedValue))
                        return true;

                    if (dgv.EditingControl is TextBox textBox)
                    {
                        string textBoxValue = textBox.Text?.Trim();

                        if (!string.IsNullOrWhiteSpace(textBoxValue))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// آیا ماوس روی همین سلول است؟
        /// </summary>
        private static bool IsMouseOverCell(
            DataGridView dgv,
            int rowIndex,
            int columnIndex)
        {
            Point clientPoint = dgv.PointToClient(Cursor.Position);

            if (!dgv.ClientRectangle.Contains(clientPoint))
                return false;

            DataGridView.HitTestInfo hit = dgv.HitTest(
                clientPoint.X,
                clientPoint.Y);

            return hit.RowIndex == rowIndex &&
                   hit.ColumnIndex == columnIndex;
        }

        /// <summary>
        /// رنگ پس‌زمینه ردیف با درنظرگرفتن انتخاب و ردیف‌های یکی‌درمیان.
        /// </summary>
        private static Color GetRowBackColor(
            DataGridView dgv,
            int rowIndex,
            DataGridViewElementStates state)
        {
            bool selected =
                (state & DataGridViewElementStates.Selected) ==
                DataGridViewElementStates.Selected;

            if (selected)
                return SelectionBackColor;

            if (rowIndex % 2 == 1)
                return AlternatingRowBackColor;

            return RowBackColor;
        }

        /// <summary>
        /// Invalidate کردن کل ردیف.
        /// </summary>
        private static void InvalidateRow(DataGridView dgv, int rowIndex)
        {
            if (dgv == null)
                return;

            if (rowIndex < 0 || rowIndex >= dgv.Rows.Count)
                return;

            dgv.InvalidateRow(rowIndex);
        }

        /// <summary>
        /// ساخت آیکن با استفاده از FontAwesome.Sharp.
        /// </summary>
        /// <param name="iconChar">نوع آیکن</param>
        /// <param name="color">رنگ آیکن</param>
        /// <param name="iconSize">سایز خود آیکن</param>
        /// <param name="bitmapSize">سایز Bitmap خروجی</param>
        private static Bitmap CreateFontAwesomeIcon(
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

            iconPictureBox.DrawToBitmap(
                bitmap,
                new Rectangle(Point.Empty, new Size(bitmapSize, bitmapSize)));

            return bitmap;
        }
    }
}

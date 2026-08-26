using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WinForm.Services.Utility
{
    public static class DataGridViewActionButtons
    {
        private const string DeleteColumnName = "dgvcDelete";
        private const string SelectColumnName = "dgvcSelect";
        private const int IconSize = 18;

        /*
         * تنظیمات هر DataGridView جداگانه نگهداری می‌شود.
         *
         * ConditionalWeakTable باعث می‌شود اگر خود Grid از حافظه حذف شد،
         * تنظیمات مربوط به آن نیز خودکار قابل پاک‌سازی باشد.
         */
        private static readonly ConditionalWeakTable<
            DataGridView,
            GridActionSettings> _gridSettings =
            new ConditionalWeakTable<DataGridView, GridActionSettings>();

        /// <summary>
        /// اضافه‌کردن دکمه‌های عملیاتی به DataGridView.
        ///
        /// فقط برای Actionهایی که ارسال شوند ستون ایجاد می‌شود:
        /// - deleteAction != null => ستون حذف
        /// - selectAction != null => ستون انتخاب
        /// </summary>
        /// <param name="grid">گرید مقصد.</param>
        /// <param name="deleteAction">عملیات حذف، اختیاری.</param>
        /// <param name="selectAction">عملیات انتخاب، اختیاری.</param>
        public static void AddButtons(
            DataGridView grid,
            Action<DataGridViewRow>? deleteAction = null,
            Action<DataGridViewRow>? selectAction = null)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            RemoveButtons(grid);

            // اگر هیچ عملیاتی داده نشده باشد، ستونی اضافه نکن.
            if (deleteAction == null && selectAction == null)
                return;

            GridActionSettings settings = new GridActionSettings
            {
                DeleteAction = deleteAction,
                SelectAction = selectAction
            };

            _gridSettings.Add(grid, settings);

            grid.CellMouseClick += Grid_CellMouseClick;
            grid.CellPainting += Grid_CellPainting;
            grid.CellMouseMove += Grid_CellMouseMove;

            /*
             * فقط در صورت ارسال deleteAction ستون حذف اضافه می‌شود.
             */
            if (deleteAction != null)
            {
                DataGridViewTextBoxColumn deleteColumn =
                    new DataGridViewTextBoxColumn
                    {
                        Name = DeleteColumnName,
                        HeaderText = string.Empty,
                        Width = 30,
                        MinimumWidth = 30,
                        SortMode = DataGridViewColumnSortMode.NotSortable,
                        ReadOnly = true,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter,
                            NullValue = string.Empty
                        }
                    };

                grid.Columns.Add(deleteColumn);
            }

            /*
             * فقط در صورت ارسال selectAction ستون انتخاب اضافه می‌شود.
             */
            if (selectAction != null)
            {
                DataGridViewTextBoxColumn selectColumn =
                    new DataGridViewTextBoxColumn
                    {
                        Name = SelectColumnName,
                        HeaderText = string.Empty,
                        Width = 30,
                        MinimumWidth = 30,
                        SortMode = DataGridViewColumnSortMode.NotSortable,
                        ReadOnly = true,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter,
                            NullValue = string.Empty
                        }
                    };

                grid.Columns.Add(selectColumn);
            }
        }

        /// <summary>
        /// حذف ستون‌ها و Eventهای مربوط به دکمه‌های عملیاتی از گرید.
        /// </summary>
        public static void RemoveButtons(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.CellMouseClick -= Grid_CellMouseClick;
            grid.CellPainting -= Grid_CellPainting;
            grid.CellMouseMove -= Grid_CellMouseMove;

            if (grid.Columns.Contains(DeleteColumnName))
            {
                grid.Columns.Remove(DeleteColumnName);
            }

            if (grid.Columns.Contains(SelectColumnName))
            {
                grid.Columns.Remove(SelectColumnName);
            }

            _gridSettings.Remove(grid);
        }

        private static void Grid_CellPainting(
            object? sender,
            DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView grid)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];

            if (!IsActionColumn(column))
                return;

            e.Paint(
                e.CellBounds,
                DataGridViewPaintParts.Background |
                DataGridViewPaintParts.Border |
                DataGridViewPaintParts.SelectionBackground
            );

            Rectangle iconRect = GetIconRectangle(e.CellBounds);

            if (column.Name == DeleteColumnName)
            {
                DrawTrashIcon(e.Graphics, iconRect, Color.Firebrick);
            }
            else if (column.Name == SelectColumnName)
            {
                DrawCheckIcon(e.Graphics, iconRect, Color.RoyalBlue);
            }

            e.Handled = true;
        }

        private static void Grid_CellMouseClick(
            object? sender,
            DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView grid)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];

            if (!IsActionColumn(column))
                return;

            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            Rectangle iconRect = GetIconRectangle(
                Rectangle.Empty,
                column.Width,
                row.Height
            );

            if (!iconRect.Contains(e.Location))
                return;

            if (!_gridSettings.TryGetValue(grid, out GridActionSettings settings))
                return;

            if (column.Name == DeleteColumnName)
            {
                settings.DeleteAction?.Invoke(row);
                return;
            }

            if (column.Name == SelectColumnName)
            {
                settings.SelectAction?.Invoke(row);
            }
        }

        private static void Grid_CellMouseMove(
            object? sender,
            DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView grid)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                grid.Cursor = Cursors.Default;
                return;
            }

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];

            if (!IsActionColumn(column))
            {
                grid.Cursor = Cursors.Default;
                return;
            }

            Rectangle iconRect = GetIconRectangle(
                Rectangle.Empty,
                column.Width,
                grid.Rows[e.RowIndex].Height
            );

            grid.Cursor = iconRect.Contains(e.Location)
                ? Cursors.Hand
                : Cursors.Default;
        }

        private static bool IsActionColumn(DataGridViewColumn column)
        {
            return column.Name == DeleteColumnName ||
                   column.Name == SelectColumnName;
        }

        #region Icon Part

        private static Rectangle GetIconRectangle(Rectangle cellBounds)
        {
            return new Rectangle(
                cellBounds.X + (cellBounds.Width - IconSize) / 2,
                cellBounds.Y + (cellBounds.Height - IconSize) / 2,
                IconSize,
                IconSize
            );
        }

        private static Rectangle GetIconRectangle(
            Rectangle _,
            int cellWidth,
            int cellHeight)
        {
            return new Rectangle(
                (cellWidth - IconSize) / 2,
                (cellHeight - IconSize) / 2,
                IconSize,
                IconSize
            );
        }

        private static void DrawTrashIcon(
            Graphics graphics,
            Rectangle rect,
            Color color)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(color);

            using Pen pen = new Pen(color, 1.6f)
            {
                LineJoin = LineJoin.Round
            };

            graphics.FillRectangle(
                brush,
                rect.X + 4,
                rect.Y + 6,
                rect.Width - 8,
                rect.Height - 7
            );

            graphics.FillRectangle(
                brush,
                rect.X + 3,
                rect.Y + 4,
                rect.Width - 6,
                2
            );

            graphics.DrawLine(
                pen,
                rect.X + 6,
                rect.Y + 3,
                rect.X + rect.Width - 6,
                rect.Y + 3
            );

            graphics.DrawArc(
                pen,
                rect.X + 6,
                rect.Y + 1,
                rect.Width - 12,
                4,
                180,
                180
            );
        }

        private static void DrawCheckIcon(
            Graphics graphics,
            Rectangle rect,
            Color color)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using Pen pen = new Pen(color, 2.8f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            PointF startPoint = new PointF(
                rect.X + 3,
                rect.Y + rect.Height / 2
            );

            PointF middlePoint = new PointF(
                rect.X + rect.Width / 2 - 1,
                rect.Y + rect.Height - 4
            );

            PointF endPoint = new PointF(
                rect.X + rect.Width - 3,
                rect.Y + 4
            );

            graphics.DrawLines(
                pen,
                new[]
                {
                    startPoint,
                    middlePoint,
                    endPoint
                }
            );
        }

        #endregion

        private class GridActionSettings
        {
            public Action<DataGridViewRow>? DeleteAction { get; set; }

            public Action<DataGridViewRow>? SelectAction { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WinForm.Controls.AnimatedTiles;
using WinApp = System.Windows.Forms.Application;

namespace VideoWall.WinForms.UserControls.CustomeControls.CustomeTabControl
{
    /// <summary>
    /// مدل ورودی برای ایجاد تب‌ها
    /// </summary>
    public class TabDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class CustomTab : UserControl
    {
        public TableLayoutPanel mainLayout = null!; // <--- گزینه های درون پنل
        public FlowLayoutPanel itemsPanel = null!; // <--- سربرگ ها
        private Button btnAdd = null!;
        private Button btnCloseAll = null!;

        private readonly List<TabItem> items = new();
        private TabItem? selectedItem;
        private bool _disposed;

        public event EventHandler<TabEventArgs>? ItemAdded;
        public event EventHandler<TabEventArgs>? ItemClosing;
        public event EventHandler<TabEventArgs>? ItemSelected;
        public event EventHandler<TabEventArgs>? ItemDeselected;

        [Category("Action")]
        [Description("هنگام کلیک روی دکمه افزودن تب (+) اجرا می‌شود.")]
        public event EventHandler? AddButtonClicked;

        /// <summary>
        /// پنل محتوای متناظر با هر تب
        /// کلید Dictionary همان Id تب است
        /// </summary>
        //public readonly Dictionary<string, FlowLayoutPanel> flowLayoutPanels = new();
        public readonly Dictionary<string, AnimatedTilePanel> flowLayoutPanels = new();


        public int ItemCount => items.Count;
        public TabItem? SelectedItem => selectedItem;
        public IReadOnlyList<TabItem> Items => items.AsReadOnly();

        public void PerformAddButtonClick()
        {
            btnAdd.PerformClick();
        }


        #region Internal Config

        private const string DefaultNewTabTitle = "تب جدید";
        private const string CloseTabMessage = "آیا این تب بسته شود؟";
        private const string CloseAllTabsMessage = "آیا همه تب‌ها بسته شوند؟";
        private const string ConfirmCaption = "تأیید";

        private const int DefaultControlHeight = 40;
        private const int DefaultControlMinWidth = 150;
        private const int SideButtonColumnWidth = 33;
        private const int SideButtonSize = 30;
        private const int SideButtonCornerRadius = 7;

        private static readonly Color AddButtonBackColor = Color.White;
        private static readonly Color AddButtonForeColor = Color.FromArgb(64, 64, 64);

        private static readonly Color CloseAllButtonBackColor = Color.FromArgb(220, 53, 69);
        private static readonly Color CloseAllButtonForeColor = Color.White;

        #endregion

        public CustomTab()
        {
            Height = DefaultControlHeight;
            MinimumSize = new Size(DefaultControlMinWidth, DefaultControlHeight);

            SetStyle(
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SideButtonColumnWidth));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SideButtonColumnWidth));

            itemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                BackColor = Color.Transparent
            };

            btnAdd = CreateSideButton();
            btnAdd.Paint += BtnAdd_Paint;
            btnAdd.Click += BtnAdd_Click;

            btnCloseAll = CreateSideButton();
            btnCloseAll.Paint += BtnCloseAll_Paint;
            btnCloseAll.Click += BtnCloseAll_Click;

            mainLayout.Controls.Add(itemsPanel, 0, 0);
            mainLayout.Controls.Add(btnAdd, 1, 0);
            mainLayout.Controls.Add(btnCloseAll, 2, 0);

            Controls.Add(mainLayout);
        }



        private Button CreateSideButton()
        {
            Button button = new Button
            {
                Size = new Size(SideButtonSize, SideButtonSize),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 5, 3, 0),
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;

            return button;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            AddButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetTabs(IEnumerable<TabDefinition>? tabDefinitions)
        {
            ClearTabs(raiseClosingEvent: false);

            if (tabDefinitions == null)
                return;

            foreach (TabDefinition definition in tabDefinitions.Where(x => x != null && !string.IsNullOrWhiteSpace(x.Title)))
            {
                string title = definition.Title.Trim();

                string tabId = string.IsNullOrWhiteSpace(definition.Id)
                    ? CreateUniqueTabName()
                    : definition.Id.Trim();

                if (flowLayoutPanels.ContainsKey(tabId))
                    tabId = CreateUniqueTabName(tabId);

                AddNewTab(title, tabId, selectTab: false);
            }

            if (items.Count > 0)
                SelectTab(items[0]);
        }

        public void SetTabs(IEnumerable<string>? titles)
        {
            if (titles == null)
            {
                SetTabs((IEnumerable<TabDefinition>?)null);
                return;
            }

            List<TabDefinition> definitions = titles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select((title, index) => new TabDefinition
                {
                    Id = $"tab-{index + 1}",
                    Title = title.Trim()
                })
                .ToList();

            SetTabs(definitions);
        }

        public TabItem? AddNewTab(string title)
        {
            return AddNewTab(title, null, selectTab: true);
        }

        public TabItem? AddNewTab(string title, string? tabName, bool selectTab = true)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            string uniqueTabName = string.IsNullOrWhiteSpace(tabName)
                ? CreateUniqueTabName()
                : tabName.Trim();

            if (flowLayoutPanels.ContainsKey(uniqueTabName))
                uniqueTabName = CreateUniqueTabName(uniqueTabName);

            TabItem newItem = new TabItem(
                title: title.Trim(),
                tabName: uniqueTabName,
                onSelect: SelectTab,
                onClose: RemoveTab);

            AnimatedTilePanel flowLayoutPanel = new AnimatedTilePanel
            {
                Name = uniqueTabName,
                Dock = DockStyle.Fill,
                Visible = true,
                AutoScroll = true,
                //WrapContents = true,
                BackColor = Color.Transparent
            };

            flowLayoutPanels.Add(uniqueTabName, flowLayoutPanel);

            items.Add(newItem);
            itemsPanel.Controls.Add(newItem);

            ItemAdded?.Invoke(this, new TabEventArgs(newItem));

            if (selectTab)
                SelectTab(newItem);

            return newItem;
        }

        public bool TryGetContentPanel(string tabName, out AnimatedTilePanel? contentPanel)
        {
            return flowLayoutPanels.TryGetValue(tabName, out contentPanel);
        }

        public void ClearTabs(bool raiseClosingEvent = true)
        {
            foreach (TabItem item in items.ToArray())
            {
                if (raiseClosingEvent)
                    ItemClosing?.Invoke(this, new TabEventArgs(item));

                itemsPanel.Controls.Remove(item);
                item.Dispose();
            }

            items.Clear();
            selectedItem = null;

            foreach (AnimatedTilePanel panel in flowLayoutPanels.Values.ToList())
            {
                panel.Parent?.Controls.Remove(panel);
                panel.Dispose();
            }

            flowLayoutPanels.Clear();
        }

        public void SelectTab(TabItem item)
        {
            if (item == null || !items.Contains(item))
                return;

            if (selectedItem == item)
                return;

            if (selectedItem != null)
            {
                selectedItem.IsSelected = false;
                ItemDeselected?.Invoke(this, new TabEventArgs(selectedItem));
            }

            selectedItem = item;
            selectedItem.IsSelected = true;

            ItemSelected?.Invoke(this, new TabEventArgs(item));
            itemsPanel.ScrollControlIntoView(item);
        }

        public void RemoveTab(TabItem item)
        {
            if (item == null || !items.Contains(item))
                return;

            DialogResult result = MessageBox.Show(
                CloseTabMessage,
                ConfirmCaption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            RemoveTabInternal(item, raiseClosingEvent: true);
        }

        private void RemoveTabInternal(TabItem item, bool raiseClosingEvent)
        {
            bool wasSelected = selectedItem == item;

            if (raiseClosingEvent)
                ItemClosing?.Invoke(this, new TabEventArgs(item));

            if (flowLayoutPanels.TryGetValue(item.TabName, out AnimatedTilePanel? panel))
            {
                panel.Parent?.Controls.Remove(panel);
                panel.Dispose();
                flowLayoutPanels.Remove(item.TabName);
            }

            items.Remove(item);
            itemsPanel.Controls.Remove(item);
            item.Dispose();

            if (!wasSelected)
                return;

            selectedItem = null;

            if (items.Count > 0)
                SelectTab(items[^1]);
        }

        private void BtnCloseAll_Click(object? sender, EventArgs e)
        {
            if (items.Count == 0)
                return;

            DialogResult result = MessageBox.Show(
                CloseAllTabsMessage,
                ConfirmCaption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
                ClearTabs(raiseClosingEvent: true);
        }

        private string CreateUniqueTabName(string prefix = "tab")
        {
            prefix = string.IsNullOrWhiteSpace(prefix) ? "tab" : prefix.Trim();

            string tabName;
            do
            {
                tabName = $"{prefix}-{Guid.NewGuid():N}";
            }
            while (flowLayoutPanels.ContainsKey(tabName));

            return tabName;
        }

        private void BtnAdd_Paint(object? sender, PaintEventArgs e)
        {
            DrawRoundButtonBackground(e.Graphics, btnAdd.ClientRectangle, AddButtonBackColor);

            using Pen pen = new Pen(AddButtonForeColor, 3);
            int center = btnAdd.Width / 2;
            const int offset = 8;

            e.Graphics.DrawLine(pen, center - offset, center, center + offset, center);
            e.Graphics.DrawLine(pen, center, center - offset, center, center + offset);
        }

        private void BtnCloseAll_Paint(object? sender, PaintEventArgs e)
        {
            DrawRoundButtonBackground(e.Graphics, btnCloseAll.ClientRectangle, CloseAllButtonBackColor);

            using Pen pen = new Pen(CloseAllButtonForeColor, 3);
            const int padding = 8;

            e.Graphics.DrawLine(
                pen,
                padding,
                padding,
                btnCloseAll.Width - padding,
                btnCloseAll.Height - padding);

            e.Graphics.DrawLine(
                pen,
                btnCloseAll.Width - padding,
                padding,
                padding,
                btnCloseAll.Height - padding);
        }

        private void DrawRoundButtonBackground(Graphics graphics, Rectangle rect, Color backColor)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle drawRect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            using GraphicsPath path = GetRoundedRectanglePath(drawRect, SideButtonCornerRadius);
            using SolidBrush brush = new SolidBrush(backColor);

            graphics.FillPath(brush, path);
        }


        public static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (rect.Width <= 0 || rect.Height <= 0)
                return path;

            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            radius = Math.Max(0, Math.Min(radius, maxRadius));

            if (radius == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearTabs(raiseClosingEvent: false);

                    if (btnAdd != null)
                    {
                        btnAdd.Paint -= BtnAdd_Paint;
                        btnAdd.Click -= BtnAdd_Click;
                        btnAdd.Dispose();
                    }

                    if (btnCloseAll != null)
                    {
                        btnCloseAll.Paint -= BtnCloseAll_Paint;
                        btnCloseAll.Click -= BtnCloseAll_Click;
                        btnCloseAll.Dispose();
                    }

                    itemsPanel?.Dispose();
                    mainLayout?.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }

    public class TabItem : Control
    {
        private string title;
        private bool isSelected;
        private bool isHovered;

        private readonly Action<TabItem> onSelect;
        private readonly Action<TabItem> onClose;

        private Rectangle closeBtnRect;
        private Rectangle textRect;

        private TextBox? editBox;
        private EditClickFilter? clickFilter;

        public string PrivateId { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Code { get; set; } = string.Empty;

        public string TabName { get; }

        public string tabname => TabName;

        #region Internal Config

        private const int HorizontalPadding = 12;
        private const int CloseButtonSizeValue = 18;
        private const int SpacingValue = 8;
        private const int MinTabWidthValue = 100;
        private const int MaxTabWidthValue = 220;
        private const int CornerRadius = 7;

        private static readonly Color SelectedBorderColor = Color.DodgerBlue;
        private static readonly Color NormalBorderColor = Color.Silver;

        private static readonly Color SelectedBackColor = Color.FromArgb(230, 240, 255);
        private static readonly Color HoverBackColor = Color.FromArgb(245, 245, 245);

        private static readonly Color SelectedTextColor = Color.DodgerBlue;
        private static readonly Color HoverTextColor = Color.Black;
        private static readonly Color NormalTextColor = Color.FromArgb(60, 60, 60);

        private static readonly Color SelectedCloseButtonBackColor = Color.FromArgb(220, 53, 69);
        private static readonly Color NormalCloseButtonBackColor = Color.FromArgb(120, 120, 120);
        private static readonly Color CloseButtonForeColor = Color.White;

        #endregion

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get => title;
            set
            {
                title = value ?? string.Empty;
                AutoSizeWidth();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                Invalidate();
            }
        }

        public TabItem(
            string title,
            string tabName,
            Action<TabItem> onSelect,
            Action<TabItem> onClose)
        {

            this.PrivateId = Guid.NewGuid().ToString();

            this.title = title;
            TabName = tabName;
            this.onSelect = onSelect ?? throw new ArgumentNullException(nameof(onSelect));
            this.onClose = onClose ?? throw new ArgumentNullException(nameof(onClose));

            Height = 30;
            Cursor = Cursors.Hand;
            //BackColor = Color.Transparent;
            DoubleBuffered = true;

            SetStyle(
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            AutoSizeWidth();
        }

        private void AutoSizeWidth()
        {
            Size textSize = TextRenderer.MeasureText(title, Font);

            int calculatedWidth =
                HorizontalPadding +
                textSize.Width +
                SpacingValue +
                CloseButtonSizeValue +
                HorizontalPadding;

            Width = Math.Max(MinTabWidthValue, Math.Min(MaxTabWidthValue, calculatedWidth));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (closeBtnRect.Contains(e.Location))
                onClose(this);
            else
                onSelect(this);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            BeginEdit();
        }

        private void BeginEdit()
        {
            if (editBox != null)
                return;

            editBox = new TextBox
            {
                Text = title,
                Font = Font,
                BorderStyle = BorderStyle.None
            };

            int verticalOffset = (Height - editBox.PreferredHeight) / 2;

            editBox.Location = new Point(textRect.X, verticalOffset);
            editBox.Size = new Size(
                Math.Max(1, textRect.Width),
                Math.Max(1, editBox.PreferredHeight));

            editBox.KeyDown += EditBox_KeyDown;
            editBox.LostFocus += EditBox_LostFocus;

            Controls.Add(editBox);

            clickFilter = new EditClickFilter(this);
            WinApp.AddMessageFilter(clickFilter);

            editBox.Focus();
            editBox.SelectAll();
        }

        private void EditBox_LostFocus(object? sender, EventArgs e)
        {
            EndEdit(save: true);
        }

        private void EditBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EndEdit(save: true);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EndEdit(save: false);
                e.SuppressKeyPress = true;
            }
        }

        private void EndEdit(bool save)
        {
            TextBox? textBox = editBox;

            if (textBox == null)
                return;

            editBox = null;

            if (clickFilter != null)
            {
                WinApp.RemoveMessageFilter(clickFilter);
                clickFilter = null;
            }

            if (save && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                title = textBox.Text.Trim();
                AutoSizeWidth();
            }

            Controls.Remove(textBox);
            textBox.Dispose();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            Color backgroundColor = GetBackgroundColor();
            Color borderColor = isSelected ? SelectedBorderColor : NormalBorderColor;
            Color textColor = GetTextColor();

            using (GraphicsPath path = CustomTab.GetRoundedRectanglePath(rect, CornerRadius))
            {
                if (backgroundColor != Color.Transparent)
                {
                    using SolidBrush brush = new SolidBrush(backgroundColor);
                    graphics.FillPath(brush, path);
                }

                using Pen pen = new Pen(borderColor);
                graphics.DrawPath(pen, path);
            }

            textRect = new Rectangle(
                HorizontalPadding,
                0,
                Math.Max(1, Width - (HorizontalPadding * 2 + CloseButtonSizeValue + SpacingValue)),
                Height);

            if (editBox == null)
            {
                TextRenderer.DrawText(
                    graphics,
                    title,
                    Font,
                    textRect,
                    textColor,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.NoPadding);
            }

            closeBtnRect = new Rectangle(
                Width - HorizontalPadding - CloseButtonSizeValue,
                (Height - CloseButtonSizeValue) / 2,
                CloseButtonSizeValue,
                CloseButtonSizeValue);

            DrawCloseButton(graphics);
        }

        private Color GetBackgroundColor()
        {
            if (isSelected)
                return SelectedBackColor;

            if (isHovered)
                return HoverBackColor;

            return Color.Transparent;
        }

        private Color GetTextColor()
        {
            if (isSelected)
                return SelectedTextColor;

            if (isHovered)
                return HoverTextColor;

            return NormalTextColor;
        }

        private void DrawCloseButton(Graphics graphics)
        {
            Color closeBackgroundColor = isSelected
                ? SelectedCloseButtonBackColor
                : NormalCloseButtonBackColor;

            using GraphicsPath path = CustomTab.GetRoundedRectanglePath(closeBtnRect, 4);
            using SolidBrush brush = new SolidBrush(closeBackgroundColor);
            using Pen pen = new Pen(CloseButtonForeColor, 2);

            graphics.FillPath(brush, path);

            const int padding = 4;

            graphics.DrawLine(
                pen,
                closeBtnRect.X + padding,
                closeBtnRect.Y + padding,
                closeBtnRect.Right - padding,
                closeBtnRect.Bottom - padding);

            graphics.DrawLine(
                pen,
                closeBtnRect.Right - padding,
                closeBtnRect.Y + padding,
                closeBtnRect.X + padding,
                closeBtnRect.Bottom - padding);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (clickFilter != null)
                {
                    WinApp.RemoveMessageFilter(clickFilter);
                    clickFilter = null;
                }

                if (editBox != null)
                {
                    editBox.KeyDown -= EditBox_KeyDown;
                    editBox.LostFocus -= EditBox_LostFocus;
                    editBox.Dispose();
                    editBox = null;
                }
            }

            base.Dispose(disposing);
        }

        private sealed class EditClickFilter : IMessageFilter
        {
            private readonly TabItem owner;

            public EditClickFilter(TabItem owner)
            {
                this.owner = owner;
            }

            public bool PreFilterMessage(ref Message m)
            {
                const int WmLeftButtonDown = 0x0201;
                const int WmLeftButtonDoubleClick = 0x0203;

                if (m.Msg == WmLeftButtonDown || m.Msg == WmLeftButtonDoubleClick)
                {
                    owner.EndEdit(save: true);
                }

                return false;
            }
        }
    }

    public class TabEventArgs : EventArgs
    {
        public TabItem Item { get; }

        public TabEventArgs(TabItem item)
        {
            Item = item;
        }
    }
}

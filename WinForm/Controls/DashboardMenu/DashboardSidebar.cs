using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using WinForm.Controls.DashboardMenu.Entities;
using WinForm.Controls.DashboardMenu.Events;
using WinForm.Controls.DashboardMenu.Themes;

namespace WinForm.Controls.DashboardMenu
{
    [DefaultEvent(nameof(MenuItemClicked))]
    public class DashboardSidebar : UserControl
    {
        private readonly Panel _headerPanel;
        private readonly PictureBox _logoPictureBox;
        private readonly PictureBox _profilePictureBox;
        private readonly Label _userNameLabel;
        private readonly Label _userRoleLabel;
        private readonly FlowLayoutPanel _menuPanel;
        private readonly Label _footerLabel;

        private DashboardMenuModel _menu;
        private DashboardSidebarTheme _theme;
        private bool _collapsed;

        // میزان شعاع گردی لبه‌ها برای کادر هر آیتم منو
        private const int ItemBorderRadius = 8;

        public DashboardSidebar()
        {
            DoubleBuffered = true;
            Width = 280;
            MinimumSize = new Size(72, 250);

            _theme = new DashboardSidebarTheme();
            _menu = new DashboardMenuModel();

            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180
            };

            _logoPictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(24, 18),
                Size = new Size(220, 42)
            };

            _profilePictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(186, 78),
                Size = new Size(54, 54)
            };

            _userNameLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(24, 88),
                Size = new Size(150, 22)
            };

            _userRoleLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(24, 112),
                Size = new Size(150, 20)
            };

            _footerLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _menuPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10, 8, 10, 8)
            };

            _headerPanel.Controls.Add(_logoPictureBox);
            _headerPanel.Controls.Add(_profilePictureBox);
            _headerPanel.Controls.Add(_userNameLabel);
            _headerPanel.Controls.Add(_userRoleLabel);

            Controls.Add(_menuPanel);
            Controls.Add(_footerLabel);
            Controls.Add(_headerPanel);

            ApplyTheme();
            RebuildMenu();
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DashboardSidebarTheme Theme
        {
            get { return _theme; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DashboardMenuModel Menu
        {
            get { return _menu; }
            set
            {
                _menu = value ?? new DashboardMenuModel();
                RebuildMenu();
            }
        }

        [Browsable(true)]
        [Category("Dashboard")]
        [Description("لوگوی بالای منوی کناری.")]
        [DefaultValue(null)]
        public Image Logo
        {
            get { return _menu.Logo; }
            set
            {
                _menu.Logo = value;
                _logoPictureBox.Image = value;
            }
        }

        [Browsable(true)]
        [Category("Dashboard")]
        [Description("متن بخش پایین کنترل.")]
        [DefaultValue("")]
        public string FooterText
        {
            get { return _menu.FooterText; }
            set
            {
                _menu.FooterText = value;
                _footerLabel.Text = value;
            }
        }

        [Browsable(true)]
        [Category("Dashboard")]
        [DefaultValue("")]
        public string UserName
        {
            get { return _menu.UserName; }
            set
            {
                _menu.UserName = value;
                _userNameLabel.Text = value;
            }
        }

        [Browsable(true)]
        [Category("Dashboard")]
        [DefaultValue("")]
        public string UserRole
        {
            get { return _menu.UserRole; }
            set
            {
                _menu.UserRole = value;
                _userRoleLabel.Text = value;
                _footerLabel.ForeColor = Theme.UserRoleColor;
            }
        }

        [Browsable(true)]
        [Category("Dashboard")]
        [DefaultValue(null)]
        public Image ProfilePicture
        {
            get { return _menu.ProfilePicture; }
            set
            {
                _menu.ProfilePicture = value;
                _profilePictureBox.Image = value;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                if (_collapsed == value)
                    return;

                _collapsed = value;
                Width = _collapsed ? 72 : 280;
                RebuildMenu();
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            AdjustHeaderLayout();
            RebuildMenu();
        }

        private void AdjustHeaderLayout()
        {
            if (RightToLeft == RightToLeft.Yes)
            {
                _profilePictureBox.Location = new Point(186, 78);
                _userNameLabel.Location = new Point(24, 88);
                _userNameLabel.TextAlign = ContentAlignment.MiddleRight;
                _userRoleLabel.Location = new Point(24, 112);
                _userRoleLabel.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                _profilePictureBox.Location = new Point(24, 78);
                _userNameLabel.Location = new Point(94, 88);
                _userNameLabel.TextAlign = ContentAlignment.MiddleLeft;
                _userRoleLabel.Location = new Point(94, 112);
                _userRoleLabel.TextAlign = ContentAlignment.MiddleLeft;
            }
        }

        public event EventHandler<DashboardMenuItemClickedEventArgs> MenuItemClicked;

        public void ToggleCollapsed()
        {
            Collapsed = !Collapsed;
        }

        public void RebuildMenu()
        {
            _menuPanel.SuspendLayout();

            try
            {
                foreach (Control control in _menuPanel.Controls)
                    control.Dispose();

                _menuPanel.Controls.Clear();

                _logoPictureBox.Image = _menu.Logo;
                _profilePictureBox.Image = _menu.ProfilePicture;
                _userNameLabel.Text = _menu.UserName;
                _userRoleLabel.Text = _menu.UserRole;
                _footerLabel.Text = _menu.FooterText;

                AdjustHeaderLayout();

                foreach (DashboardMenuGroup group in _menu.Groups)
                {
                    if (group.Visible)
                        AddGroup(group);
                }
            }
            finally
            {
                _menuPanel.ResumeLayout(true);
            }
        }

        private void ApplyTheme()
        {
            BackColor = Theme.BackgroundColor;
            _headerPanel.BackColor = Theme.BackgroundColor;
            _menuPanel.BackColor = Theme.BackgroundColor;
            _footerLabel.BackColor = Theme.BackgroundColor;

            _userNameLabel.ForeColor = Theme.UserNameColor;
            _userNameLabel.Font = Theme.UserNameFont;

            _userRoleLabel.ForeColor = Theme.UserRoleColor;
            _userRoleLabel.Font = Theme.UserRoleFont;

            _footerLabel.ForeColor = Theme.UserRoleColor;
            _footerLabel.Font = Theme.UserRoleFont;
        }

        private void AddGroup(DashboardMenuGroup group)
        {
            if (!Collapsed && !string.IsNullOrWhiteSpace(group.Title))
            {
                Label groupLabel = new Label
                {
                    Width = Math.Max(1, _menuPanel.ClientSize.Width - 24),
                    Height = 32,
                    Padding = new Padding(8, 8, 8, 0),
                    Text = group.Title.ToUpperInvariant(),
                    Font = Theme.GroupFont,
                    ForeColor = group.TitleColor == Color.Empty
                        ? Theme.GroupTitleColor
                        : group.TitleColor,
                    TextAlign = RightToLeft == RightToLeft.Yes ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                    RightToLeft = RightToLeft.No
                };

                _menuPanel.Controls.Add(groupLabel);
            }

            foreach (DashboardMenuItem item in group.Items)
            {
                if (item.Visible)
                    _menuPanel.Controls.Add(CreateMenuItemControl(item));
            }
        }

        private Control CreateMenuItemControl(DashboardMenuItem item)
        {
            RoundedPanel itemPanel = new RoundedPanel
            {
                Width = Collapsed ? 48 : Math.Max(1, _menuPanel.ClientSize.Width - 24),
                Height = 44,
                Margin = new Padding(0, 2, 0, 2),
                Cursor = Cursors.Hand,
                Tag = item,
                RightToLeft = RightToLeft.No,
                BorderRadius = ItemBorderRadius
            };

            bool isRtl = this.RightToLeft == RightToLeft.Yes;

            PictureBox iconPictureBox = new PictureBox
            {
                Dock = isRtl ? DockStyle.Right : DockStyle.Left,
                Width = 44,
                Height = 44,
                Image = item.Icon,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Cursor = Cursors.Hand,
                Tag = item,
                BackColor = Color.Transparent
            };

            Label titleLabel = new Label
            {
                Width = itemPanel.Width - iconPictureBox.Width,
                Height = 44,
                Text = Collapsed ? string.Empty : item.Title,
                TextAlign = isRtl ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                Padding = isRtl ? new Padding(8, 0, 4, 0) : new Padding(4, 0, 8, 0),
                Font = Theme.ItemFont,
                Cursor = Cursors.Hand,
                Tag = item,
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            if (this.RightToLeft == RightToLeft.Yes)
            {
                itemPanel.Controls.Add(titleLabel);
                itemPanel.Controls.Add(iconPictureBox);
            }
            else
            {
                itemPanel.Controls.Add(iconPictureBox);
                itemPanel.Controls.Add(titleLabel);
            }

            ApplyMenuItemVisualState(itemPanel, item, isHover: false);

            itemPanel.Click += MenuItemControl_Click;
            iconPictureBox.Click += MenuItemControl_Click;
            titleLabel.Click += MenuItemControl_Click;

            itemPanel.MouseEnter += MenuItemControl_MouseEnter;
            iconPictureBox.MouseEnter += MenuItemControl_MouseEnter;
            titleLabel.MouseEnter += MenuItemControl_MouseEnter;

            itemPanel.MouseLeave += MenuItemControl_MouseLeave;
            iconPictureBox.MouseLeave += MenuItemControl_MouseLeave;
            titleLabel.MouseLeave += MenuItemControl_MouseLeave;

            return itemPanel;
        }

        private void MenuItemControl_MouseEnter(object sender, EventArgs e)
        {
            RoundedPanel itemPanel = GetItemPanelFromSender(sender);
            if (itemPanel == null) return;

            DashboardMenuItem item = itemPanel.Tag as DashboardMenuItem;
            if (item == null) return;

            ApplyMenuItemVisualState(itemPanel, item, isHover: true);
        }

        private void MenuItemControl_MouseLeave(object sender, EventArgs e)
        {
            RoundedPanel itemPanel = GetItemPanelFromSender(sender);
            if (itemPanel == null) return;

            if (itemPanel.ClientRectangle.Contains(itemPanel.PointToClient(Cursor.Position)))
                return;

            DashboardMenuItem item = itemPanel.Tag as DashboardMenuItem;
            if (item == null) return;

            ApplyMenuItemVisualState(itemPanel, item, isHover: false);
        }

        private RoundedPanel GetItemPanelFromSender(object sender)
        {
            Control c = sender as Control;
            if (c == null) return null;

            if (c is RoundedPanel p)
                return p;

            return c.Parent as RoundedPanel;
        }

        private void ApplyMenuItemVisualState(RoundedPanel itemPanel, DashboardMenuItem item, bool isHover)
        {
            bool selected = item.IsSelected;

            Color selectedTextColor = Color.FromArgb(28, 39, 70);
            Color foreColor = item.TitleColor != Color.Empty
                ? item.TitleColor
                : selected
                    ? selectedTextColor
                    : Theme.ItemTextColor;

            Color selectedBgColor = Color.White;
            Color normalBackColor = item.BackgroundColor != Color.Empty
                ? item.BackgroundColor
                : selected
                    ? selectedBgColor
                    : Theme.BackgroundColor;

            Color hoverBackColor = selected
                ? selectedBgColor
                : Blend(normalBackColor, Theme.SelectedItemBackgroundColor, 1f);

            Color backColor = isHover ? hoverBackColor : normalBackColor;

            itemPanel.BackColor = backColor;

            foreach (Control child in itemPanel.Controls)
            {
                child.BackColor = Color.Transparent;

                if (child is Label lbl)
                {
                    lbl.ForeColor = foreColor;
                }
                else if (child is PictureBox pic)
                {
                    // اگر آیتم انتخاب شده باشد، رنگ آیکون را متناسب با رنگ متن تغییر می‌دهیم.
                    if (selected && item.Icon != null)
                    {
                        pic.Image = ColorizeImage(item.Icon, foreColor);
                    }
                    else
                    {
                        pic.Image = item.Icon;
                    }
                }
            }
        }

        /// <summary>
        /// متد کمکی برای رنگ‌آمیزی پویای آیکون‌های تک‌رنگ (سفید) بر اساس رنگ متن انتخابی
        /// </summary>
        private Image ColorizeImage(Image originalImage, Color color)
        {
            if (originalImage == null) return null;

            Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // ایجاد ماتریس رنگ جهت جایگزینی رنگ‌های عکس
                float r = color.R / 255f;
                float gColor = color.G / 255f;
                float b = color.B / 255f;

                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 1, 0}, // نگه داشتن ترنسپرنسی (Alpha) اصلی آیکون
                    new float[] {r, gColor, b, 0, 1} // تنظیم مقادیر رنگی جدید
                });

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(originalImage,
                        new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                        0, 0, originalImage.Width, originalImage.Height,
                        GraphicsUnit.Pixel, attributes);
                }
            }
            return bmp;
        }

        private static Color Blend(Color baseColor, Color overlayColor, float overlayAmount)
        {
            if (overlayAmount < 0f) overlayAmount = 0f;
            if (overlayAmount > 1f) overlayAmount = 1f;

            int r = (int)(baseColor.R + (overlayColor.R - baseColor.R) * overlayAmount);
            int g = (int)(baseColor.G + (overlayColor.G - baseColor.G) * overlayAmount);
            int b = (int)(baseColor.B + (overlayColor.B - baseColor.B) * overlayAmount);

            return Color.FromArgb(baseColor.A, r, g, b);
        }

        private void MenuItemControl_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            DashboardMenuItem item = control == null
                ? null
                : control.Tag as DashboardMenuItem;

            if (item == null)
                return;

            foreach (DashboardMenuGroup group in _menu.Groups)
            {
                foreach (DashboardMenuItem menuItem in group.Items)
                {
                    menuItem.IsSelected = (menuItem == item);
                }
            }

            foreach (Control ctrl in _menuPanel.Controls)
            {
                if (ctrl is RoundedPanel itemPanel && itemPanel.Tag is DashboardMenuItem menuItem)
                {
                    ApplyMenuItemVisualState(itemPanel, menuItem, isHover: false);
                }
            }

            MenuItemClicked?.Invoke(
                this,
                new DashboardMenuItemClickedEventArgs(item));
        }
    }

    /// <summary>
    /// پانل سفارشی با قابلیت رسم لبه‌های گرد با کیفیت بالا (Anti-Alias) بدون نیاز به بریدن سخت Region
    /// </summary>
    internal class RoundedPanel : Panel
    {
        private int _borderRadius = 8;

        [Category("Appearance")]
        [Description("شعاع لبه‌های گرد پانل")]
        [DefaultValue(8)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (value >= 0)
                {
                    _borderRadius = value;
                    Invalidate();
                }
            }
        }

        public RoundedPanel()
        {
            DoubleBuffered = true;
            // فعال‌سازی ترسیم دستی و بهینه‌سازی رویداد Paint
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // عدم صدا زدن base.OnPaint برای مدیریت کامل ترسیم پس‌زمینه جهت رفع دندانه‌دندانه شدن لبه‌ها
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // ابتدا رنگ پس‌زمینه کنترل والد ترسیم می‌شود تا لبه‌ها با رنگ پشت تلفیق و نرم شوند
            if (Parent != null)
            {
                using (SolidBrush parentBrush = new SolidBrush(Parent.BackColor))
                {
                    g.FillRectangle(parentBrush, ClientRectangle);
                }
            }

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (_borderRadius > 0)
            {
                using (GraphicsPath path = GetRoundedPath(rect, _borderRadius))
                {
                    using (SolidBrush brush = new SolidBrush(BackColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float size = radius * 2F;

            if (size <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, size, size, 180, 90);
            path.AddArc(rect.Right - size, rect.Y, size, size, 270, 90);
            path.AddArc(rect.Right - size, rect.Bottom - size, size, size, 0, 90);
            path.AddArc(rect.X, rect.Bottom - size, size, size, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

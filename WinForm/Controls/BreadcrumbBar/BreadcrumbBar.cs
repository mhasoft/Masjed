using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace WinForm.Controls.BreadcrumbBar
{
    public sealed class BreadcrumbBarItems
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public sealed class BreadcrumbClickedEventArgs : EventArgs
    {
        public int Index { get; }
        public string Code { get; }
        public string Title { get; }
        public string FullPathUpToHere { get; }

        public BreadcrumbClickedEventArgs(int index, string code, string title, string fullPathUpToHere)
        {
            Index = index;
            Code = code ?? string.Empty;
            Title = title ?? string.Empty;
            FullPathUpToHere = fullPathUpToHere ?? string.Empty;
        }
    }

    public sealed class BreadcrumbBar : Control
    {
        private static readonly Color DefaultCrumbColor = Color.FromArgb(0, 102, 204);
        private static readonly Color DefaultCrumbHoverColor = Color.FromArgb(0, 76, 153);
        private static readonly Color DefaultSeparatorColor = Color.FromArgb(40, 40, 40);
        private static readonly Color DefaultHoverBackColor = Color.FromArgb(235, 243, 255);
        private static readonly Color DefaultBorderColor = Color.FromArgb(220, 225, 232);
        private static readonly Color DefaultSurfaceColor = Color.White;

        private const int DefaultCornerRadius = 10;
        private const int DefaultItemHorizontalPadding = 10;
        private const int DefaultItemVerticalPadding = 6;
        private const char DefaultSeparator = '/';
        private const bool DefaultUnderlineOnHover = true;

        private readonly List<BreadcrumbBarItems> _items = new List<BreadcrumbBarItems>();
        private readonly List<Rectangle> _crumbRects = new List<Rectangle>();
        private readonly List<Rectangle> _sepRects = new List<Rectangle>();

        private Color _crumbColor = DefaultCrumbColor;
        private Color _crumbHoverColor = DefaultCrumbHoverColor;
        private Color _separatorColor = DefaultSeparatorColor;
        private Color _hoverBackColor = DefaultHoverBackColor;
        private Color _borderColor = DefaultBorderColor;
        private Color _surfaceColor = DefaultSurfaceColor;

        private int _cornerRadius = DefaultCornerRadius;
        private int _itemHorizontalPadding = DefaultItemHorizontalPadding;
        private int _itemVerticalPadding = DefaultItemVerticalPadding;
        private char _separator = DefaultSeparator;
        private bool _underlineOnHover = DefaultUnderlineOnHover;

        private int _hoverIndex = -1;
        private int _pressedIndex = -1;

        [Category("Appearance")]
        public Color CrumbColor
        {
            get => _crumbColor;
            set
            {
                if (_crumbColor == value) return;
                _crumbColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color CrumbHoverColor
        {
            get => _crumbHoverColor;
            set
            {
                if (_crumbHoverColor == value) return;
                _crumbHoverColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color SeparatorColor
        {
            get => _separatorColor;
            set
            {
                if (_separatorColor == value) return;
                _separatorColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color HoverBackColor
        {
            get => _hoverBackColor;
            set
            {
                if (_hoverBackColor == value) return;
                _hoverBackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color SurfaceColor
        {
            get => _surfaceColor;
            set
            {
                if (_surfaceColor == value) return;
                _surfaceColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(DefaultCornerRadius)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                int newValue = Math.Max(0, value);
                if (_cornerRadius == newValue) return;
                _cornerRadius = newValue;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(DefaultItemHorizontalPadding)]
        public int ItemHorizontalPadding
        {
            get => _itemHorizontalPadding;
            set
            {
                int newValue = Math.Max(0, value);
                if (_itemHorizontalPadding == newValue) return;
                _itemHorizontalPadding = newValue;
                RebuildLayout();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(DefaultItemVerticalPadding)]
        public int ItemVerticalPadding
        {
            get => _itemVerticalPadding;
            set
            {
                int newValue = Math.Max(0, value);
                if (_itemVerticalPadding == newValue) return;
                _itemVerticalPadding = newValue;
                RebuildLayout();
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(DefaultSeparator)]
        public char Separator
        {
            get => _separator;
            set
            {
                if (_separator == value) return;
                _separator = value;
                RebuildLayout();
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(DefaultUnderlineOnHover)]
        public bool UnderlineOnHover
        {
            get => _underlineOnHover;
            set
            {
                if (_underlineOnHover == value) return;
                _underlineOnHover = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<BreadcrumbBarItems> Items => _items.AsReadOnly();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BreadcrumbBarItems? SelectedItem { get; private set; }

        public event EventHandler<BreadcrumbClickedEventArgs>? CrumbClicked;

        public BreadcrumbBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            DoubleBuffered = true;
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(10, 6, 10, 6);
            Height = 36;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.Yes;
        }

        public void SetPath(string path)
        {
            _items.Clear();
            ResetMouseState();

            if (!string.IsNullOrWhiteSpace(path))
            {
                string normalized = path
                    .Replace('\\', _separator)
                    .Replace('/', _separator);

                int index = 0;
                foreach (string text in normalized
                    .Split(new[] { _separator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0))
                {
                    _items.Add(new BreadcrumbBarItems
                    {
                        Code = index.ToString(),
                        Title = text
                    });

                    index++;
                }
            }

            RebuildLayout();
            Invalidate();
        }

        public void SetPath(IEnumerable<BreadcrumbBarItems> items)
        {
            _items.Clear();
            ResetMouseState();

            if (items != null)
            {
                _items.AddRange(
                    items
                        .Where(x => x != null)
                        .Select(x => new BreadcrumbBarItems
                        {
                            Code = (x.Code ?? string.Empty).Trim(),
                            Title = (x.Title ?? string.Empty).Trim()
                        })
                        .Where(x => x.Title.Length > 0));
            }

            RebuildLayout();
            Invalidate();
        }

        public void SetItems(IEnumerable<BreadcrumbBarItems> items)
        {
            SetPath(items);
        }

        public void SetCrumbs(IEnumerable<string> crumbs)
        {
            _items.Clear();
            ResetMouseState();

            if (crumbs != null)
            {
                int index = 0;
                foreach (string title in crumbs.Select(x => (x ?? string.Empty).Trim()).Where(x => x.Length > 0))
                {
                    _items.Add(new BreadcrumbBarItems
                    {
                        Code = index.ToString(),
                        Title = title
                    });
                    index++;
                }
            }

            RebuildLayout();
            Invalidate();
        }

        public void ResetCrumbColor() => CrumbColor = DefaultCrumbColor;
        public bool ShouldSerializeCrumbColor() => CrumbColor != DefaultCrumbColor;

        public void ResetCrumbHoverColor() => CrumbHoverColor = DefaultCrumbHoverColor;
        public bool ShouldSerializeCrumbHoverColor() => CrumbHoverColor != DefaultCrumbHoverColor;

        public void ResetSeparatorColor() => SeparatorColor = DefaultSeparatorColor;
        public bool ShouldSerializeSeparatorColor() => SeparatorColor != DefaultSeparatorColor;

        public void ResetHoverBackColor() => HoverBackColor = DefaultHoverBackColor;
        public bool ShouldSerializeHoverBackColor() => HoverBackColor != DefaultHoverBackColor;

        public void ResetBorderColor() => BorderColor = DefaultBorderColor;
        public bool ShouldSerializeBorderColor() => BorderColor != DefaultBorderColor;

        public void ResetSurfaceColor() => SurfaceColor = DefaultSurfaceColor;
        public bool ShouldSerializeSurfaceColor() => SurfaceColor != DefaultSurfaceColor;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            RebuildLayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RebuildLayout();
            Invalidate();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            RebuildLayout();
            Invalidate();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            RebuildLayout();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RebuildLayout();
            Invalidate();
        }

        private void ResetMouseState()
        {
            _hoverIndex = -1;
            _pressedIndex = -1;
            Cursor = Cursors.Default;
            SelectedItem = null;
        }

        private void RebuildLayout()
        {
            _crumbRects.Clear();
            _sepRects.Clear();

            if (_items.Count == 0 || Width <= 0 || Height <= 0)
                return;

            using (Graphics g = CreateGraphics())
            {
                int baselineHeight = Math.Max(Font.Height, 16);
                int itemHeight = baselineHeight + (_itemVerticalPadding * 2);
                int availableH = Math.Max(Height - Padding.Vertical, itemHeight);
                int drawY = Padding.Top + ((availableH - itemHeight) / 2);

                string sepText = " " + _separator + " ";
                Size sepSize = TextRenderer.MeasureText(
                    g,
                    sepText,
                    Font,
                    Size.Empty,
                    TextFormatFlags.NoPadding);

                bool isRtl = RightToLeft == RightToLeft.Yes;

                if (isRtl)
                {
                    int x = Width - Padding.Right;

                    for (int i = 0; i < _items.Count; i++)
                    {
                        Size textSize = TextRenderer.MeasureText(
                            g,
                            _items[i].Title,
                            Font,
                            Size.Empty,
                            TextFormatFlags.NoPadding);

                        int width = textSize.Width + (_itemHorizontalPadding * 2);
                        x -= width;
                        _crumbRects.Add(new Rectangle(x, drawY, width, itemHeight));

                        if (i < _items.Count - 1)
                        {
                            x -= sepSize.Width;
                            _sepRects.Add(new Rectangle(x, drawY, sepSize.Width, itemHeight));
                        }
                    }
                }
                else
                {
                    int x = Padding.Left;

                    for (int i = 0; i < _items.Count; i++)
                    {
                        Size textSize = TextRenderer.MeasureText(
                            g,
                            _items[i].Title,
                            Font,
                            Size.Empty,
                            TextFormatFlags.NoPadding);

                        int width = textSize.Width + (_itemHorizontalPadding * 2);
                        _crumbRects.Add(new Rectangle(x, drawY, width, itemHeight));
                        x += width;

                        if (i < _items.Count - 1)
                        {
                            _sepRects.Add(new Rectangle(x, drawY, sepSize.Width, itemHeight));
                            x += sepSize.Width;
                        }
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle surface = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = RoundedRect(surface, _cornerRadius))
            using (SolidBrush brush = new SolidBrush(_surfaceColor))
            using (Pen pen = new Pen(_borderColor))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            if (_items.Count == 0)
                return;

            bool isRtl = RightToLeft == RightToLeft.Yes;
            TextFormatFlags textFlags = TextFormatFlags.NoPadding |
                                        TextFormatFlags.VerticalCenter |
                                        TextFormatFlags.EndEllipsis;

            if (isRtl)
                textFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;

            for (int i = 0; i < _items.Count; i++)
            {
                if (i >= _crumbRects.Count)
                    continue;

                Rectangle itemRect = _crumbRects[i];
                bool isHover = i == _hoverIndex;
                bool isPressed = i == _pressedIndex;

                if ((isHover || isPressed) && !itemRect.IsEmpty)
                {
                    Rectangle hoverRect = Rectangle.Inflate(itemRect, -1, -3);

                    using (GraphicsPath hoverPath = RoundedRect(hoverRect, 8))
                    using (SolidBrush hoverBrush = new SolidBrush(_hoverBackColor))
                    {
                        g.FillPath(hoverBrush, hoverPath);
                    }
                }

                Color textColor = isHover ? _crumbHoverColor : _crumbColor;
                Rectangle textRect = Rectangle.Inflate(itemRect, -_itemHorizontalPadding, 0);

                TextRenderer.DrawText(
                    g,
                    _items[i].Title,
                    Font,
                    textRect,
                    textColor,
                    textFlags);

                if (_underlineOnHover && isHover && !itemRect.IsEmpty)
                {
                    Size textSize = TextRenderer.MeasureText(
                        g,
                        _items[i].Title,
                        Font,
                        Size.Empty,
                        TextFormatFlags.NoPadding);

                    int underlineY = itemRect.Bottom - 7;
                    int underlineX1;
                    int underlineX2;

                    if (isRtl)
                    {
                        underlineX2 = textRect.Right;
                        underlineX1 = Math.Max(textRect.Right - textSize.Width, textRect.Left);
                    }
                    else
                    {
                        underlineX1 = textRect.Left;
                        underlineX2 = Math.Min(textRect.Left + textSize.Width, textRect.Right);
                    }

                    using (Pen underlinePen = new Pen(textColor, 2))
                    {
                        g.DrawLine(underlinePen, underlineX1, underlineY, underlineX2, underlineY);
                    }
                }

                if (i < _items.Count - 1 && i < _sepRects.Count)
                {
                    TextRenderer.DrawText(
                        g,
                        " " + _separator + " ",
                        Font,
                        _sepRects[i],
                        _separatorColor,
                        textFlags);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = HitTestCrumb(e.Location);
            if (index == _hoverIndex)
                return;

            _hoverIndex = index;
            Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ResetMouseState();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            _pressedIndex = HitTestCrumb(e.Location);
            if (_pressedIndex >= 0)
                Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button != MouseButtons.Left)
                return;

            int upIndex = HitTestCrumb(e.Location);
            int clickedIndex = (_pressedIndex >= 0 && _pressedIndex == upIndex) ? upIndex : -1;

            _pressedIndex = -1;
            Invalidate();

            if (clickedIndex < 0)
                return;

            BreadcrumbBarItems item = _items[clickedIndex];
            SelectedItem = item;

            string fullPath = string.Join(
                " " + _separator + " ",
                _items.Take(clickedIndex + 1).Select(x => x.Title));

            CrumbClicked?.Invoke(
                this,
                new BreadcrumbClickedEventArgs(
                    clickedIndex,
                    item.Code,
                    item.Title,
                    fullPath));
        }

        private int HitTestCrumb(Point point)
        {
            for (int i = 0; i < _crumbRects.Count; i++)
            {
                if (_crumbRects[i].Contains(point))
                    return i;
            }

            return -1;
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (rect.Width <= 0 || rect.Height <= 0)
                return path;

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int effectiveRadius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            if (effectiveRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = effectiveRadius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}

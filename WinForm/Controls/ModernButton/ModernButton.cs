using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace WinForm.Controls.ModernButton
{
    public enum FontAwesomeIcon
    {
        None = 0,
        Plus,
        Minus,
        Check,
        Times,
        Trash,
        Edit,
        Save,
        Search,
        Home,
        User,
        Cog,
        InfoCircle,
        ExclamationTriangle,
        ArrowRight,
        ArrowLeft,
        ChevronRight,
        ChevronLeft,
        Calendar,
        Clock,
        Print,
        FileAlt,
        Database,
        Lock,
        Unlock,
        Key,
        Sync,
        PowerOff,
        Envelope,
        Phone,
        MapMarkerAlt,
        Download,
        Upload
    }

    [DefaultEvent("Click")]
    public class ModernButton : Control
    {
        private enum ButtonVisualState
        {
            Normal,
            Hover,
            Pressed,
            Disabled
        }

        private ButtonVisualState _state = ButtonVisualState.Normal;
        private bool _isMouseDown;
        private bool _isFocused;

        #region Fields

        private int _borderRadius = 14;
        private int _borderThickness = 1;
        private int _iconSize = 12;
        private int _contentSpacing = 8;
        private int _horizontalPadding = 12;

        private Color _borderColor = Color.FromArgb(72, 122, 190);
        private Color _backColor = Color.White;
        private Color _foreColor = Color.FromArgb(72, 122, 190);

        private Color _hoverBackColor = Color.FromArgb(240, 245, 255);
        private Color _hoverBorderColor = Color.FromArgb(40, 90, 160);
        private Color _hoverForeColor = Color.FromArgb(40, 90, 160);

        private Color _pressedBackColor = Color.FromArgb(220, 230, 250);
        private Color _pressedBorderColor = Color.FromArgb(20, 70, 140);
        private Color _pressedForeColor = Color.FromArgb(20, 70, 140);

        private Color _disabledBackColor = Color.FromArgb(245, 245, 245);
        private Color _disabledBorderColor = Color.FromArgb(210, 210, 210);
        private Color _disabledForeColor = Color.FromArgb(160, 160, 160);

        private Color _iconColor = Color.FromArgb(0, 180, 0);
        private Color _hoverIconColor = Color.FromArgb(0, 150, 0);
        private Color _pressedIconColor = Color.FromArgb(0, 120, 0);
        private Color _disabledIconColor = Color.FromArgb(170, 170, 170);

        private FontAwesomeIcon _buttonIcon = FontAwesomeIcon.None;
        private IconChar _iconChar = IconChar.None;

        #endregion

        public ModernButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.Selectable,
                true);

            base.BackColor = Color.Transparent;
            base.ForeColor = _foreColor;
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            Size = new Size(180, 45);
            Cursor = Cursors.Hand;
            TabStop = true;
            Text = "Modern Button"; // مقدار پیش‌فرض اولیه
        }

        #region Designer Visible Properties

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Invalidate(); // لرزش‌گیری و رسم مجدد به محض تغییر متن در طراح یا کد
            }
        }

        [Category("Appearance - Custom")]
        [DefaultValue(14)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (value < 0) value = 0;
                _borderRadius = value;
                Invalidate();
            }
        }

        [Category("Appearance - Custom")]
        [DefaultValue(1)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (value < 0) value = 0;
                _borderThickness = value;
                Invalidate();
            }
        }

        [Category("Appearance - Custom")]
        [DefaultValue(12)]
        public int HorizontalPadding
        {
            get => _horizontalPadding;
            set
            {
                if (value < 0) value = 0;
                _horizontalPadding = value;
                Invalidate();
            }
        }

        [Category("Appearance - Custom")]
        [DefaultValue(8)]
        public int ContentSpacing
        {
            get => _contentSpacing;
            set
            {
                if (value < 0) value = 0;
                _contentSpacing = value;
                Invalidate();
            }
        }

        [Category("Appearance - Custom Colors")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeBorderColor() => _borderColor != Color.FromArgb(72, 122, 190);

        [Category("Appearance - Custom Colors")]
        public Color ButtonBackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeButtonBackColor() => _backColor != Color.White;

        [Category("Appearance - Custom Colors")]
        public override Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                base.ForeColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeForeColor() => _foreColor != Color.FromArgb(72, 122, 190);

        [Category("Appearance - Custom Colors")]
        public Color HoverBackColor
        {
            get => _hoverBackColor;
            set
            {
                _hoverBackColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeHoverBackColor() => _hoverBackColor != Color.FromArgb(240, 245, 255);

        [Category("Appearance - Custom Colors")]
        public Color HoverBorderColor
        {
            get => _hoverBorderColor;
            set
            {
                _hoverBorderColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeHoverBorderColor() => _hoverBorderColor != Color.FromArgb(40, 90, 160);

        [Category("Appearance - Custom Colors")]
        public Color HoverForeColor
        {
            get => _hoverForeColor;
            set
            {
                _hoverForeColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeHoverForeColor() => _hoverForeColor != Color.FromArgb(40, 90, 160);

        [Category("Appearance - Custom Colors")]
        public Color PressedBackColor
        {
            get => _pressedBackColor;
            set
            {
                _pressedBackColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializePressedBackColor() => _pressedBackColor != Color.FromArgb(220, 230, 250);

        [Category("Appearance - Custom Colors")]
        public Color PressedBorderColor
        {
            get => _pressedBorderColor;
            set
            {
                _pressedBorderColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializePressedBorderColor() => _pressedBorderColor != Color.FromArgb(20, 70, 140);

        [Category("Appearance - Custom Colors")]
        public Color PressedForeColor
        {
            get => _pressedForeColor;
            set
            {
                _pressedForeColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializePressedForeColor() => _pressedForeColor != Color.FromArgb(20, 70, 140);

        [Category("Appearance - Custom Colors")]
        public Color DisabledBackColor
        {
            get => _disabledBackColor;
            set
            {
                _disabledBackColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeDisabledBackColor() => _disabledBackColor != Color.FromArgb(245, 245, 245);

        [Category("Appearance - Custom Colors")]
        public Color DisabledBorderColor
        {
            get => _disabledBorderColor;
            set
            {
                _disabledBorderColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeDisabledBorderColor() => _disabledBorderColor != Color.FromArgb(210, 210, 210);

        [Category("Appearance - Custom Colors")]
        public Color DisabledForeColor
        {
            get => _disabledForeColor;
            set
            {
                _disabledForeColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeDisabledForeColor() => _disabledForeColor != Color.FromArgb(160, 160, 160);

        [Category("Appearance - Custom Icon")]
        public Color IconColor
        {
            get => _iconColor;
            set
            {
                _iconColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeIconColor() => _iconColor != Color.FromArgb(0, 180, 0);

        [Category("Appearance - Custom Icon")]
        public Color HoverIconColor
        {
            get => _hoverIconColor;
            set
            {
                _hoverIconColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeHoverIconColor() => _hoverIconColor != Color.FromArgb(0, 150, 0);

        [Category("Appearance - Custom Icon")]
        public Color PressedIconColor
        {
            get => _pressedIconColor;
            set
            {
                _pressedIconColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializePressedIconColor() => _pressedIconColor != Color.FromArgb(0, 120, 0);

        [Category("Appearance - Custom Icon")]
        public Color DisabledIconColor
        {
            get => _disabledIconColor;
            set
            {
                _disabledIconColor = value;
                Invalidate();
            }
        }
        public bool ShouldSerializeDisabledIconColor() => _disabledIconColor != Color.FromArgb(170, 170, 170);

        [Category("Appearance - Custom Icon")]
        [DefaultValue(12)]
        public int IconSize
        {
            get => _iconSize;
            set
            {
                if (value < 6) value = 6;
                _iconSize = value;
                Invalidate();
            }
        }

        [Category("Appearance - Custom Icon")]
        [DefaultValue(FontAwesomeIcon.None)]
        [Description("Selects the icon to display on the button directly from the Properties window.")]
        public FontAwesomeIcon ButtonIcon
        {
            get => _buttonIcon;
            set
            {
                _buttonIcon = value;
                _iconChar = MapToIconChar(value);
                Invalidate();
            }
        }

        #endregion

        #region Hidden From Designer - CodeBehind Only

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IconChar IconChar
        {
            get => _iconChar;
            set
            {
                _iconChar = value;
                _buttonIcon = MapToFontAwesomeIcon(value);
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIcon => _iconChar != IconChar.None;

        [Browsable(false)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        #endregion

        #region Public Methods For CodeBehind

        public void SetAwesomeIcon(FontAwesomeIcon icon)
        {
            ButtonIcon = icon;
        }

        public void SetAwesomeIcon(IconChar iconChar)
        {
            IconChar = iconChar;
        }

        public void ClearIcon()
        {
            ButtonIcon = FontAwesomeIcon.None;
        }

        #endregion

        #region Paint

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Parent == null)
            {
                using (SolidBrush brush = new SolidBrush(SystemColors.Control))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            GraphicsState state = e.Graphics.Save();

            try
            {
                e.Graphics.TranslateTransform(-Left, -Top);

                Rectangle parentRect = new Rectangle(Left, Top, Width, Height);
                using (PaintEventArgs pea = new PaintEventArgs(e.Graphics, parentRect))
                {
                    InvokePaintBackground(Parent, pea);
                    InvokePaint(Parent, pea);
                }
            }
            finally
            {
                e.Graphics.Restore(state);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rect = ClientRectangle;
            if (rect.Width <= 1 || rect.Height <= 1)
                return;

            RectangleF surfaceRect = new RectangleF(
                _borderThickness / 2f,
                _borderThickness / 2f,
                rect.Width - _borderThickness - 1f,
                rect.Height - _borderThickness - 1f);

            GetCurrentColors(out Color fillColor, out Color borderColor, out Color textColor, out Color iconColor);

            using (GraphicsPath path = CreateRoundPath(surfaceRect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                if (_borderThickness > 0)
                {
                    using (Pen pen = new Pen(borderColor, _borderThickness))
                    {
                        pen.Alignment = PenAlignment.Center;
                        g.DrawPath(pen, path);
                    }
                }
            }

            DrawContent(g, rect, textColor, iconColor);

            if (_isFocused && Enabled)
            {
                DrawFocusBorder(g, rect);
            }
        }

        private void DrawContent(Graphics g, Rectangle rect, Color textColor, Color iconColor)
        {
            string text = Text ?? string.Empty;
            bool hasIcon = _iconChar != IconChar.None;
            bool rtl = RightToLeft == RightToLeft.Yes;

            using (StringFormat iconSf = new StringFormat())
            {
                iconSf.LineAlignment = StringAlignment.Center;
                iconSf.Alignment = StringAlignment.Center;
                iconSf.FormatFlags = StringFormatFlags.NoWrap;

                Size textSize = TextRenderer.MeasureText(
                    g,
                    string.IsNullOrEmpty(text) ? " " : text,
                    Font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding);

                Size iconSize = Size.Empty;
                Bitmap iconBitmap = null;

                try
                {
                    if (hasIcon)
                    {
                        iconBitmap = _iconChar.ToBitmap(iconColor, _iconSize);
                        if (iconBitmap != null)
                            iconSize = iconBitmap.Size;
                    }

                    int totalWidth = textSize.Width;
                    if (hasIcon)
                        totalWidth += iconSize.Width + _contentSpacing;

                    int startX = Math.Max(_horizontalPadding, (rect.Width - totalWidth) / 2);
                    int currentX = startX;

                    if (rtl)
                    {
                        currentX = rect.Width - startX;

                        Rectangle textRect;
                        Rectangle iconRect;

                        if (hasIcon)
                        {
                            iconRect = new Rectangle(
                                currentX - iconSize.Width,
                                (rect.Height - iconSize.Height) / 2,
                                iconSize.Width,
                                iconSize.Height);

                            currentX -= iconSize.Width + _contentSpacing;
                        }
                        else
                        {
                            iconRect = Rectangle.Empty;
                        }

                        textRect = new Rectangle(
                            currentX - textSize.Width,
                            0,
                            textSize.Width,
                            rect.Height);

                        if (hasIcon && iconBitmap != null)
                            g.DrawImage(iconBitmap, iconRect);

                        TextRenderer.DrawText(
                            g,
                            text,
                            Font,
                            textRect,
                            textColor,
                            TextFormatFlags.VerticalCenter |
                            TextFormatFlags.Right |
                            TextFormatFlags.NoPadding |
                            TextFormatFlags.EndEllipsis);
                    }
                    else
                    {
                        Rectangle iconRect = Rectangle.Empty;

                        if (hasIcon)
                        {
                            iconRect = new Rectangle(
                                currentX,
                                (rect.Height - iconSize.Height) / 2,
                                iconSize.Width,
                                iconSize.Height);

                            currentX += iconSize.Width + _contentSpacing;
                        }

                        Rectangle textRect = new Rectangle(
                            currentX,
                            0,
                            rect.Width - currentX - _horizontalPadding,
                            rect.Height);

                        if (hasIcon && iconBitmap != null)
                            g.DrawImage(iconBitmap, iconRect);

                        TextRenderer.DrawText(
                            g,
                            text,
                            Font,
                            textRect,
                            textColor,
                            TextFormatFlags.VerticalCenter |
                            TextFormatFlags.Left |
                            TextFormatFlags.NoPadding |
                            TextFormatFlags.EndEllipsis);
                    }
                }
                finally
                {
                    iconBitmap?.Dispose();
                }
            }
        }

        private void DrawFocusBorder(Graphics g, Rectangle rect)
        {
            Rectangle focusRect = Rectangle.Inflate(rect, -4, -4);

            using (GraphicsPath path = CreateRoundPath(focusRect, Math.Max(2, _borderRadius - 2)))
            using (Pen pen = new Pen(Color.FromArgb(120, 0, 120, 215), 1))
            {
                pen.DashStyle = DashStyle.Dot;
                g.DrawPath(pen, path);
            }
        }

        #endregion

        #region Mouse / Keyboard

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (Enabled && !_isMouseDown)
            {
                _state = ButtonVisualState.Hover;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (Enabled && !_isMouseDown)
            {
                _state = ButtonVisualState.Normal;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Enabled || e.Button != MouseButtons.Left)
                return;

            Focus();
            _isMouseDown = true;
            _state = ButtonVisualState.Pressed;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!Enabled || e.Button != MouseButtons.Left)
                return;

            bool inside = ClientRectangle.Contains(e.Location);

            _isMouseDown = false;
            _state = inside ? ButtonVisualState.Hover : ButtonVisualState.Normal;
            Invalidate();

            if (inside)
                OnClick(EventArgs.Empty);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            _state = Enabled ? ButtonVisualState.Normal : ButtonVisualState.Disabled;
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Space || keyData == Keys.Enter)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!Enabled)
                return;

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                _state = ButtonVisualState.Pressed;
                Invalidate();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!Enabled)
                return;

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                _state = ButtonVisualState.Hover;
                Invalidate();
                OnClick(EventArgs.Empty);
            }
        }

        #endregion

        #region Helpers

        private void GetCurrentColors(out Color fillColor, out Color borderColor, out Color textColor, out Color iconColor)
        {
            if (!Enabled)
            {
                fillColor = _disabledBackColor;
                borderColor = _disabledBorderColor;
                textColor = _disabledForeColor;
                iconColor = _disabledIconColor;
                return;
            }

            switch (_state)
            {
                case ButtonVisualState.Hover:
                    fillColor = _hoverBackColor;
                    borderColor = _hoverBorderColor;
                    textColor = _hoverForeColor;
                    iconColor = _hoverIconColor;
                    break;

                case ButtonVisualState.Pressed:
                    fillColor = _pressedBackColor;
                    borderColor = _pressedBorderColor;
                    textColor = _pressedForeColor;
                    iconColor = _pressedIconColor;
                    break;

                default:
                    fillColor = _backColor;
                    borderColor = _borderColor;
                    textColor = _foreColor;
                    iconColor = _iconColor;
                    break;
            }
        }

        private GraphicsPath CreateRoundPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseFigure();
                return path;
            }

            float diameter = radius * 2f;
            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private IconChar MapToIconChar(FontAwesomeIcon icon)
        {
            switch (icon)
            {
                case FontAwesomeIcon.Plus: return IconChar.Plus;
                case FontAwesomeIcon.Minus: return IconChar.Minus;
                case FontAwesomeIcon.Check: return IconChar.Check;
                case FontAwesomeIcon.Times: return IconChar.Times;
                case FontAwesomeIcon.Trash: return IconChar.Trash;
                case FontAwesomeIcon.Edit: return IconChar.Edit;
                case FontAwesomeIcon.Save: return IconChar.Save;
                case FontAwesomeIcon.Search: return IconChar.Search;
                case FontAwesomeIcon.Home: return IconChar.Home;
                case FontAwesomeIcon.User: return IconChar.User;
                case FontAwesomeIcon.Cog: return IconChar.Cog;
                case FontAwesomeIcon.InfoCircle: return IconChar.InfoCircle;
                case FontAwesomeIcon.ExclamationTriangle: return IconChar.ExclamationTriangle;
                case FontAwesomeIcon.ArrowRight: return IconChar.ArrowRight;
                case FontAwesomeIcon.ArrowLeft: return IconChar.ArrowLeft;
                case FontAwesomeIcon.ChevronRight: return IconChar.ChevronRight;
                case FontAwesomeIcon.ChevronLeft: return IconChar.ChevronLeft;
                case FontAwesomeIcon.Calendar: return IconChar.Calendar;
                case FontAwesomeIcon.Clock: return IconChar.Clock;
                case FontAwesomeIcon.Print: return IconChar.Print;
                case FontAwesomeIcon.FileAlt: return IconChar.FileAlt;
                case FontAwesomeIcon.Database: return IconChar.Database;
                case FontAwesomeIcon.Lock: return IconChar.Lock;
                case FontAwesomeIcon.Unlock: return IconChar.Unlock;
                case FontAwesomeIcon.Key: return IconChar.Key;
                case FontAwesomeIcon.Sync: return IconChar.Sync;
                case FontAwesomeIcon.PowerOff: return IconChar.PowerOff;
                case FontAwesomeIcon.Envelope: return IconChar.Envelope;
                case FontAwesomeIcon.Phone: return IconChar.Phone;
                case FontAwesomeIcon.MapMarkerAlt: return IconChar.MapMarkerAlt;
                case FontAwesomeIcon.Download: return IconChar.Download;
                case FontAwesomeIcon.Upload: return IconChar.Upload;
                default: return IconChar.None;
            }
        }

        private FontAwesomeIcon MapToFontAwesomeIcon(IconChar iconChar)
        {
            switch (iconChar)
            {
                case IconChar.Plus: return FontAwesomeIcon.Plus;
                case IconChar.Minus: return FontAwesomeIcon.Minus;
                case IconChar.Check: return FontAwesomeIcon.Check;
                case IconChar.Times: return FontAwesomeIcon.Times;
                case IconChar.Trash: return FontAwesomeIcon.Trash;
                case IconChar.Edit: return FontAwesomeIcon.Edit;
                case IconChar.Save: return FontAwesomeIcon.Save;
                case IconChar.Search: return FontAwesomeIcon.Search;
                case IconChar.Home: return FontAwesomeIcon.Home;
                case IconChar.User: return FontAwesomeIcon.User;
                case IconChar.Cog: return FontAwesomeIcon.Cog;
                case IconChar.InfoCircle: return FontAwesomeIcon.InfoCircle;
                case IconChar.ExclamationTriangle: return FontAwesomeIcon.ExclamationTriangle;
                case IconChar.ArrowRight: return FontAwesomeIcon.ArrowRight;
                case IconChar.ArrowLeft: return FontAwesomeIcon.ArrowLeft;
                case IconChar.ChevronRight: return FontAwesomeIcon.ChevronRight;
                case IconChar.ChevronLeft: return FontAwesomeIcon.ChevronLeft;
                case IconChar.Calendar: return FontAwesomeIcon.Calendar;
                case IconChar.Clock: return FontAwesomeIcon.Clock;
                case IconChar.Print: return FontAwesomeIcon.Print;
                case IconChar.FileAlt: return FontAwesomeIcon.FileAlt;
                case IconChar.Database: return FontAwesomeIcon.Database;
                case IconChar.Lock: return FontAwesomeIcon.Lock;
                case IconChar.Unlock: return FontAwesomeIcon.Unlock;
                case IconChar.Key: return FontAwesomeIcon.Key;
                case IconChar.Sync: return FontAwesomeIcon.Sync;
                case IconChar.PowerOff: return FontAwesomeIcon.PowerOff;
                case IconChar.Envelope: return FontAwesomeIcon.Envelope;
                case IconChar.Phone: return FontAwesomeIcon.Phone;
                case IconChar.MapMarkerAlt: return FontAwesomeIcon.MapMarkerAlt;
                case IconChar.Download: return FontAwesomeIcon.Download;
                case IconChar.Upload: return FontAwesomeIcon.Upload;
                default: return FontAwesomeIcon.None;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

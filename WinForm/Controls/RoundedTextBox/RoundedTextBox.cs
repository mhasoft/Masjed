using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinForm.Controls.RoundedTextBox
{
    [DefaultEvent(nameof(SearchDelayCompleted))]
    public class RoundedTextBox : UserControl
    {
        private readonly TextBox _textBox;
        private readonly System.Windows.Forms.Timer _debounceTimer;

        private Color _borderColor = Color.FromArgb(180, 180, 180);
        private Color _borderFocusColor = Color.FromArgb(0, 120, 215);
        private int _borderRadius = 8;
        private int _borderSize = 1;
        private bool _isFocused;

        private string _placeholderText = string.Empty;
        private Color _placeholderColor = Color.Gray;
        private bool _isPlaceholderActive;

        private bool _suppressTextChanged;

        public event EventHandler? SearchDelayCompleted;

        public RoundedTextBox()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Multiline = false,
                Location = new Point(10, 7),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.Enter += TextBox_Enter;
            _textBox.Leave += TextBox_Leave;
            _textBox.KeyDown += (s, e) => OnKeyDown(e);
            _textBox.KeyPress += (s, e) => OnKeyPress(e);
            _textBox.KeyUp += (s, e) => OnKeyUp(e);

            _debounceTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _debounceTimer.Tick += DebounceTimer_Tick;

            Controls.Add(_textBox);

            BackColor = SystemColors.Window;
            Padding = new Padding(10, 7, 10, 7);
            Size = new Size(250, 30);

            UpdateTextBoxSizeAndPosition();
        }

        #region Appearance

        [Category("Appearance")]
        [Description("رنگ کادر دور کنترل در حالت عادی")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }
        public void ResetBorderColor() => BorderColor = Color.FromArgb(180, 180, 180);
        public bool ShouldSerializeBorderColor() => BorderColor != Color.FromArgb(180, 180, 180);

        [Category("Appearance")]
        [Description("رنگ کادر دور کنترل در زمان فوکوس")]
        public Color BorderFocusColor
        {
            get => _borderFocusColor;
            set
            {
                _borderFocusColor = value;
                Invalidate();
            }
        }
        public void ResetBorderFocusColor() => BorderFocusColor = Color.FromArgb(0, 120, 215);
        public bool ShouldSerializeBorderFocusColor() => BorderFocusColor != Color.FromArgb(0, 120, 215);

        [Category("Appearance")]
        [Description("میزان گردی لبه‌های کنترل")]
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

        [Category("Appearance")]
        [Description("ضخامت خط کادر دور")]
        [DefaultValue(1)]
        public int BorderSize
        {
            get => _borderSize;
            set
            {
                if (value >= 1)
                {
                    _borderSize = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("متن راهنما یا نگهدارنده جا")]
        [DefaultValue("")]

        // public?
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value ?? string.Empty;
                if (!_isFocused)
                    ApplyPlaceholderState();
            }
        }
        public bool ShouldSerializePlaceholderText() => !string.IsNullOrEmpty(PlaceholderText);

        [Category("Appearance")]
        [Description("رنگ متن راهنما")]
        public Color PlaceholderColor
        {
            get => _placeholderColor;
            set
            {
                _placeholderColor = value;
                if (_isPlaceholderActive)
                    _textBox.ForeColor = value;
            }
        }
        public void ResetPlaceholderColor() => PlaceholderColor = Color.Gray;
        public bool ShouldSerializePlaceholderColor() => PlaceholderColor != Color.Gray;

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _isPlaceholderActive ? string.Empty : _textBox.Text;
            set
            {
                _suppressTextChanged = true;
                try
                {
                    _isPlaceholderActive = false;
                    _textBox.ForeColor = ForeColor;
                    _textBox.Text = value ?? string.Empty;
                }
                finally
                {
                    _suppressTextChanged = false;
                }

                if (!_isFocused)
                    ApplyPlaceholderState();
            }
        }
        public override void ResetText() => Text = string.Empty;
        public bool ShouldSerializeText() => !string.IsNullOrEmpty(_textBox.Text);

        #endregion

        #region Behavior

        [Category("Behavior")]
        [Description("تعیین این که آیا تکست‌باکس چند خطی است یا خیر")]
        [DefaultValue(false)]
        public bool Multiline
        {
            get => _textBox.Multiline;
            set
            {
                _textBox.Multiline = value;
                UpdateTextBoxSizeAndPosition();
            }
        }

        [Category("Behavior")]
        [Description("تعیین کاراکتر ماسک برای پسورد")]
        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get => _textBox.UseSystemPasswordChar;
            set => _textBox.UseSystemPasswordChar = value;
        }

        [Category("Behavior")]
        [Description("تعیین وضعیت فقط خواندنی")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _textBox.ReadOnly;
            set => _textBox.ReadOnly = value;
        }

        [Category("Behavior")]
        [Description("حداکثر طول کاراکترهای ورودی")]
        [DefaultValue(32767)]
        public int MaxLength
        {
            get => _textBox.MaxLength;
            set => _textBox.MaxLength = value;
        }

        [Category("Behavior")]
        [Description("میزان زمان تاخیر پس از آخرین تایپ برای اجرای رویداد ویژه به میلی‌ثانیه")]
        [DefaultValue(1000)]
        public int SearchDelayInterval
        {
            get => _debounceTimer.Interval;
            set
            {
                if (value > 0)
                    _debounceTimer.Interval = value;
            }
        }

        #endregion

        #region Events

        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            if (_suppressTextChanged)
                return;

            if (!_isPlaceholderActive)
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();
                OnTextChanged(e);
            }
        }

        private void DebounceTimer_Tick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            SearchDelayCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void TextBox_Enter(object? sender, EventArgs e)
        {
            _isFocused = true;
            Invalidate();

            if (_isPlaceholderActive)
            {
                _suppressTextChanged = true;
                try
                {
                    _isPlaceholderActive = false;
                    _textBox.Text = string.Empty;
                    _textBox.ForeColor = ForeColor;
                }
                finally
                {
                    _suppressTextChanged = false;
                }
            }
        }

        private void TextBox_Leave(object? sender, EventArgs e)
        {
            _isFocused = false;
            Invalidate();
            ApplyPlaceholderState();
        }

        private void ApplyPlaceholderState()
        {
            if (_textBox.Focused)
                return;

            if (string.IsNullOrEmpty(_textBox.Text) && !string.IsNullOrEmpty(_placeholderText))
            {
                _suppressTextChanged = true;
                try
                {
                    _isPlaceholderActive = true;
                    _textBox.Text = _placeholderText;
                    _textBox.ForeColor = _placeholderColor;
                }
                finally
                {
                    _suppressTextChanged = false;
                }
            }
            else
            {
                _isPlaceholderActive = false;
                _textBox.ForeColor = ForeColor;
            }
        }

        #endregion

        #region Layout / Paint

        private void UpdateTextBoxSizeAndPosition()
        {
            if (_textBox.Multiline)
            {
                _textBox.Height = Height - Padding.Vertical;
                _textBox.Top = Padding.Top;
            }
            else
            {
                int txtHeight = _textBox.PreferredHeight;
                _textBox.Top = (Height - txtHeight) / 2;
            }

            _textBox.Left = Padding.Left;
            _textBox.Width = Width - Padding.Horizontal;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTextBoxSizeAndPosition();
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textBox.Font = Font;
            UpdateTextBoxSizeAndPosition();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (!_isPlaceholderActive)
                _textBox.ForeColor = ForeColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath pathBackground = GetRoundedPath(rectBorder, _borderRadius))
            using (SolidBrush brushBackground = new SolidBrush(BackColor))
            {
                g.FillPath(brushBackground, pathBackground);
            }

            using (GraphicsPath pathBorder = GetRoundedPath(rectBorder, _borderRadius))
            using (Pen penBorder = new Pen(_isFocused ? _borderFocusColor : _borderColor, _borderSize))
            {
                penBorder.Alignment = PenAlignment.Inset;
                g.DrawPath(penBorder, pathBorder);
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

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _textBox.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _debounceTimer.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}

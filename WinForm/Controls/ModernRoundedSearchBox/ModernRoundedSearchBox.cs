using FontAwesome.Sharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinForm.Controls.ModernRoundedSearchBox
{
    public class SearchSuggestion
    {
        public SearchSuggestion()
        {
        }

        public SearchSuggestion(string text)
        {
            Text = text;
        }

        public SearchSuggestion(
            string text,
            IconChar icon)
        {
            Text = text;
            Icon = icon;
        }

        public SearchSuggestion(
            string text,
            IconChar icon,
            object? value)
        {
            Text = text;
            Icon = icon;
            Value = value;
        }

        public SearchSuggestion(
            string text,
            IconChar icon,
            IconFont iconFont,
            object? value = null)
        {
            Text = text;
            Icon = icon;
            IconFont = iconFont;
            Value = value;
        }

        [Category("Suggestion")]
        [DefaultValue("")]
        public string Text { get; set; } =
            string.Empty;

        [Category("Suggestion")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public object? Value { get; set; }

        [Category("Icon")]
        [DefaultValue(IconChar.None)]
        public IconChar Icon { get; set; } =
            IconChar.None;

        [Category("Icon")]
        [DefaultValue(IconFont.Solid)]
        public IconFont IconFont { get; set; } =
            IconFont.Solid;

        [Category("Icon")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color IconColor { get; set; } =
            Color.Empty;

        [Category("Icon")]
        [DefaultValue(18)]
        public int IconSize { get; set; } = 18;

        [Category("Appearance")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color TextColor { get; set; } =
            Color.Empty;

        [Category("Appearance")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color HoverTextColor { get; set; } =
            Color.Empty;

        [Category("Data")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public object? Tag { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class SearchSuggestionSelectedEventArgs :
        EventArgs
    {
        public SearchSuggestionSelectedEventArgs(
            SearchSuggestion suggestion)
        {
            Suggestion = suggestion;
        }

        public SearchSuggestion Suggestion { get; }

        public string Text =>
            Suggestion.Text;

        public object? Value =>
            Suggestion.Value;

        public object? Tag =>
            Suggestion.Tag;
    }

    [DefaultEvent(nameof(SuggestionSelected))]
    public class ModernRoundedSearchBox :
        UserControl
    {
        private readonly TextBox _textBox;

        private readonly SuggestionPopup
            _suggestionPopup;

        private readonly System.Windows.Forms.Timer
            _debounceTimer;

        #region Fields

        private Color _borderColor =
            Color.FromArgb(180, 180, 180);

        private Color _borderFocusColor =
            Color.FromArgb(0, 120, 215);

        private int _borderRadius = 8;

        private int _borderSize = 1;

        private bool _isFocused;

        private string _placeholderText =
            string.Empty;

        private Color _placeholderColor =
            Color.Gray;

        private bool _isPlaceholderActive;

        private bool _suppressTextChanged;

        private Color _suggestionIconColor =
            Color.FromArgb(80, 80, 80);

        private int _suggestionIconSize = 18;

        private Color _suggestionTextColor =
            Color.FromArgb(40, 40, 40);

        private Color _suggestionHoverTextColor =
            Color.FromArgb(30, 30, 30);

        private Color _suggestionHoverBackColor =
            Color.FromArgb(240, 240, 240);

        private Color _suggestionBackColor =
            Color.White;

        private Color _suggestionBorderColor =
            Color.FromArgb(220, 220, 220);

        private int _suggestionItemHeight = 38;

        private int _suggestionMaxItems = 8;

        private int _suggestionIconTextSpacing = 10;

        private bool _showSuggestions = true;

        #endregion

        #region Events

        public event EventHandler?
            SearchDelayCompleted;

        public event EventHandler<
            SearchSuggestionSelectedEventArgs>?
            SuggestionSelected;

        #endregion

        #region Constructor

        public ModernRoundedSearchBox()
        {
            SetStyle(
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw,
                true);

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Multiline = false,
                Location = new Point(10, 7),
                Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            _textBox.TextChanged +=
                TextBox_TextChanged;

            _textBox.Enter +=
                TextBox_Enter;

            _textBox.Leave +=
                TextBox_Leave;

            _textBox.KeyDown +=
                TextBox_KeyDown;

            _textBox.KeyPress +=
                TextBox_KeyPress;

            _textBox.KeyUp +=
                TextBox_KeyUp;

            _debounceTimer =
                new System.Windows.Forms.Timer
                {
                    Interval = 1000
                };

            _debounceTimer.Tick +=
                DebounceTimer_Tick;

            _suggestionPopup =
                new SuggestionPopup();

            _suggestionPopup.ItemClicked +=
                SuggestionPopup_ItemClicked;

            _suggestions.ListChanged +=
                Suggestions_ListChanged;

            Controls.Add(_textBox);

            BackColor =
                SystemColors.Window;

            Padding =
                new Padding(10, 7, 10, 7);

            Size =
                new Size(250, 30);

            UpdateTextBoxSizeAndPosition();
        }

        #endregion

        #region Suggestions

        private void SuggestionPopup_ItemClicked(
            object? sender,
            SearchSuggestion suggestion)
        {
            if (suggestion == null)
                return;

            _suppressTextChanged = true;

            try
            {
                _isPlaceholderActive = false;

                _textBox.ForeColor =
                    ForeColor;

                _textBox.Text =
                    suggestion.Text;

                _textBox.SelectionStart =
                    _textBox.TextLength;

                _textBox.SelectionLength = 0;
            }
            finally
            {
                _suppressTextChanged = false;
            }

            _suggestionPopup.Hide();

            _textBox.Focus();

            SuggestionSelected?.Invoke(
                this,
                new SearchSuggestionSelectedEventArgs(
                    suggestion));
        }

        private void Suggestions_ListChanged(
            object? sender,
            ListChangedEventArgs e)
        {
            UpdateSuggestionPopup();
        }

        private void UpdateSuggestionPopup()
        {
            if (!_showSuggestions ||
                _suggestions.Count == 0)
            {
                _suggestionPopup.Hide();
                return;
            }

            _suggestionPopup.SuggestionBackColor =
                _suggestionBackColor;

            _suggestionPopup.SuggestionBorderColor =
                _suggestionBorderColor;

            _suggestionPopup.SuggestionHoverBackColor =
                _suggestionHoverBackColor;

            _suggestionPopup.SuggestionTextColor =
                _suggestionTextColor;

            _suggestionPopup.SuggestionHoverTextColor =
                _suggestionHoverTextColor;

            _suggestionPopup.SuggestionIconColor =
                _suggestionIconColor;

            _suggestionPopup.SuggestionIconSize =
                _suggestionIconSize;

            _suggestionPopup.SuggestionItemHeight =
                _suggestionItemHeight;

            _suggestionPopup.SuggestionMaxItems =
                _suggestionMaxItems;

            _suggestionPopup.SuggestionIconTextSpacing =
                _suggestionIconTextSpacing;

            _suggestionPopup.SetItems(
                _suggestions);

            Point location =
                new Point(0, Height);

            _suggestionPopup.ShowPopup(
                this,
                location);
        }

        #endregion

        #region Appearance

        [Category("Appearance")]
        [Description("رنگ کادر کنترل")]
        public Color BorderColor
        {
            get => _borderColor;

            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public void ResetBorderColor()
        {
            BorderColor =
                Color.FromArgb(180, 180, 180);
        }

        public bool ShouldSerializeBorderColor()
        {
            return BorderColor !=
                   Color.FromArgb(180, 180, 180);
        }

        [Category("Appearance")]
        [Description("رنگ کادر هنگام Focus")]
        public Color BorderFocusColor
        {
            get => _borderFocusColor;

            set
            {
                _borderFocusColor = value;
                Invalidate();
            }
        }

        public void ResetBorderFocusColor()
        {
            BorderFocusColor =
                Color.FromArgb(0, 120, 215);
        }

        public bool ShouldSerializeBorderFocusColor()
        {
            return BorderFocusColor !=
                   Color.FromArgb(0, 120, 215);
        }

        [Category("Appearance")]
        [DefaultValue(8)]
        public int BorderRadius
        {
            get => _borderRadius;

            set
            {
                if (value < 0)
                    return;

                _borderRadius = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(1)]
        public int BorderSize
        {
            get => _borderSize;

            set
            {
                if (value < 1)
                    return;

                _borderSize = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue("")]
        public string PlaceholderText
        {
            get => _placeholderText;

            set
            {
                _placeholderText =
                    value ?? string.Empty;

                if (!_isFocused)
                    ApplyPlaceholderState();
            }
        }

        public bool ShouldSerializePlaceholderText()
        {
            return !string.IsNullOrEmpty(
                PlaceholderText);
        }

        [Category("Appearance")]
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

        public void ResetPlaceholderColor()
        {
            PlaceholderColor = Color.Gray;
        }

        public bool ShouldSerializePlaceholderColor()
        {
            return PlaceholderColor != Color.Gray;
        }

        [Category("Appearance")]
        [Browsable(true)]
        public override string Text
        {
            get =>
                _isPlaceholderActive
                    ? string.Empty
                    : _textBox.Text;

            set
            {
                _suppressTextChanged = true;

                try
                {
                    _isPlaceholderActive = false;

                    _textBox.ForeColor =
                        ForeColor;

                    _textBox.Text =
                        value ?? string.Empty;
                }
                finally
                {
                    _suppressTextChanged = false;
                }

                if (!_isFocused)
                    ApplyPlaceholderState();
            }
        }

        public override void ResetText()
        {
            Text = string.Empty;
        }

        public bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(
                _textBox.Text);
        }

        #endregion

        #region Suggestion Appearance

        [Category("Suggestions")]
        public Color SuggestionBackColor
        {
            get => _suggestionBackColor;

            set
            {
                _suggestionBackColor = value;
                _suggestionPopup.SuggestionBackColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionBackColor()
        {
            SuggestionBackColor = Color.White;
        }

        public bool ShouldSerializeSuggestionBackColor()
        {
            return SuggestionBackColor != Color.White;
        }

        [Category("Suggestions")]
        public Color SuggestionHoverBackColor
        {
            get => _suggestionHoverBackColor;

            set
            {
                _suggestionHoverBackColor = value;
                _suggestionPopup.SuggestionHoverBackColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionHoverBackColor()
        {
            SuggestionHoverBackColor =
                Color.FromArgb(240, 240, 240);
        }

        public bool ShouldSerializeSuggestionHoverBackColor()
        {
            return SuggestionHoverBackColor !=
                   Color.FromArgb(240, 240, 240);
        }

        [Category("Suggestions")]
        public Color SuggestionTextColor
        {
            get => _suggestionTextColor;

            set
            {
                _suggestionTextColor = value;
                _suggestionPopup.SuggestionTextColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionTextColor()
        {
            SuggestionTextColor =
                Color.FromArgb(40, 40, 40);
        }

        public bool ShouldSerializeSuggestionTextColor()
        {
            return SuggestionTextColor !=
                   Color.FromArgb(40, 40, 40);
        }

        [Category("Suggestions")]
        public Color SuggestionHoverTextColor
        {
            get => _suggestionHoverTextColor;

            set
            {
                _suggestionHoverTextColor = value;
                _suggestionPopup.SuggestionHoverTextColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionHoverTextColor()
        {
            SuggestionHoverTextColor =
                Color.FromArgb(30, 30, 30);
        }

        public bool ShouldSerializeSuggestionHoverTextColor()
        {
            return SuggestionHoverTextColor !=
                   Color.FromArgb(30, 30, 30);
        }

        [Category("Suggestions")]
        public Color SuggestionBorderColor
        {
            get => _suggestionBorderColor;

            set
            {
                _suggestionBorderColor = value;
                _suggestionPopup.SuggestionBorderColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionBorderColor()
        {
            SuggestionBorderColor =
                Color.FromArgb(220, 220, 220);
        }

        public bool ShouldSerializeSuggestionBorderColor()
        {
            return SuggestionBorderColor !=
                   Color.FromArgb(220, 220, 220);
        }

        [Category("Suggestions")]
        public Color SuggestionIconColor
        {
            get => _suggestionIconColor;

            set
            {
                _suggestionIconColor = value;
                _suggestionPopup.SuggestionIconColor = value;
                _suggestionPopup.Invalidate();
            }
        }

        public void ResetSuggestionIconColor()
        {
            SuggestionIconColor =
                Color.FromArgb(80, 80, 80);
        }

        public bool ShouldSerializeSuggestionIconColor()
        {
            return SuggestionIconColor !=
                   Color.FromArgb(80, 80, 80);
        }

        [Category("Suggestions")]
        [DefaultValue(18)]
        public int SuggestionIconSize
        {
            get => _suggestionIconSize;

            set
            {
                if (value <= 0)
                    return;

                _suggestionIconSize = value;
                _suggestionPopup.SuggestionIconSize = value;
                _suggestionPopup.Invalidate();
            }
        }

        [Category("Suggestions")]
        [DefaultValue(38)]
        public int SuggestionItemHeight
        {
            get => _suggestionItemHeight;

            set
            {
                if (value < 20)
                    return;

                _suggestionItemHeight = value;
                _suggestionPopup.SuggestionItemHeight = value;
            }
        }

        [Category("Suggestions")]
        [DefaultValue(8)]
        public int SuggestionMaxItems
        {
            get => _suggestionMaxItems;

            set
            {
                if (value < 1)
                    return;

                _suggestionMaxItems = value;
                _suggestionPopup.SuggestionMaxItems = value;
            }
        }

        [Category("Suggestions")]
        [DefaultValue(10)]
        public int SuggestionIconTextSpacing
        {
            get => _suggestionIconTextSpacing;

            set
            {
                if (value < 0)
                    return;

                _suggestionIconTextSpacing = value;
                _suggestionPopup.SuggestionIconTextSpacing = value;
                _suggestionPopup.Invalidate();
            }
        }

        [Category("Suggestions")]
        [DefaultValue(true)]
        public bool ShowSuggestions
        {
            get => _showSuggestions;

            set
            {
                _showSuggestions = value;

                if (!value)
                    _suggestionPopup.Hide();
                else
                    UpdateSuggestionPopup();
            }
        }

        #endregion

        #region Behavior

        [Category("Behavior")]
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
        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get => _textBox.UseSystemPasswordChar;

            set =>
                _textBox.UseSystemPasswordChar = value;
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _textBox.ReadOnly;

            set =>
                _textBox.ReadOnly = value;
        }

        [Category("Behavior")]
        [DefaultValue(32767)]
        public int MaxLength
        {
            get => _textBox.MaxLength;

            set =>
                _textBox.MaxLength = value;
        }

        [Category("Behavior")]
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

        #region Suggestions Collection

        private readonly BindingList<SearchSuggestion>
            _suggestions =
                new BindingList<SearchSuggestion>();

        [Category("Suggestions")]
        [Description("لیست پیشنهادهای جستجو")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Content)]
        public BindingList<SearchSuggestion> Suggestions =>
            _suggestions;

        #endregion

        #region TextBox Events

        private void TextBox_TextChanged(
            object? sender,
            EventArgs e)
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

        private void DebounceTimer_Tick(
            object? sender,
            EventArgs e)
        {
            _debounceTimer.Stop();

            SearchDelayCompleted?.Invoke(
                this,
                EventArgs.Empty);
        }

        private void TextBox_Enter(
            object? sender,
            EventArgs e)
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

        private void TextBox_Leave(
            object? sender,
            EventArgs e)
        {
            _isFocused = false;

            Invalidate();

            ApplyPlaceholderState();
        }

        private void ApplyPlaceholderState()
        {
            if (_textBox.Focused)
                return;

            if (string.IsNullOrEmpty(_textBox.Text) &&
                !string.IsNullOrEmpty(_placeholderText))
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

        #region Keyboard

        private void TextBox_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void TextBox_KeyPress(
            object? sender,
            KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void TextBox_KeyUp(
            object? sender,
            KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        #endregion

        #region Layout

        private void UpdateTextBoxSizeAndPosition()
        {
            if (_textBox.Multiline)
            {
                _textBox.Height =
                    Height - Padding.Vertical;

                _textBox.Top =
                    Padding.Top;
            }
            else
            {
                int txtHeight =
                    _textBox.PreferredHeight;

                _textBox.Top =
                    (Height - txtHeight) / 2;
            }

            _textBox.Left =
                Padding.Left;

            _textBox.Width =
                Width - Padding.Horizontal;
        }

        protected override void OnResize(
            EventArgs e)
        {
            base.OnResize(e);

            UpdateTextBoxSizeAndPosition();
            Invalidate();
        }

        protected override void OnFontChanged(
            EventArgs e)
        {
            base.OnFontChanged(e);

            _textBox.Font = Font;

            UpdateTextBoxSizeAndPosition();
        }

        protected override void OnForeColorChanged(
            EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (!_isPlaceholderActive)
                _textBox.ForeColor = ForeColor;
        }

        #endregion

        #region Painting

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle rect =
                new Rectangle(
                    0,
                    0,
                    Width - 1,
                    Height - 1);

            using GraphicsPath path =
                GetRoundedPath(
                    rect,
                    _borderRadius);

            using SolidBrush brush =
                new SolidBrush(BackColor);

            g.FillPath(brush, path);

            using Pen pen =
                new Pen(
                    _isFocused
                        ? _borderFocusColor
                        : _borderColor,
                    _borderSize);

            pen.Alignment =
                PenAlignment.Inset;

            g.DrawPath(pen, path);
        }

        private static GraphicsPath GetRoundedPath(
            Rectangle rect,
            int radius)
        {
            GraphicsPath path =
                new GraphicsPath();

            float size = radius * 2F;

            if (size <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            size =
                Math.Min(
                    size,
                    Math.Min(
                        rect.Width,
                        rect.Height));

            path.StartFigure();

            path.AddArc(
                rect.X,
                rect.Y,
                size,
                size,
                180,
                90);

            path.AddArc(
                rect.Right - size,
                rect.Y,
                size,
                size,
                270,
                90);

            path.AddArc(
                rect.Right - size,
                rect.Bottom - size,
                size,
                size,
                0,
                90);

            path.AddArc(
                rect.X,
                rect.Bottom - size,
                size,
                size,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        #endregion

        #region Focus

        protected override void OnGotFocus(
            EventArgs e)
        {
            base.OnGotFocus(e);

            _textBox.Focus();
        }

        #endregion

        #region Dispose

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                _debounceTimer.Stop();

                _debounceTimer.Tick -=
                    DebounceTimer_Tick;

                _textBox.TextChanged -=
                    TextBox_TextChanged;

                _textBox.Enter -=
                    TextBox_Enter;

                _textBox.Leave -=
                    TextBox_Leave;

                _textBox.KeyDown -=
                    TextBox_KeyDown;

                _textBox.KeyPress -=
                    TextBox_KeyPress;

                _textBox.KeyUp -=
                    TextBox_KeyUp;

                _suggestionPopup.ItemClicked -=
                    SuggestionPopup_ItemClicked;

                _suggestions.ListChanged -=
                    Suggestions_ListChanged;

                _suggestionPopup.Dispose();

                _debounceTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
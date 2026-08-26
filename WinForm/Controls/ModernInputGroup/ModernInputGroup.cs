using FontAwesome.Sharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinForm.Controls.ModernButton;

// Alias برای جلوگیری از تداخل نام کلاس ModernButton با namespace
using ModernButtonControl = WinForm.Controls.ModernButton.ModernButton;

namespace WinForm.Controls.ModernInputGroup
{
    [DefaultEvent("ButtonClick")]
    public class ModernInputGroup : UserControl
    {
        // اجزای داخلی
        private readonly TextBox txtInput;
        private readonly ModernButtonControl btnAction;

        // رویداد کلیک دکمه برای استفاده در فرم
        public event EventHandler ButtonClick;

        // فیلدهای مربوط به ظاهر کنترل
        private int _borderRadius = 10; // شعاع گردی کنترل اصلی
        private Color _borderColor = Color.FromArgb(72, 122, 190);
        private Color _focusColor = Color.FromArgb(40, 90, 160);
        private bool _isFocused = false;

        // مقادیر پیش‌فرض برای خصوصیات دکمه
        private int _buttonWidth = 100;
        private int _buttonBorderRadius = 5; // شعاع گردی لبه های دکمه

        public ModernInputGroup()
        {
            // مقداردهی اولیه کامپوننت‌های داخلی
            txtInput = new TextBox()
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11f),
                Multiline = false,
            };

            txtInput.Enter += (s, e) => { _isFocused = true; Invalidate(); };
            txtInput.Leave += (s, e) => { _isFocused = false; Invalidate(); };

            btnAction = new ModernButtonControl()
            {
                Text = "ذخیره",
                Padding = new Padding(5, 5, 5, 5),
                Size = new Size(50, 35)

            };
            btnAction.Click += (s, e) => OnButtonClick(EventArgs.Empty);

            this.Controls.Add(txtInput);
            this.Controls.Add(btnAction);

            // تنظیمات اولیه خود کنترل
            this.Size = new Size(350, 45);
            this.Padding = new Padding(0, 0, 0, 0); // اضافه کردن padding از همه طرف برای کنترل
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
        }

        #region Properties

        [Category("Appearance - Input")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public override string Text
        {
            get => txtInput?.Text ?? string.Empty;
            set
            {
                if (txtInput != null)
                {
                    txtInput.Text = value ?? string.Empty;
                }
            }
        }

        [Category("Appearance - Button")]
        [DefaultValue("ذخیره")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ButtonText
        {
            get => btnAction.Text;
            set
            {
                if (btnAction != null)
                {
                    btnAction.Text = value ?? string.Empty;
                    btnAction.Invalidate();
                }
            }
        }

        [Category("Appearance - Button")]
        [DefaultValue(FontAwesomeIcon.None)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public FontAwesomeIcon ButtonIcon
        {
            get => btnAction.ButtonIcon;
            set
            {
                if (btnAction != null)
                {
                    btnAction.ButtonIcon = value;
                    btnAction.Invalidate();
                }
            }
        }

        [Category("Appearance - Button")]
        [DefaultValue(100)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonWidth
        {
            get => _buttonWidth;
            set
            {
                _buttonWidth = Math.Max(30, value); // حداقل عرض 30 پیکسل
                OnResize(EventArgs.Empty);
            }
        }

        [Category("Appearance - Button")]
        [DefaultValue(5)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonBorderRadius
        {
            get => _buttonBorderRadius;
            set
            {
                _buttonBorderRadius = Math.Max(0, value);
                if (btnAction != null)
                {
                    btnAction.BorderRadius = _buttonBorderRadius;
                    btnAction.Invalidate();
                }
            }
        }

        [Category("Appearance - Style")]
        [DefaultValue(10)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(0, value);
                UpdateRegion();
                Invalidate();
            }
        }

        [Category("Appearance - Style")]
        [DefaultValue(typeof(Color), "72, 122, 190")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance - Style")]
        [DefaultValue(typeof(Color), "40, 90, 160")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FocusColor
        {
            get => _focusColor;
            set
            {
                _focusColor = value;
                if (_isFocused)
                    Invalidate();
            }
        }

        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (txtInput != null && btnAction != null)
            {
                // محاسبه فضای داخلی کنترل پس از اعمال Padding
                int innerWidth = this.Width - this.Padding.Left - this.Padding.Right;
                int innerHeight = this.Height - this.Padding.Top - this.Padding.Bottom;

                // تنظیم ابعاد و موقعیت دکمه
                btnAction.Height = innerHeight;
                btnAction.Width = _buttonWidth;
                // موقعیت دکمه در سمت راست، با فاصله 5 پیکسل از لبه راست و 5 پیکسل از لبه بالایی/پایینی
                btnAction.Location = new Point(this.Width - this.Padding.Right - _buttonWidth, this.Padding.Top);

                // تنظیم ابعاد و موقعیت TextBox
                txtInput.Width = innerWidth - btnAction.Width - 5; // فضای باقی مانده برای TextBox منهای عرض دکمه و 5 پیکسل فاصله
                txtInput.Height = innerHeight; // TextBox ارتفاع داخلی را پر می‌کند
                // موقعیت TextBox در سمت چپ دکمه، با فاصله 5 پیکسل از لبه چپ و 5 پیکسل از لبه بالایی
                txtInput.Location = new Point(this.Padding.Left, this.Padding.Top);
                txtInput.Padding = new Padding(0, (txtInput.Height - txtInput.Font.Height) / 2, 0, 0); // تنظیم padding عمودی برای متن داخل TextBox

                // اطمینان از اینکه BorderRadius دکمه به روز شده است
                btnAction.BorderRadius = _buttonBorderRadius;
            }

            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // مستطیل برای رسم حاشیه و پس‌زمینه کنترل اصلی (با در نظر گرفتن Padding)
            Rectangle rect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right - 1, Height - Padding.Top - Padding.Bottom - 1);
            Color currentBorder = _isFocused ? _focusColor : _borderColor;

            // ایجاد مسیر گرد برای کنترل اصلی
            using (GraphicsPath path = CreateRoundPath(rect, _borderRadius))
            {
                // رسم پس‌زمینه کلی کنترل
                using (SolidBrush br = new SolidBrush(this.BackColor))
                {
                    g.FillPath(br, path);
                }

                // رسم حاشیه کنترل اصلی
                using (Pen pen = new Pen(currentBorder, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            UpdateRegion();
        }

        // ایجاد مسیر گرد برای شکل کنترل
        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // اطمینان از اینکه قطر دایره از ابعاد مستطیل بزرگتر نباشد
            diameter = Math.Min(diameter, rect.Width);
            diameter = Math.Min(diameter, rect.Height);

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // بالا-چپ
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // بالا-راست
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // پایین-راست
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // پایین-چپ
            path.CloseFigure();

            return path;
        }

        // به‌روزرسانی Region کنترل بر اساس شکل گرد آن
        private void UpdateRegion()
        {
            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width, Height), _borderRadius))
            {
                this.Region = new Region(path);
            }
        }

        protected virtual void OnButtonClick(EventArgs e)
        {
            txtInput.Focus();
            ButtonClick?.Invoke(this, e);
        }

        protected override void OnClick(EventArgs e)
        {
            txtInput.Focus();
            base.OnClick(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == (char)Keys.Enter)
            {
                OnButtonClick(EventArgs.Empty);
                e.Handled = true;
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace WinForm.Controls.ModernRoundedPanel
{
    [DefaultProperty(nameof(BackgroundColor))]
    [DefaultEvent(nameof(Click))]
    public class ModernRoundedPanel : Panel
    {
        #region Constants

        private const int DefaultBorderRadius = 14;
        private const int DefaultBorderThickness = 1;

        private static readonly Color DefaultBorderColor =
            Color.FromArgb(72, 122, 190);

        private static readonly Color DefaultBackgroundColor =
            Color.White;

        #endregion

        #region Fields

        private int _borderRadius = DefaultBorderRadius;
        private int _borderThickness = DefaultBorderThickness;
        private Color _borderColor = DefaultBorderColor;
        private Color _backgroundColor = DefaultBackgroundColor;

        #endregion

        #region Properties

        [Category("Rounded Panel")]
        [DisplayName("Background Color")]
        [Description("رنگ زمینه بخش داخلی پنل را تعیین می‌کند.")]
        [DefaultValue(typeof(Color), "White")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if (_backgroundColor == value)
                    return;

                _backgroundColor = value;

                // فقط زمینه داخلی پنل تغییر می‌کند.
                base.BackColor = value;

                Invalidate();
            }
        }

        [Category("Rounded Panel")]
        [DisplayName("Border Color")]
        [Description("رنگ حاشیه یا Stroke پنل را تعیین می‌کند.")]
        [DefaultValue(typeof(Color), "72, 122, 190")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                if (_borderColor == value)
                    return;

                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Rounded Panel")]
        [DisplayName("Border Thickness")]
        [Description(
            "ضخامت حاشیه را برحسب پیکسل تعیین می‌کند. " +
            "مقدار صفر یعنی بدون حاشیه."
        )]
        [DefaultValue(DefaultBorderThickness)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get
            {
                return _borderThickness;
            }
            set
            {
                int newValue = Math.Max(0, value);

                if (_borderThickness == newValue)
                    return;

                _borderThickness = newValue;
                Invalidate();
            }
        }

        [Category("Rounded Panel")]
        [DisplayName("Border Radius")]
        [Description("میزان گردی گوشه‌های پنل را تعیین می‌کند.")]
        [DefaultValue(DefaultBorderRadius)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get
            {
                return _borderRadius;
            }
            set
            {
                int newValue = Math.Max(0, value);

                if (_borderRadius == newValue)
                    return;

                _borderRadius = newValue;
                Invalidate();
            }
        }

        /*
         * BackColor استاندارد در Properties مخفی است.
         * رنگ داخلی پنل از طریق BackgroundColor تنظیم می‌شود.
         */
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(typeof(Color), "White")]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                return BackgroundColor;
            }
            set
            {
                BackgroundColor = value;
            }
        }

        #endregion

        #region Designer Serialization

        private bool ShouldSerializeBackgroundColor()
        {
            return _backgroundColor != DefaultBackgroundColor;
        }

        private void ResetBackgroundColor()
        {
            BackgroundColor = DefaultBackgroundColor;
        }

        private bool ShouldSerializeBorderColor()
        {
            return _borderColor != DefaultBorderColor;
        }

        private void ResetBorderColor()
        {
            BorderColor = DefaultBorderColor;
        }

        private bool ShouldSerializeBorderThickness()
        {
            return _borderThickness != DefaultBorderThickness;
        }

        private void ResetBorderThickness()
        {
            BorderThickness = DefaultBorderThickness;
        }

        private bool ShouldSerializeBorderRadius()
        {
            return _borderRadius != DefaultBorderRadius;
        }

        private void ResetBorderRadius()
        {
            BorderRadius = DefaultBorderRadius;
        }

        #endregion

        #region Constructor

        public ModernRoundedPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            DoubleBuffered = true;

            Size = new Size(250, 150);
            Padding = new Padding(_borderThickness + 1);

            base.BackColor = _backgroundColor;
        }

        #endregion

        #region Background Painting

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            /*
             * زمینه واقعی Parent را در محدوده کنترل بازسازی می‌کنیم.
             * این کار باعث می‌شود گوشه‌های گرد همان رنگ و محتوای پشت خود
             * را نشان دهند و به رنگ BackgroundColor تبدیل نشوند.
             */
            if (Parent == null)
            {
                using (SolidBrush brush =
                    new SolidBrush(SystemColors.Control))
                {
                    e.Graphics.FillRectangle(
                        brush,
                        ClientRectangle);
                }

                return;
            }

            GraphicsState state = e.Graphics.Save();

            try
            {
                e.Graphics.TranslateTransform(-Left, -Top);

                Rectangle parentRectangle = new Rectangle(
                    Left,
                    Top,
                    Width,
                    Height);

                using (PaintEventArgs parentPaintArgs =
                    new PaintEventArgs(
                        e.Graphics,
                        parentRectangle))
                {
                    InvokePaintBackground(Parent, parentPaintArgs);
                    InvokePaint(Parent, parentPaintArgs);
                }
            }
            finally
            {
                e.Graphics.Restore(state);
            }
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;

            if (rect.Width <= 1 || rect.Height <= 1)
                return;

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            /*
             * همان محاسبات ModernButton برای رسم نرم‌تر Stroke.
             */
            RectangleF surfaceRect = new RectangleF(
                _borderThickness / 2f,
                _borderThickness / 2f,
                rect.Width - _borderThickness - 1f,
                rect.Height - _borderThickness - 1f);

            if (surfaceRect.Width <= 1f ||
                surfaceRect.Height <= 1f)
            {
                return;
            }

            float radius = Math.Min(
                _borderRadius,
                Math.Min(
                    surfaceRect.Width,
                    surfaceRect.Height) / 2f);

            using (GraphicsPath path = CreateRoundedPath(
                surfaceRect,
                radius))
            {
                /*
                 * فقط سطح داخلی پنل با BackgroundColor پر می‌شود.
                 * خارج از این مسیر دست‌نخورده باقی می‌ماند.
                 */
                using (SolidBrush backgroundBrush =
                    new SolidBrush(_backgroundColor))
                {
                    g.FillPath(backgroundBrush, path);
                }

                if (_borderThickness > 0)
                {
                    using (Pen borderPen =
                        new Pen(_borderColor, _borderThickness))
                    {
                        borderPen.Alignment = PenAlignment.Center;
                        borderPen.LineJoin = LineJoin.Round;
                        borderPen.StartCap = LineCap.Round;
                        borderPen.EndCap = LineCap.Round;

                        g.DrawPath(borderPen, path);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private static GraphicsPath CreateRoundedPath(
            RectangleF rectangle,
            float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (rectangle.Width <= 0f ||
                rectangle.Height <= 0f)
            {
                return path;
            }

            if (radius <= 0f)
            {
                path.AddRectangle(rectangle);
                path.CloseFigure();

                return path;
            }

            float diameter = radius * 2f;

            if (diameter > rectangle.Width)
                diameter = rectangle.Width;

            if (diameter > rectangle.Height)
                diameter = rectangle.Height;

            path.StartFigure();

            path.AddArc(
                rectangle.X,
                rectangle.Y,
                diameter,
                diameter,
                180f,
                90f);

            path.AddArc(
                rectangle.Right - diameter,
                rectangle.Y,
                diameter,
                diameter,
                270f,
                90f);

            path.AddArc(
                rectangle.Right - diameter,
                rectangle.Bottom - diameter,
                diameter,
                diameter,
                0f,
                90f);

            path.AddArc(
                rectangle.X,
                rectangle.Bottom - diameter,
                diameter,
                diameter,
                90f,
                90f);

            path.CloseFigure();

            return path;
        }

        #endregion
    }
}

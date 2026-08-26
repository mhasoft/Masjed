using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WinForm.Controls.AnimatedTilePanel.Entities; // فرض شده این namespace وجود دارد
using WinForm.Controls.AnimatedTilePanel.Events; // فرض شده این namespace وجود دارد

namespace WinForm.Controls.AnimatedTiles
{
    [DefaultEvent(nameof(ItemClicked))]
    public class AnimatedTilePanel : ScrollableControl
    {
        private readonly System.Windows.Forms.Timer _animationTimer;
        private readonly List<TileVisualInfo> _tiles = new List<TileVisualInfo>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        // --- Properties ---
        private int _cardWidth = 320;
        private int _cardHeight = 120;
        private int _cardSpacing = 18;
        private int _cardRadius = 18;
        private int _animationDuration = 380;
        private int _animationStartOffsetY = 30;
        private int _staggerDelay = 90;
        private int _outerPadding = 20;
        private int _hoverOffset = 5;
        private float _hoverAnimationStep = 0.50f;
        private Color _hoverStrokeColor = Color.FromArgb(135, 206, 250); // Sky Blue default
        private List<DashboardTileItem> _items = new List<DashboardTileItem>();
        private bool _forceResetAnimation = false;

        // --- Constructor ---
        public AnimatedTilePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            BackColor = Color.FromArgb(240, 240, 240);

            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        // --- Public Properties ---
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<DashboardTileItem> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<DashboardTileItem>();
                _forceResetAnimation = true;
                BuildLayout();
                StartAnimation();
            }
        }

        [Category("Layout"), DefaultValue(320)]
        public int CardWidth
        {
            get => _cardWidth;
            set { _cardWidth = Math.Max(120, value); BuildLayout(); EnsureAnimationTimerRunningIfNeeded(); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(120)]
        public int CardHeight
        {
            get => _cardHeight;
            set { _cardHeight = Math.Max(80, value); BuildLayout(); EnsureAnimationTimerRunningIfNeeded(); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(18)]
        public int CardSpacing
        {
            get => _cardSpacing;
            set { _cardSpacing = Math.Max(0, value); BuildLayout(); EnsureAnimationTimerRunningIfNeeded(); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(18)]
        public int CardRadius
        {
            get => _cardRadius;
            set { _cardRadius = Math.Max(1, value); Invalidate(); }
        }

        [Category("Appearance"), Description("رنگ استروک (خط دور) کارت هنگام قرار گرفتن موس روی آن")]
        public Color HoverStrokeColor
        {
            get => _hoverStrokeColor;
            set { _hoverStrokeColor = value; Invalidate(); }
        }

        // برای رفع خطای DesignerSerializationVisibility WFO1000
        private bool ShouldSerializeHoverStrokeColor() => _hoverStrokeColor != Color.FromArgb(135, 206, 250);
        private void ResetHoverStrokeColor() => _hoverStrokeColor = Color.FromArgb(135, 206, 250);

        [Category("Animation"), DefaultValue(380)]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(50, value);
        }

        [Category("Animation"), DefaultValue(30)]
        public int AnimationStartOffsetY
        {
            get => _animationStartOffsetY;
            set { _animationStartOffsetY = Math.Max(0, value); BuildLayout(); EnsureAnimationTimerRunningIfNeeded(); Invalidate(); }
        }

        [Category("Animation"), DefaultValue(90)]
        public int StaggerDelay
        {
            get => _staggerDelay;
            set => _staggerDelay = Math.Max(0, value);
        }

        [Category("Layout"), DefaultValue(20)]
        public int OuterPadding
        {
            get => _outerPadding;
            set { _outerPadding = Math.Max(0, value); BuildLayout(); EnsureAnimationTimerRunningIfNeeded(); Invalidate(); }
        }

        [Category("Animation"), DefaultValue(5), Description("میزان جابجایی کارت به سمت بالا هنگام قرار گرفتن موس روی آن")]
        public int HoverOffset
        {
            get => _hoverOffset;
            set { _hoverOffset = Math.Max(0, value); Invalidate(); }
        }

        [Category("Animation"), DefaultValue(0.30f), Description("سرعت انیمیشن Hover. مقادیر بزرگتر سریع‌تر هستند.")]
        public float HoverAnimationStep
        {
            get => _hoverAnimationStep;
            set => _hoverAnimationStep = Math.Max(0.01f, Math.Min(1f, value));
        }

        // --- Events ---
        public event EventHandler<DashboardTileItemClickedEventArgs> ItemClicked;

        // --- Public Methods ---
        public void SetItems(IEnumerable<DashboardTileItem> items)
        {
            _items = items?.Where(x => x != null && x.Visible).ToList() ?? new List<DashboardTileItem>();
            _forceResetAnimation = true;
            BuildLayout();
            StartAnimation();
        }

        public void ReloadAnimation()
        {
            _forceResetAnimation = true;
            BuildLayout();
            StartAnimation();
        }

        // --- Overrides ---
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            EnsureAnimationTimerRunningIfNeeded();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _animationTimer.Stop();
            _stopwatch.Stop();
            base.OnHandleDestroyed(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible) EnsureAnimationTimerRunningIfNeeded();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            BuildLayout();
            EnsureAnimationTimerRunningIfNeeded();
            Invalidate();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            BuildLayout();
            EnsureAnimationTimerRunningIfNeeded();
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool changed = false;
            bool overAnyTile = false;
            foreach (TileVisualInfo tile in _tiles)
            {
                bool hover = tile.CurrentBounds.Contains(e.Location);
                if (hover) overAnyTile = true;
                if (tile.Hovered != hover) { tile.Hovered = hover; changed = true; }
            }
            Cursor = overAnyTile ? Cursors.Hand : Cursors.Default;
            if (changed) EnsureAnimationTimerRunningIfNeeded();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool changed = false;
            foreach (TileVisualInfo tile in _tiles) { if (tile.Hovered) { tile.Hovered = false; changed = true; } }
            Cursor = Cursors.Default;
            if (changed) EnsureAnimationTimerRunningIfNeeded();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            foreach (TileVisualInfo tile in _tiles)
            {
                if (tile.CurrentBounds.Contains(e.Location))
                {
                    ItemClicked?.Invoke(this, new DashboardTileItemClickedEventArgs(tile.Item));
                    return;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            foreach (TileVisualInfo tile in _tiles) DrawTile(e.Graphics, tile);
        }

        // --- Private Methods ---
        private void BuildLayout()
        {
            Dictionary<string, TileVisualInfo> previousTiles = new Dictionary<string, TileVisualInfo>();
            if (!_forceResetAnimation)
            {
                previousTiles = _tiles
                    .Where(t => t != null && t.Item != null)
                    .GroupBy(t => t.Item.Code) // Assuming DashboardTileItem has a unique Id property
                    .ToDictionary(group => group.Key, group => group.Last());
            }

            _tiles.Clear();
            List<DashboardTileItem> visibleItems = _items.Where(x => x != null && x.Visible).ToList();

            if (visibleItems.Count == 0 || ClientSize.Width <= 0 || ClientSize.Height < 0)
            {
                AutoScrollMinSize = Size.Empty;
                return;
            }

            int availableWidth = Math.Max(1, ClientSize.Width - (_outerPadding * 2));
            int columns = Math.Max(1, (availableWidth + _cardSpacing) / (_cardWidth + _cardSpacing));
            int actualGridWidth = (columns * _cardWidth) + Math.Max(0, columns - 1) * _cardSpacing;

            int startX = RightToLeft == RightToLeft.Yes
                ? ClientSize.Width - _outerPadding - actualGridWidth
                : _outerPadding;

            for (int i = 0; i < visibleItems.Count; i++)
            {
                DashboardTileItem item = visibleItems[i];
                int row = i / columns;
                int colInSequence = i % columns;
                int visualCol = RightToLeft == RightToLeft.Yes ? columns - 1 - colInSequence : colInSequence;

                int x = startX + visualCol * (_cardWidth + _cardSpacing);
                int y = _outerPadding + row * (_cardHeight + _cardSpacing);
                int animationOrder = row * columns + colInSequence;

                Rectangle target = new Rectangle(x, y, _cardWidth, _cardHeight);

                float opacity = 0f;
                float hoverProgress = 0f;
                bool hovered = false;
                Rectangle current = new Rectangle(target.X, target.Y + _animationStartOffsetY, target.Width, target.Height);

                if (!_forceResetAnimation && previousTiles.TryGetValue(item.Code, out TileVisualInfo previous))
                {
                    opacity = Clamp01(previous.Opacity);
                    hoverProgress = Clamp01(previous.HoverProgress);
                    hovered = previous.Hovered;
                    int introOffsetY = (int)((1f - opacity) * _animationStartOffsetY);
                    int hoverOffsetY = (int)(hoverProgress * _hoverOffset);
                    current = new Rectangle(target.X, target.Y + introOffsetY - hoverOffsetY, target.Width, target.Height);
                }

                _tiles.Add(new TileVisualInfo
                {
                    Item = item,
                    TargetBounds = target,
                    CurrentBounds = current,
                    Opacity = opacity,
                    Delay = animationOrder * _staggerDelay,
                    Hovered = hovered,
                    HoverProgress = hoverProgress
                });
            }

            int rows = (int)Math.Ceiling(visibleItems.Count / (double)columns);
            int totalHeight = _outerPadding * 2 + rows * _cardHeight + Math.Max(0, rows - 1) * _cardSpacing;
            AutoScrollMinSize = new Size(0, totalHeight);
        }

        private void StartAnimation()
        {
            _forceResetAnimation = false;
            if (_tiles.Count == 0) { _animationTimer.Stop(); _stopwatch.Reset(); Invalidate(); return; }

            foreach (TileVisualInfo tile in _tiles)
            {
                tile.Opacity = 0f;
                tile.HoverProgress = 0f;
                tile.CurrentBounds = new Rectangle(tile.TargetBounds.X, tile.TargetBounds.Y + _animationStartOffsetY, tile.TargetBounds.Width, tile.TargetBounds.Height);
            }
            _stopwatch.Restart();
            _animationTimer.Start();
            Invalidate();
        }

        private void EnsureAnimationTimerRunningIfNeeded()
        {
            if (!IsHandleCreated || !Visible || _tiles.Count == 0) return;

            bool introRunning = _tiles.Any(tile => tile.Opacity < 0.999f);
            bool hoverRunning = _tiles.Any(tile => (tile.Hovered && tile.HoverProgress < 0.999f) || (!tile.Hovered && tile.HoverProgress > 0.001f));
            bool needTimer = introRunning || hoverRunning;

            if (!needTimer) return;
            if (!_stopwatch.IsRunning) _stopwatch.Start();
            if (!_animationTimer.Enabled) _animationTimer.Start();
            Invalidate();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_tiles.Count == 0) { _animationTimer.Stop(); return; }

            long elapsed = _stopwatch.ElapsedMilliseconds;
            bool anyRunning = false;

            foreach (TileVisualInfo tile in _tiles)
            {
                float introProgress = (elapsed <= tile.Delay) ? 0f : (elapsed - tile.Delay) / (float)_animationDuration;
                if (introProgress < 1f && elapsed > tile.Delay) anyRunning = true;
                introProgress = Clamp01(introProgress);

                if (tile.Opacity < 0.999f || introProgress >= 1f)
                {
                    float easedIntro = EaseOutCubic(introProgress);
                    tile.Opacity = Math.Max(tile.Opacity, easedIntro);
                }
                if (tile.Opacity < 0.999f) anyRunning = true;

                float targetHover = tile.Hovered ? 1f : 0f;
                if (Math.Abs(tile.HoverProgress - targetHover) > 0.001f)
                {
                    tile.HoverProgress = targetHover > tile.HoverProgress
                        ? Math.Min(targetHover, tile.HoverProgress + _hoverAnimationStep)
                        : Math.Max(targetHover, tile.HoverProgress - _hoverAnimationStep);
                    anyRunning = true;
                }
                else { tile.HoverProgress = targetHover; }

                int introOffsetY = (int)((1f - tile.Opacity) * _animationStartOffsetY);
                int hoverOffsetY = (int)(tile.HoverProgress * _hoverOffset);
                int currentY = tile.TargetBounds.Y + introOffsetY - hoverOffsetY;
                tile.CurrentBounds = new Rectangle(tile.TargetBounds.X, currentY, tile.TargetBounds.Width, tile.TargetBounds.Height);
            }

            Invalidate();
            if (!anyRunning) _animationTimer.Stop();
        }

        private void DrawTile(Graphics g, TileVisualInfo tile)
        {
            Rectangle rect = tile.CurrentBounds;
            int alpha = (int)(tile.Opacity * 255f);
            alpha = Math.Max(0, Math.Min(255, alpha));
            if (alpha <= 0) return;

            int r = (int)(228 + (255 - 228) * tile.HoverProgress);
            int gr = (int)(231 + (255 - 231) * tile.HoverProgress);
            int b = (int)(235 + (255 - 235) * tile.HoverProgress);
            Color baseCardColor = Color.FromArgb(r, gr, b);

            float hover = tile.HoverProgress;

            // افکت سایه نرم که از لبه شروع به محو شدن می‌کند (فقط در صورت Hover)
            if (hover > 0.01f)
            {
                // تنظیم مقادیر برای شروع دقیق‌تر محو شدگی از لبه
                // yOffset: کمترین مقدار ممکن برای نزدیک شدن به لبه بالایی
                // spread: مقداری که تعیین کننده گستردگی محو شدگی است
                int yOffset = (int)(2 + hover * 3);  // کاهش مقدار پایه برای نزدیکتر شدن به لبه
                int spread = (int)(6 + hover * 6);   // تنظیم میزان پخش‌شدگی
                int maxAlpha = (int)(alpha * (0.15f + hover * 0.05f)); // تنظیم شفافیت

                DrawSoftShadow(g, rect, _cardRadius, yOffset, spread, maxAlpha);
            }

            using (GraphicsPath cardPath = CreateRoundRectangle(rect, _cardRadius))
            {
                using (SolidBrush cardBrush = new SolidBrush(Color.FromArgb(alpha, baseCardColor)))
                {
                    g.FillPath(cardBrush, cardPath);
                }

                // استروک آبی فقط هنگام Hover
                if (hover > 0.01f)
                {
                    int strokeAlpha = (int)(hover * alpha);
                    strokeAlpha = Math.Max(0, Math.Min(255, strokeAlpha));
                    using (Pen strokePen = new Pen(Color.FromArgb(strokeAlpha, _hoverStrokeColor), 1f))
                    {
                        strokePen.Alignment = PenAlignment.Inset;
                        g.DrawPath(strokePen, cardPath);
                    }
                }
            }

            // --- Draw Content (Icon and Text) ---
            Rectangle imageRect, titleRect, descRect;
            bool rtl = RightToLeft == RightToLeft.Yes;

            if (rtl)
            {
                imageRect = new Rectangle(rect.Right - 105, rect.Y + 14, 90, 90);
                titleRect = new Rectangle(rect.X + 18, rect.Y + 14, rect.Width - 130, 34);
                descRect = new Rectangle(rect.X + 18, rect.Y + 50, rect.Width - 130, 48);
            }
            else
            {
                imageRect = new Rectangle(rect.X + 15, rect.Y + 14, 90, 90);
                titleRect = new Rectangle(rect.X + 118, rect.Y + 14, rect.Width - 130, 34);
                descRect = new Rectangle(rect.X + 118, rect.Y + 50, rect.Width - 130, 48);
            }

            DrawImage(g, tile.Item.Icon, imageRect, alpha);

            using (StringFormat titleFormat = new StringFormat() { Alignment = rtl ? StringAlignment.Far : StringAlignment.Near, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap })
            using (StringFormat descFormat = new StringFormat() { Alignment = rtl ? StringAlignment.Far : StringAlignment.Near, LineAlignment = StringAlignment.Near })
            using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(alpha, 30, 30, 30)))
            using (SolidBrush descBrush = new SolidBrush(Color.FromArgb(alpha, 90, 90, 90)))
            using (Font titleFont = new Font("Tahoma", 16f, FontStyle.Bold))
            using (Font descFont = new Font("Tahoma", 10.2f, FontStyle.Regular))
            {
                g.DrawString(tile.Item.Title ?? string.Empty, titleFont, titleBrush, titleRect, titleFormat);
                g.DrawString(tile.Item.Description ?? string.Empty, descFont, descBrush, descRect, descFormat);
            }
        }

        /// <summary>
        /// Draws a soft shadow that fades out from the edges of the rectangle.
        /// </summary>
        private void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int yOffset, int spread, int maxAlpha)
        {
            if (maxAlpha <= 0) return;

            spread = Math.Max(1, spread);
            radius = Math.Max(1, radius);

            // Base rectangle for shadow calculation, offset by yOffset
            Rectangle baseRect = new Rectangle(rect.X, rect.Y + yOffset, rect.Width, rect.Height);
            const int layers = 10; // Number of shadow layers for smooth fading

            for (int i = layers; i >= 1; i--)
            {
                float t = i / (float)layers; // Current layer's progress (1.0 for innermost, 0.0 for outermost)
                // Calculate alpha for the current layer. Using t*t for a more pronounced fade at the edges.
                int alpha = (int)(maxAlpha * t * t * 0.90f);
                alpha = Math.Max(0, Math.Min(255, alpha)); // Clamp alpha to 0-255

                if (alpha == 0) continue; // Skip if alpha is zero

                // Calculate the growth (spread) of the shadow for the current layer
                int grow = (int)((1f - t) * spread);

                // Calculate the shadow rectangle, expanded by 'grow' on all sides
                Rectangle shadowRect = new Rectangle(
                    baseRect.X - grow,
                    baseRect.Y - grow,
                    baseRect.Width + grow * 2,
                    baseRect.Height + grow * 2);

                // Calculate the rounded radius for the shadow, increasing with 'grow'
                int shadowRadius = radius + grow;

                // Create a rounded rectangle path for the shadow
                using (GraphicsPath path = CreateRoundRectangle(shadowRect, shadowRadius))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0))) // Shadow color is black
                {
                    g.FillPath(brush, path); // Fill the shadow path
                }
            }
        }

        private void DrawImage(Graphics g, Image image, Rectangle rect, int alpha)
        {
            if (image == null || rect.Width <= 0 || rect.Height <= 0) return;

            using (GraphicsPath path = CreateRoundRectangle(rect, 14)) // 14 is the radius for image corners
            {
                GraphicsState state = g.Save();
                try
                {
                    g.SetClip(path); // Clip drawing to the rounded rectangle
                    using (ImageAttributesWrapper attributes = new ImageAttributesWrapper(alpha / 255f))
                    {
                        g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes.ImageAttributes);
                    }
                }
                finally { g.Restore(state); }
            }
        }

        private GraphicsPath CreateRoundRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            radius = Math.Max(1, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static float Clamp01(float value) => Math.Max(0f, Math.Min(1f, value));
        private static float EaseOutCubic(float t) { t = Clamp01(t); return 1f - (float)Math.Pow(1f - t, 3); }
    }

    // --- Helper Classes ---
    internal class TileVisualInfo
    {
        public DashboardTileItem Item { get; set; }
        public Rectangle TargetBounds { get; set; }
        public Rectangle CurrentBounds { get; set; }
        public float Opacity { get; set; }
        public int Delay { get; set; }
        public bool Hovered { get; set; }
        public float HoverProgress { get; set; }
    }

    internal sealed class ImageAttributesWrapper : IDisposable
    {
        public System.Drawing.Imaging.ImageAttributes ImageAttributes { get; }
        public ImageAttributesWrapper(float opacity)
        {
            ImageAttributes = new System.Drawing.Imaging.ImageAttributes();
            var matrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = Math.Max(0f, Math.Min(1f, opacity)) };
            ImageAttributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
        }
        public void Dispose() => ImageAttributes?.Dispose();
    }
}

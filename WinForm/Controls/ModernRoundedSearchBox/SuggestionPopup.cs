using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinForm.Controls.ModernRoundedSearchBox
{
    internal class SuggestionPopup :
        Form,
        IMessageFilter
    {
        private readonly List<SearchSuggestion> _items =
            new List<SearchSuggestion>();

        private int _hoveredIndex = -1;

        private int _scrollOffset = 0;

        private bool _scrollDragging;

        private int _scrollDragStartY;

        private int _scrollDragStartOffset;

        private Color _backColor =
            Color.White;

        private Color _borderColor =
            Color.FromArgb(220, 220, 220);

        private Color _hoverBackColor =
            Color.FromArgb(240, 240, 240);

        private Color _textColor =
            Color.FromArgb(40, 40, 40);

        private Color _hoverTextColor =
            Color.FromArgb(30, 30, 30);

        private Color _iconColor =
            Color.FromArgb(80, 80, 80);

        private Color _scrollBarColor =
            Color.FromArgb(180, 180, 180);

        private Color _scrollBarHoverColor =
            Color.FromArgb(120, 120, 120);

        private int _itemHeight = 38;

        private int _maxItems = 8;

        private int _iconSize = 18;

        private int _iconTextSpacing = 10;

        private int _borderRadius = 6;

        private const int ScrollBarWidth = 6;

        private const int ScrollBarRightMargin = 4;

        private const int ScrollBarMinThumbHeight = 28;

        private bool _messageFilterRegistered;

        public event EventHandler<SearchSuggestion>?
            ItemClicked;

        public SuggestionPopup()
        {
            FormBorderStyle =
                FormBorderStyle.None;

            ShowInTaskbar = false;

            ShowIcon = false;

            StartPosition =
                FormStartPosition.Manual;

            TopMost = false;

            DoubleBuffered = true;

            BackColor =
                _backColor;

            Padding =
                new Padding(1);

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            MouseMove +=
                SuggestionPopup_MouseMove;

            MouseLeave +=
                SuggestionPopup_MouseLeave;

            MouseClick +=
                SuggestionPopup_MouseClick;

            MouseWheel +=
                SuggestionPopup_MouseWheel;

            MouseDown +=
                SuggestionPopup_MouseDown;

            MouseUp +=
                SuggestionPopup_MouseUp;
        }

        #region Prevent Activation

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp =
                    base.CreateParams;

                const int WS_EX_NOACTIVATE =
                    0x08000000;

                cp.ExStyle |=
                    WS_EX_NOACTIVATE;

                return cp;
            }
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionBackColor
        {
            get => _backColor;

            set
            {
                _backColor = value;
                BackColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionBorderColor
        {
            get => _borderColor;

            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionHoverBackColor
        {
            get => _hoverBackColor;

            set
            {
                _hoverBackColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionTextColor
        {
            get => _textColor;

            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionHoverTextColor
        {
            get => _hoverTextColor;

            set
            {
                _hoverTextColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Color SuggestionIconColor
        {
            get => _iconColor;

            set
            {
                _iconColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public int SuggestionItemHeight
        {
            get => _itemHeight;

            set
            {
                if (value < 20)
                    return;

                _itemHeight = value;

                UpdatePopupSize();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public int SuggestionMaxItems
        {
            get => _maxItems;

            set
            {
                if (value < 1)
                    return;

                _maxItems = value;

                EnsureValidScrollOffset();

                UpdatePopupSize();

                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public int SuggestionIconSize
        {
            get => _iconSize;

            set
            {
                if (value <= 0)
                    return;

                _iconSize = value;

                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public int SuggestionIconTextSpacing
        {
            get => _iconTextSpacing;

            set
            {
                if (value < 0)
                    return;

                _iconTextSpacing = value;

                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
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

        #endregion

        #region Items

        public void SetItems(
            IEnumerable<SearchSuggestion> items)
        {
            _items.Clear();

            if (items != null)
                _items.AddRange(items);

            _hoveredIndex = -1;

            _scrollOffset = 0;

            _scrollDragging = false;

            UpdatePopupSize();

            Invalidate();
        }

        #endregion

        #region Popup

        public void ShowPopup(
            Control owner,
            Point location)
        {
            if (_items.Count == 0)
            {
                Hide();
                return;
            }

            UpdatePopupSize();

            Point screenPoint =
                owner.PointToScreen(location);

            Location =
                screenPoint;

            RegisterMessageFilter();

            if (!Visible)
            {
                Show(owner);
            }
            else
            {
                Invalidate();
            }
        }

        private void UpdatePopupSize()
        {
            int count =
                Math.Min(
                    _items.Count,
                    _maxItems);

            Height =
                count * _itemHeight + 2;

            if (Width < 200)
                Width = 200;

            EnsureValidScrollOffset();
        }

        #endregion

        #region Outside Click Message Filter

        private void RegisterMessageFilter()
        {
            if (_messageFilterRegistered)
                return;

            Application.AddMessageFilter(this);

            _messageFilterRegistered = true;
        }

        private void UnregisterMessageFilter()
        {
            if (!_messageFilterRegistered)
                return;

            Application.RemoveMessageFilter(this);

            _messageFilterRegistered = false;
        }

        public bool PreFilterMessage(
            ref Message m)
        {
            if (!Visible)
                return false;

            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_MBUTTONDOWN = 0x0207;
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int WM_NCRBUTTONDOWN = 0x00A4;
            const int WM_NCMBUTTONDOWN = 0x00A7;

            bool isMouseDown =
                m.Msg == WM_LBUTTONDOWN ||
                m.Msg == WM_RBUTTONDOWN ||
                m.Msg == WM_MBUTTONDOWN ||
                m.Msg == WM_NCLBUTTONDOWN ||
                m.Msg == WM_NCRBUTTONDOWN ||
                m.Msg == WM_NCMBUTTONDOWN;

            if (!isMouseDown)
                return false;

            Point mousePosition =
                Cursor.Position;

            if (!Bounds.Contains(mousePosition))
            {
                Hide();
            }

            return false;
        }

        protected override void OnVisibleChanged(
            EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                RegisterMessageFilter();
            else
                UnregisterMessageFilter();
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            UnregisterMessageFilter();

            base.OnFormClosed(e);
        }

        #endregion

        #region Scroll

        private bool HasScrollBar
        {
            get
            {
                return _items.Count > _maxItems;
            }
        }

        private int MaxScrollOffset
        {
            get
            {
                return Math.Max(
                    0,
                    _items.Count - _maxItems);
            }
        }

        private Rectangle GetScrollTrackRectangle()
        {
            if (!HasScrollBar)
                return Rectangle.Empty;

            int x =
                Width -
                ScrollBarWidth -
                ScrollBarRightMargin;

            return new Rectangle(
                x,
                5,
                ScrollBarWidth,
                Math.Max(
                    0,
                    Height - 10));
        }

        private Rectangle GetScrollThumbRectangle()
        {
            Rectangle track =
                GetScrollTrackRectangle();

            if (track.IsEmpty)
                return Rectangle.Empty;

            if (MaxScrollOffset <= 0)
                return track;

            float visibleRatio =
                (float)_maxItems /
                _items.Count;

            int thumbHeight =
                Math.Max(
                    ScrollBarMinThumbHeight,
                    (int)(
                        track.Height *
                        visibleRatio));

            thumbHeight =
                Math.Min(
                    thumbHeight,
                    track.Height);

            int available =
                track.Height -
                thumbHeight;

            int top =
                track.Top;

            if (available > 0)
            {
                float ratio =
                    (float)_scrollOffset /
                    MaxScrollOffset;

                top +=
                    (int)(
                        available *
                        ratio);
            }

            return new Rectangle(
                track.Left,
                top,
                track.Width,
                thumbHeight);
        }

        private void SetScrollOffset(
            int value)
        {
            int max =
                MaxScrollOffset;

            _scrollOffset =
                Math.Max(
                    0,
                    Math.Min(
                        value,
                        max));

            Invalidate();
        }

        private void SuggestionPopup_MouseWheel(
            object? sender,
            MouseEventArgs e)
        {
            if (!HasScrollBar ||
                e.Delta == 0)
                return;

            int steps =
                Math.Max(
                    1,
                    Math.Abs(e.Delta) /
                    120);

            if (e.Delta > 0)
                SetScrollOffset(
                    _scrollOffset - steps);
            else
                SetScrollOffset(
                    _scrollOffset + steps);
        }

        private void EnsureValidScrollOffset()
        {
            _scrollOffset =
                Math.Max(
                    0,
                    Math.Min(
                        _scrollOffset,
                        MaxScrollOffset));
        }

        #endregion

        #region Mouse ScrollBar

        private void SuggestionPopup_MouseDown(
            object? sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                !HasScrollBar)
                return;

            Rectangle thumb =
                GetScrollThumbRectangle();

            Rectangle track =
                GetScrollTrackRectangle();

            if (thumb.Contains(e.Location))
            {
                _scrollDragging = true;

                _scrollDragStartY =
                    e.Y;

                _scrollDragStartOffset =
                    _scrollOffset;

                Capture = true;

                Cursor =
                    Cursors.Hand;

                return;
            }

            if (track.Contains(e.Location))
            {
                if (e.Y < thumb.Top)
                {
                    SetScrollOffset(
                        _scrollOffset -
                        _maxItems);
                }
                else if (e.Y > thumb.Bottom)
                {
                    SetScrollOffset(
                        _scrollOffset +
                        _maxItems);
                }
            }
        }

        private void SuggestionPopup_MouseUp(
            object? sender,
            MouseEventArgs e)
        {
            if (!_scrollDragging)
                return;

            _scrollDragging = false;

            Capture = false;

            Cursor = Cursors.Default;
        }

        private void UpdateScrollByDrag(
            int currentY)
        {
            Rectangle track =
                GetScrollTrackRectangle();

            Rectangle thumb =
                GetScrollThumbRectangle();

            int available =
                track.Height -
                thumb.Height;

            if (available <= 0)
                return;

            int delta =
                currentY -
                _scrollDragStartY;

            float ratio =
                (float)delta /
                available;

            int offsetDelta =
                (int)Math.Round(
                    ratio *
                    MaxScrollOffset);

            SetScrollOffset(
                _scrollDragStartOffset +
                offsetDelta);
        }

        #endregion

        #region Mouse

        private void SuggestionPopup_MouseMove(
            object? sender,
            MouseEventArgs e)
        {
            if (_scrollDragging)
            {
                UpdateScrollByDrag(e.Y);
                return;
            }

            if (HasScrollBar &&
                GetScrollThumbRectangle()
                    .Contains(e.Location))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                int index =
                    GetItemIndex(e.Location);

                Cursor =
                    index >= 0
                        ? Cursors.Hand
                        : Cursors.Default;

                if (index != _hoveredIndex)
                {
                    _hoveredIndex = index;

                    Invalidate();
                }
            }
        }

        private void SuggestionPopup_MouseLeave(
            object? sender,
            EventArgs e)
        {
            if (_scrollDragging)
                return;

            if (_hoveredIndex != -1)
            {
                _hoveredIndex = -1;

                Cursor =
                    Cursors.Default;

                Invalidate();
            }
        }

        private void SuggestionPopup_MouseClick(
            object? sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (HasScrollBar &&
                GetScrollTrackRectangle()
                    .Contains(e.Location))
            {
                return;
            }

            int index =
                GetItemIndex(e.Location);

            if (index < 0 ||
                index >= _items.Count)
            {
                return;
            }

            SearchSuggestion suggestion =
                _items[index];

            ItemClicked?.Invoke(
                this,
                suggestion);
        }

        private int GetItemIndex(
            Point location)
        {
            if (location.Y < 1)
                return -1;

            if (HasScrollBar &&
                GetScrollTrackRectangle()
                    .Contains(location))
            {
                return -1;
            }

            int localIndex =
                (location.Y - 1) /
                _itemHeight;

            int index =
                localIndex +
                _scrollOffset;

            int visibleCount =
                Math.Min(
                    _items.Count,
                    _maxItems);

            if (localIndex < 0 ||
                localIndex >= visibleCount)
            {
                return -1;
            }

            if (index < 0 ||
                index >= _items.Count)
            {
                return -1;
            }

            return index;
        }

        #endregion

        #region Deactivate

        protected override void OnDeactivate(
            EventArgs e)
        {
            base.OnDeactivate(e);

            /*
             * Popup نباید Activate شود.
             * اینجا فقط در صورتی که واقعاً
             * Deactivate اتفاق افتاد Popup بسته می‌شود.
             */
            if (Visible)
                Hide();
        }

        #endregion

        #region Painting

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g =
                e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle borderRect =
                new Rectangle(
                    0,
                    0,
                    Width - 1,
                    Height - 1);

            using GraphicsPath path =
                GetRoundedPath(
                    borderRect,
                    _borderRadius);

            using SolidBrush brush =
                new SolidBrush(
                    _backColor);

            g.FillPath(
                brush,
                path);

            DrawItems(g);

            DrawAndroidScrollBar(g);

            using Pen pen =
                new Pen(
                    _borderColor,
                    1);

            pen.Alignment =
                PenAlignment.Inset;

            g.DrawPath(
                pen,
                path);
        }

        private void DrawItems(
            Graphics g)
        {
            int visibleCount =
                Math.Min(
                    _items.Count,
                    _maxItems);

            int startIndex =
                _scrollOffset;

            int endIndex =
                Math.Min(
                    startIndex +
                    visibleCount,
                    _items.Count);

            int itemWidth =
                Width - 2;

            if (HasScrollBar)
            {
                itemWidth -=
                    ScrollBarWidth +
                    ScrollBarRightMargin +
                    5;
            }

            for (
                int i = startIndex;
                i < endIndex;
                i++)
            {
                int localIndex =
                    i - startIndex;

                Rectangle itemRect =
                    new Rectangle(
                        1,
                        1 +
                        localIndex *
                        _itemHeight,
                        itemWidth,
                        _itemHeight);

                DrawItem(
                    g,
                    itemRect,
                    _items[i],
                    i == _hoveredIndex);
            }
        }

        private void DrawAndroidScrollBar(
            Graphics g)
        {
            if (!HasScrollBar)
                return;

            Rectangle thumb =
                GetScrollThumbRectangle();

            using SolidBrush brush =
                new SolidBrush(
                    _scrollBarColor);

            using GraphicsPath path =
                GetRoundedPath(
                    thumb,
                    ScrollBarWidth);

            g.FillPath(
                brush,
                path);

            if (_scrollDragging)
            {
                using SolidBrush hoverBrush =
                    new SolidBrush(
                        _scrollBarHoverColor);

                using GraphicsPath hoverPath =
                    GetRoundedPath(
                        thumb,
                        ScrollBarWidth);

                g.FillPath(
                    hoverBrush,
                    hoverPath);
            }
        }

        private void DrawItem(
            Graphics g,
            Rectangle bounds,
            SearchSuggestion suggestion,
            bool hovered)
        {
            Color backgroundColor =
                hovered
                    ? _hoverBackColor
                    : _backColor;

            using SolidBrush backgroundBrush =
                new SolidBrush(
                    backgroundColor);

            g.FillRectangle(
                backgroundBrush,
                bounds);

            int iconSize =
                suggestion.IconSize > 0
                    ? suggestion.IconSize
                    : _iconSize;

            Color iconColor =
                !suggestion.IconColor.IsEmpty
                    ? suggestion.IconColor
                    : _iconColor;

            Color textColor =
                !suggestion.TextColor.IsEmpty
                    ? suggestion.TextColor
                    : hovered
                        ? _hoverTextColor
                        : _textColor;

            int left =
                bounds.Left + 12;

            if (suggestion.Icon !=
                IconChar.None)
            {
                DrawIcon(
                    g,
                    suggestion.Icon,
                    suggestion.IconFont,
                    iconColor,
                    iconSize,
                    left,
                    bounds,
                    out int iconRight);

                left =
                    iconRight +
                    _iconTextSpacing;
            }

            Rectangle textRect =
                new Rectangle(
                    left,
                    bounds.Top,
                    Math.Max(
                        0,
                        bounds.Right -
                        left -
                        10),
                    bounds.Height);

            TextFormatFlags flags =
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix;

            TextRenderer.DrawText(
                g,
                suggestion.Text ??
                string.Empty,
                Font,
                textRect,
                textColor,
                flags);
        }

        private static void DrawIcon(
            Graphics g,
            IconChar icon,
            IconFont iconFont,
            Color color,
            int size,
            int left,
            Rectangle itemBounds,
            out int right)
        {
            right =
                left + size;

            if (icon == IconChar.None ||
                size <= 0)
            {
                return;
            }

            try
            {
                using Bitmap bitmap =
                    icon.ToBitmap(
                        iconFont,
                        size,
                        color);

                int top =
                    itemBounds.Top +
                    (itemBounds.Height -
                     bitmap.Height) / 2;

                g.DrawImage(
                    bitmap,
                    new Rectangle(
                        left,
                        top,
                        bitmap.Width,
                        bitmap.Height));
            }
            catch
            {
                // Ignore icon rendering errors.
            }
        }

        #endregion

        #region Rounded Border

        private static GraphicsPath GetRoundedPath(
            Rectangle rect,
            int radius)
        {
            GraphicsPath path =
                new GraphicsPath();

            if (rect.Width <= 0 ||
                rect.Height <= 0)
            {
                return path;
            }

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter =
                radius * 2;

            diameter =
                Math.Min(
                    diameter,
                    Math.Min(
                        rect.Width,
                        rect.Height));

            path.StartFigure();

            path.AddArc(
                rect.X,
                rect.Y,
                diameter,
                diameter,
                180,
                90);

            path.AddArc(
                rect.Right - diameter,
                rect.Y,
                diameter,
                diameter,
                270,
                90);

            path.AddArc(
                rect.Right - diameter,
                rect.Bottom - diameter,
                diameter,
                diameter,
                0,
                90);

            path.AddArc(
                rect.X,
                rect.Bottom - diameter,
                diameter,
                diameter,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        #endregion

        #region Dispose

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                UnregisterMessageFilter();

                _items.Clear();

                _scrollDragging = false;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
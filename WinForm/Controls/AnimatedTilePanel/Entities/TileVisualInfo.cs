using System;
using System.Collections.Generic;
using System.Text;

namespace WinForm.Controls.AnimatedTilePanel.Entities
{
    public class TileVisualInfo
    {
        public DashboardTileItem Item { get; set; }
        public Rectangle TargetBounds { get; set; }
        public Rectangle CurrentBounds { get; set; }
        public float Opacity { get; set; }
        public int Delay { get; set; }
        public bool Hovered { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Controls.AnimatedTilePanel.Enums;

namespace WinForm.Controls.AnimatedTilePanel.Entities
{
    public class DashboardTileItem
    {
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Image Icon { get; set; }
        public DashboardTileItemType ItemType { get; set; }
        public bool Visible { get; set; } = true;
    }
}

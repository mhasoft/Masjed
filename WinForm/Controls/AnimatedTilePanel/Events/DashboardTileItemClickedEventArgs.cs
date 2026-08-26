using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Controls.AnimatedTilePanel.Entities;

namespace WinForm.Controls.AnimatedTilePanel.Events
{
    public class DashboardTileItemClickedEventArgs : EventArgs
    {
        public DashboardTileItemClickedEventArgs(DashboardTileItem item)
        {
            Item = item;
        }

        public DashboardTileItem Item { get; }
    }
}

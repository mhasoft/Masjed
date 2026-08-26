using System;
using WinForm.Controls.DashboardMenu.Entities;

namespace WinForm.Controls.DashboardMenu.Events
{
    public class DashboardMenuItemClickedEventArgs : EventArgs
    {
        public DashboardMenuItemClickedEventArgs(DashboardMenuItem item)
        {
            Item = item;
        }

        public DashboardMenuItem Item { get; private set; }
    }
}

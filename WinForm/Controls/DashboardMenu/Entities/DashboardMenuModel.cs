using System.ComponentModel;
using System.Drawing;

namespace WinForm.Controls.DashboardMenu.Entities
{
    public class DashboardMenuModel
    {
        public DashboardMenuModel()
        {
            Groups = new BindingList<DashboardMenuGroup>();

            FooterText = string.Empty;
            UserName = string.Empty;
            UserRole = string.Empty;
        }

        public Image Logo { get; set; }
        public string FooterText { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public Image ProfilePicture { get; set; }

        public BindingList<DashboardMenuGroup> Groups { get; private set; }
    }

    public class DashboardMenuGroup
    {
        public DashboardMenuGroup()
        {
            Items = new BindingList<DashboardMenuItem>();
            Visible = true;
            TitleColor = Color.Empty;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Image Icon { get; set; }
        public bool Visible { get; set; }
        public Color TitleColor { get; set; }
        public BindingList<DashboardMenuItem> Items { get; private set; }
    }

    public class DashboardMenuItem
    {
        public DashboardMenuItem()
        {
            Visible = true;
            TitleColor = Color.Empty;
            IconColor = Color.Empty;
            BackgroundColor = Color.Empty;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Image Icon { get; set; }
        public string NavigationKey { get; set; }
        public bool Visible { get; set; }
        public bool IsSelected { get; set; }
        public Color TitleColor { get; set; }
        public Color IconColor { get; set; }
        public Color BackgroundColor { get; set; }
    }
}

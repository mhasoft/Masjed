using System.ComponentModel;
using System.Drawing;

namespace WinForm.Controls.DashboardMenu.Themes
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DashboardSidebarTheme
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb(28, 39, 70);
        public Color UserNameColor { get; set; } = Color.White;
        public Color UserRoleColor { get; set; } = Color.FromArgb(152, 164, 192);
        public Color GroupTitleColor { get; set; } = Color.FromArgb(150, 162, 190);
        public Color ItemTextColor { get; set; } = Color.FromArgb(169, 181, 209);
        public Color ItemIconColor { get; set; } = Color.FromArgb(150, 162, 190);
        public Color SelectedItemTextColor { get; set; } = Color.FromArgb(48, 157, 255);
        public Color SelectedItemIconColor { get; set; } = Color.FromArgb(48, 157, 255);
        public Color SelectedItemBackgroundColor { get; set; } = Color.FromArgb(35, 53, 92);
        public Color SeparatorColor { get; set; } = Color.FromArgb(53, 68, 104);

        public Font GroupFont { get; set; } = new Font("Segoe UI", 8.5f);
        public Font ItemFont { get; set; } = new Font("Segoe UI", 10f);
        public Font UserNameFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font UserRoleFont { get; set; } = new Font("Segoe UI", 9f);

        public override string ToString()
        {
            return "Dashboard Sidebar Theme";
        }
    }
}

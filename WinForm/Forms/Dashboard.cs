using FontAwesome.Sharp;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using VideoWall.WinForms.UserControls.CustomeControls.CustomeTabControl;
using WinForm.Controls.AnimatedTilePanel.Entities;
using WinForm.Controls.AnimatedTilePanel.Enums;
using WinForm.Controls.AnimatedTilePanel.Events;
using WinForm.Controls.AnimatedTiles;
using WinForm.Controls.BreadcrumbBar;
using WinForm.Controls.DashboardMenu;
using WinForm.Controls.DashboardMenu.Entities;
using WinForm.Controls.DashboardMenu.Events;
using WinForm.Models.Entities;
using WinForm.Services.Convertors.CreateUserControlByName;
using WinForm.Services.License.getPermission;
using WinForm.Services.License.getPermission.DTOs;
using WinForm.Services.ShowMessage.getShowMessage.Model;
using WinForm.Services.Database.Select.getTileByCode;
using WinForm.Services.Database.Select.getTileByTitle;
using WinForm.Services.Database.Select.getTileByTitle.DTOs;
using WinForm.Services.Database.Select.getTilePath;
using WinForm.Services.Database.Select.getTilesBySubCode;
using WinForm.UserControls;

namespace WinForm.Forms
{
    public partial class Dashboard : Form
    {


        #region ====[ Defenitions ]====
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private ucDesktop _ucDesktop;
        #endregion ====[ Defenitions ]====

        #region ====[ this Base Methods ]====
        public Dashboard()
        {
            InitializeComponent();

            //منوی سمت راست کابر
            ConfigureSidebar();

            RegisterEvents();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            //DashboardSidebar1_MenuItemClicked(null, new DashboardMenuItemClickedEventArgs { Item})
            BeginInvoke(new Action(() =>
            {
                var item = dashboardSidebar1.Menu.Groups[0].Items[0];

                DashboardSidebar1_MenuItemClicked(
                    dashboardSidebar1,
                    new DashboardMenuItemClickedEventArgs(item)
                );
            }));
        }
        
        private void RegisterEvents()
        {
            Load += Form_Load;
            dashboardSidebar1.MenuItemClicked += DashboardSidebar1_MenuItemClicked;
        }
        #endregion ====[ this Base Methods ]====

        #region ====[ Public Methods ]===

        #endregion ====[ Public Methods ]===
        #region ====[ DashboardSidebar Methods ]===
        private void DashboardSidebar1_MenuItemClicked(object? sender, DashboardMenuItemClickedEventArgs e)
        {
            // دسترسی به آیتم کلیک شده از طریق e.Item
            DashboardMenuItem clickedItem = e.Item;

            // تشخیص کلید ناوبری یا شناسه منحصر‌به‌فرد گزینه کلیک شده
            string navKey = clickedItem.NavigationKey;

            // مدیریت تغییر وضعیت یا صفحات بر اساس گزینه کلیک شده
            switch (navKey)
            {
                case "UserDesktop":
                    panelLeft.Controls.Clear();
                    _ucDesktop.Dock = DockStyle.Fill;
                    panelLeft.Controls.Add(_ucDesktop);
                    break;
                
                case "AboutForm":
                    panelLeft.Controls.Clear();
                    ucAbout _ucAbout = new ucAbout();
                    _ucAbout.Dock = DockStyle.Fill;
                    panelLeft.Controls.Add(_ucAbout);
                    break;
                case "UserAccount":
                    panelLeft.Controls.Clear();
                    ucUserAccount _uucUserAccount = new ucUserAccount();
                    _uucUserAccount.Dock = DockStyle.Fill;
                    panelLeft.Controls.Add(_uucUserAccount);
                    break;
                case "PrivacyForm":
                    panelLeft.Controls.Clear();
                    ucPrivacy _ucPrivacy = new ucPrivacy();
                    _ucPrivacy.Dock = DockStyle.Fill;
                    panelLeft.Controls.Add(_ucPrivacy);
                    break;
                default:
                    MessageBox.Show($"گزینه تعریف نشده: {clickedItem.Title}");
                    break;
            }
        }
        private void ConfigureSidebar()
        {
            //dashboardSidebar1.Logo = Properties.Resources.CompanyLogo;
            _ucDesktop = new ucDesktop();

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(appPath, "Pictures\\DashboardSidebar", "UserProfileIcon.png");
            if (File.Exists(imagePath))
            {
                dashboardSidebar1.ProfilePicture = Image.FromFile(imagePath);
            }
            else
            {
                dashboardSidebar1.ProfilePicture = IconChar.User.ToBitmap(
                                                                        IconFont.Auto,
                                                                        18,
                                                                        Color.White);
            }
            dashboardSidebar1.UserName = "Joan Wilkins";
            dashboardSidebar1.UserRole = "مدیر سیستم";
            dashboardSidebar1.FooterText =
                "Master Admin Dashboard\r\n© 2021 All Rights Reserved";

            #region ----[ Groups ]----

            #region ....[ User Profile Group ]....
            DashboardMenuGroup userProfileGroup = new DashboardMenuGroup
            {
                Id = 1,
                Code = "APPTOOLS",
                Title = "امکانات برنامه",
                Visible = true
            };

            userProfileGroup.Items.Add(new DashboardMenuItem
            {
                Id = 1,
                Code = "APPTOOLS_DESKTOP",
                Title = "میزکار",
                NavigationKey = "UserDesktop",
                Icon = IconChar.Desktop.ToBitmap(
                                            IconFont.Auto,
                                            18,
                                            Color.White),
                Visible = true,
                IsSelected = true
            });
            #endregion ....[ Profile Group ]....

            #region ....[ Settings Group ]....
            DashboardMenuGroup settingsGroup = new DashboardMenuGroup
            {
                Id = 2,
                Code = "SETTINGS",
                Title = "تنظیمات",
                Visible = true,
                TitleColor = Color.LightGray
            };

            settingsGroup.Items.Add(new DashboardMenuItem
            {
                Id = 4,
                Code = "SETTINGS_USERS_ACCOUNT",
                Title = "حساب کاربری",
                NavigationKey = "UserAccount",
                Icon = IconChar.Passport.ToBitmap(
                                                IconFont.Auto,
                                                18,
                                                Color.White),
                Visible = true
            });
            settingsGroup.Items.Add(new DashboardMenuItem
            {
                Id = 4,
                Code = "SETTINGS_USERS_PRIVACY",
                Title = "حریم خصوصی",
                NavigationKey = "PrivacyForm",
                Icon = IconChar.Shield.ToBitmap(
                                                IconFont.Auto,
                                                18,
                                                Color.White),
                Visible = true
            });
            settingsGroup.Items.Add(new DashboardMenuItem
            {
                Id = 4,
                Code = "SETTINGS_ABOUT",
                Title = "درباره برنامه",
                NavigationKey = "AboutForm",
                Icon = IconChar.Info.ToBitmap(
                                                IconFont.Auto,
                                                18,
                                                Color.White),
                Visible = true
            });
            #endregion ....[ Settings Group ]....

            #endregion ----[ Groups ]----

            dashboardSidebar1.Menu.Groups.Clear();
            dashboardSidebar1.Menu.Groups.Add(userProfileGroup);
            dashboardSidebar1.Menu.Groups.Add(settingsGroup);

            dashboardSidebar1.RebuildMenu();

            dashboardSidebar1.Invalidate();
        }
        #endregion====[ DashboardSidebar Methods ]===


    }
}

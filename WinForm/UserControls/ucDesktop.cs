using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VideoWall.WinForms.UserControls.CustomeControls.CustomeTabControl;
using WinForm.Controls.AnimatedTilePanel.Entities;
using WinForm.Controls.AnimatedTilePanel.Enums;
using WinForm.Controls.AnimatedTilePanel.Events;
using WinForm.Controls.BreadcrumbBar;
using WinForm.Controls.DashboardMenu;
using WinForm.Controls.DashboardMenu.Entities;
using WinForm.Controls.DashboardMenu.Events;
using WinForm.Models.Entities;
using WinForm.Services.Convertors.CreateUserControlByName;
using WinForm.Services.Database.Select.getTileByCode;
using WinForm.Services.License.getPermission;
using WinForm.Services.License.getPermission.DTOs;
using WinForm.Services.ShowMessage.getShowMessage.Model;
using WinForm.Services.Database.Select.getTileByTitle;
using WinForm.Services.Database.Select.getTilePath;
using WinForm.Services.Database.Select.getTilesBySubCode;

namespace WinForm.UserControls
{
    public partial class ucDesktop : UserControl
    {
        #region ====[ Defenitions ]====
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        #endregion ====[ Defenitions ]====

        #region ====[ this Base Methods ]====
        public ucDesktop()
        {
            InitializeComponent();

            

            //ثبت ایونتها
            RegisterEvents();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            //CustomTab1_AddButtonClicked(customTab1, EventArgs.Empty);
            BeginInvoke(new Action(() =>
            {
                customTab1.PerformAddButtonClick();
            }));
        }
        #endregion ====[ this Base Methods ]====

        #region ====[ Public Methods ]===
        private void RegisterEvents()
        {
            Load += Form_Load;
            //dashboardSidebar1.MenuItemClicked += DashboardSidebar1_MenuItemClicked;
            //animatedTilePanel1.ItemClicked += AnimatedTilePanel1_ItemClicked;

            //customTab1.ItemAdded += CustomTab_ItemAdded;
            customTab1.AddButtonClicked += CustomTab1_AddButtonClicked;
            customTab1.ItemSelected += CustomTabControl_ItemSelected;
            customTab1.ItemClosing += CustomTab1_ItemClosing;

            breadcrumbBar1.CrumbClicked += breadcrumbBar_CrumbClicked;
            txtSearchTiles.SearchDelayCompleted += txtSearchTiles_SearchDelayCompleted;
        }
        private dtoGetPermissionResult TileCheckPermission(string Code)
        {
            dtoGetPermissionResult result = (new srvGetPermission()).Execute(Code);

            if (result.isActive == false)
            {
                var msg = new modelShowMessage()
                {
                    Title = "محدودیت دسترسی",
                    Message = "لایسنس این گزینه خریداری نشده است"
                };

                var rtn = Program.ShowMessage(msg);
            }

            return result;
        }

        private void ShowForm(string FormCode)
        {
            TabItem? currentTab = customTab1.SelectedItem;

            if (currentTab == null)
            {
                MessageBox.Show("هیچ تبی انتخاب نشده است.");
                return;
            }
            string tabName = currentTab.TabName; // شناسه تب (مثلاً tab-xxxx)
            string title = currentTab.Title;     // عنوان تب 
            string Code = currentTab.Code;     // کد عنوانی که در سربرگ درج شد

            customTab1.flowLayoutPanels[tabName].Items.Clear();
            customTab1.flowLayoutPanels[tabName].Controls.Clear();
            customTab1.flowLayoutPanels[tabName].RightToLeft = RightToLeft.Yes;

            var result = srvGetTileByCode.Execute(FormCode);
            if (result != null && result.Any())
            {
                string ucFormName = null;
                foreach (var a in result)
                {
                    ucFormName = a.FormName;
                }

                if (ucFormName != null)
                {

                    UserControl uc = srvCreateUserControlByName.Execute(ucFormName);

                    #region ---[ تصمیم گیری در مورد محتوام قایل ننمایش فرم نتخاب شد ]---

                    // --- بخش هوشمندسازی برای ucMap ---
                    if (uc is ucMap mapControl)
                    {
                        // تشخیص نوع منبع بر اساس FormCode دیتابیس
                        // شما می‌توانید بر اساس کدهای واقعی خودتان این Switch را تنظیم کنید
                        MapSource source = MapSource.None; // پیش‌فرض

                        if (title.Contains("محله") /*|| FormCode == "101"*/) // مثال برای کد محله
                            source = MapSource.Mahale;
                        else if (title.Contains("مسجد") /*|| FormCode == "103"*/) // مثال برای کد مسجد
                            source = MapSource.Masjed;
                        else if (title.Contains("اماکن") /*|| FormCode == "103"*/) // مثال برای کد اماکن
                            source = MapSource.Amaken;

                        // فراخوانی متد مقداردهی که در کلاس ucMap تعریف کردیم
                        if (source != MapSource.None)
                            mapControl.Initialize(source);
                    }

                    # endregion ---[ تصمیم گیری در مورد محتوام قایل ننمایش فرم نتخاب شد ]---


                    customTab1.flowLayoutPanels[tabName].SetItems(null);
                    uc.Dock = DockStyle.Fill;
                    customTab1.flowLayoutPanels[tabName].AutoScroll = true;
                    customTab1.flowLayoutPanels[tabName].Controls.Add(uc);
                }
            }
        }

        private void SetTileToTabByTileList(IEnumerable<Tiles> _TileInfo)
        {
            /////////////////////////////[ دریافت تایلها پدر برای ایجاد سربرگ ]///////////////////////////
            //var dbParentTile = (new srvGetTileByCode()).Execute(Code).First();
            //// دریافت شیء تب انتخاب شده
            TabItem? currentTab = customTab1.SelectedItem;

            if (currentTab == null)
            {
                MessageBox.Show("هیچ تبی انتخاب نشده است.");
                return;
            }
            string tabName = currentTab.TabName; // شناسه تب (مثلاً tab-xxxx)
            string title = currentTab.Title;     // عنوان تب (مثلاً تب جدید)
            string Code = currentTab.Code;     // عنوان تب (مثلاً تب جدید)

            //currentTab.Title = dbParentTile.Title;
            //currentTab.Code = Code;
            //MessageBox.Show($"تب انتخاب شده: {title} با شناسه {tabName} , code {currentTab.Code}");

            /////////////////////////////[ دریافت تابلها پدر برای ایجاد مسیر ]///////////////////////////
            //SetBreadcrumbBar("");

            /////////////////////////////[ دریافت تایلها به وسیله کد پدر ]///////////////////////////
            //var dbTiles = (new srvGetTileBySubCode()).Execute(Code);

            customTab1.flowLayoutPanels[tabName].Items.Clear();
            customTab1.flowLayoutPanels[tabName].Controls.Clear();
            List<DashboardTileItem> Tiles = new List<DashboardTileItem> { };

            if (_TileInfo != null && _TileInfo.Any())
            {

                foreach (var a in _TileInfo)
                {
                    Tiles.Add(
                        new DashboardTileItem
                        {
                            Code = a.Code,
                            ParentCode = a.SubCode,
                            Title = a.Title,
                            Description = a.Describe,
                            Icon = Image.FromFile(System.IO.Path.Combine(appPath, "Pictures\\Items", a.Icon)),
                            ItemType = ((DashboardTileItemType)a.ItemType),
                            Visible = true
                        });
                }

            }

            if (txtSearchTiles.Text != "")
            {
                //نمایش تایلها
                customTab1.flowLayoutPanels[tabName].RightToLeft = RightToLeft.Yes;
                customTab1.flowLayoutPanels[tabName].SetItems(Tiles);
            }
            else
            {
                SetTileToTab(Code);
            }

        }

        private void SetTileToTab(string Code)
        {
            //if (TileCheckPermission(Code).isActive == false)
            //{
            //    return;
            //}

            ///////////////////////////[ دریافت تایلها پدر برای ایجاد سربرگ ]///////////////////////////
            var dbParentTile = srvGetTileByCode.Execute(Code).First();
            // دریافت شیء تب انتخاب شده
            TabItem? currentTab = customTab1.SelectedItem;

            if (currentTab == null)
            {
                MessageBox.Show("هیچ تبی انتخاب نشده است.");
                return;
            }
            string tabName = currentTab.TabName; // شناسه تب (مثلاً tab-xxxx)
            string title = currentTab.Title;     // عنوان تب (مثلاً تب جدید)

            currentTab.Title = dbParentTile.Title;
            currentTab.Code = Code;
            //MessageBox.Show($"تب انتخاب شده: {title} با شناسه {tabName}");

            ///////////////////////////[ دریافت تابلها پدر برای ایجاد مسیر ]///////////////////////////
            SetBreadcrumbBar(Code);

            ///////////////////////////[ دریافت تایلها به وسیله کد پدر ]///////////////////////////
            var dbTiles = (new srvGetTileBySubCode()).Execute(Code);

            customTab1.flowLayoutPanels[tabName].Items.Clear();
            customTab1.flowLayoutPanels[tabName].Controls.Clear();

            List<DashboardTileItem> Tiles = new List<DashboardTileItem> { };
            foreach (var a in dbTiles)
            {
                Tiles.Add(
                    new DashboardTileItem
                    {
                        Code = a.Code,
                        ParentCode = a.SubCode,
                        Title = a.Title,
                        Description = a.Describe,
                        Icon = Image.FromFile(System.IO.Path.Combine(appPath, "Pictures\\Items", a.Icon)),
                        ItemType = ((DashboardTileItemType)a.ItemType),
                        Visible = true
                    });
            }
            //نمایش تایلها
            customTab1.flowLayoutPanels[tabName].RightToLeft = RightToLeft.Yes;
            customTab1.flowLayoutPanels[tabName].SetItems(Tiles);
        }
        #endregion ====[ Public Methods ]===

        #region ====[ BreadcrumbBar Methods ]===
        //private class BreadcrumbBarItems()
        //{
        //    public string Code { get; set; }
        //    public string Title { get; set; }
        //}
        private void SetBreadcrumbBar(string Code)
        {

            List<BreadcrumbBarItems> items = new List<BreadcrumbBarItems>();

            if (!String.IsNullOrWhiteSpace(Code))
            {
                var dbTilePath = (new srvGetTilePath()).Execute(Code);
                string pathItem = "";
                foreach (var a in dbTilePath)
                {
                    items.Add(new BreadcrumbBarItems
                    {
                        Code = a.Code,
                        Title = a.Title,
                    });
                }
            }

            breadcrumbBar1.SetPath(items);
        }
        private void breadcrumbBar_CrumbClicked(object sender, BreadcrumbClickedEventArgs e)
        {
            string Code = e.Code;
            string title = e.Title;


            if (TileCheckPermission(Code).isActive == false)
            {
                return;
            }

            //MessageBox.Show($"Code = {code} , Title = {title}");
            SetTileToTab(Code);
        }

        #endregion ====[ BreadcrumbBar Methods ]===

        #region ====[ roundedTextBox Methods ]===
        private void txtSearchTiles_SearchDelayCompleted(object sender, EventArgs e)
        {
            // دریافت متن نهایی وارد شده توسط کاربر (بدون در نظر گرفتن متن Placeholder)
            //string query = roundedTextBox1.Text;
            //MessageBox.Show("ddd");
            // اجرای عملیات جستجو، فیلتر دیتابیس یا گرید
            //ExecuteSearch(query);

            var result = (new srvGetTileByTitle()).Execute(txtSearchTiles.Text);

            SetTileToTabByTileList(result);


        }
        #endregion ====[ roundedTextBox Methods ]===

        #region ====[ CustomeTab Methods ]===
        private void CustomTabControl_ItemSelected(object? sender, TabEventArgs e)
        {
            string tabName = e.Item.tabname;
            string title = e.Item.Title;

            var control = tlpMain.GetControlFromPosition(0, 3);
            if (control != null) tlpMain.Controls.Remove(control);

            //lblCurrentBox.Text = title;

            customTab1.flowLayoutPanels[tabName].Dock= DockStyle.Fill;
            customTab1.flowLayoutPanels[tabName].AutoScroll = true;
            tlpMain.Controls.Add(customTab1.flowLayoutPanels[tabName], 0, 3);

            SetBreadcrumbBar(e.Item.Code);

        }
        //AddButtonClicked : هنگام کلیک روی کمه اضافه کردن
        //ItemAdded : بعداز اضافه شدن سربرگ
        private void CustomTab1_ItemAdded(object? sender, TabEventArgs e)
        {
            TabItem addedItem = e.Item;
            MessageBox.Show($"سربرگ {addedItem.Title} با شناسه {addedItem.TabName} اضافه شد.");

        }

        private void CustomTab1_AddButtonClicked(object? sender, EventArgs e)
        {
            var dbParentTile = srvGetTileByCode.Execute("0").FirstOrDefault();

            if (dbParentTile == null)
            {
                MessageBox.Show("اطلاعات سربرگ پیدا نشد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customTabName = dbParentTile.Code + Guid.NewGuid().ToString().Replace("-", "");
            //TabItem? newTabItem = customTab.AddNewTab(title: dbParentTile.Title,tabName: dbParentTile.Code,selectTab: true);
            customTab1.AddNewTab(title: dbParentTile.Title, tabName: customTabName, selectTab: true);

            var control = tlpMain.GetControlFromPosition(0, 3);
            if (control != null) tlpMain.Controls.Remove(control);

            var panel = customTab1.flowLayoutPanels[customTabName];
            panel.ItemClicked -= AnimatedTilePanel_ItemClicked;
            panel.ItemClicked += AnimatedTilePanel_ItemClicked;

            tlpMain.Controls.Add(panel, 0, 3);

            SetTileToTab("0");

            panel.AutoScroll = true;
        }
        private void CustomTab1_ItemClosing(object? sender, TabEventArgs e)
        {
            if (customTab1.ItemCount <=1)
            {
                BeginInvoke(new Action(() =>
                {
                    customTab1.PerformAddButtonClick();
                }));
            }
        }

        #endregion ====[ CustomeTab Methods ]===

        #region ====[ AnimatedTilePanel Methods ]===
        private void AnimatedTilePanel_ItemClicked(object sender, DashboardTileItemClickedEventArgs e)
        {
            // e.Item حالا حاوی اطلاعات کاشی کلیک شده است (DashboardTileItem)
            DashboardTileItem clickedItem = e.Item;

            // حالا می‌توانید از اطلاعات clickedItem استفاده کنید:
            string Code = clickedItem.Code;
            string title = clickedItem.Title;
            string description = clickedItem.Description;
            Image icon = clickedItem.Icon;
            DashboardTileItemType itemType = clickedItem.ItemType;

            if (TileCheckPermission(Code).isActive == false)
            {
                return;
            }

            txtSearchTiles.Text = "";

            // مثال: نمایش ID و Title گزینه انتخاب شده
            //MessageBox.Show($"گزینه انتخاب شده:\nID: {id}\nTitle: {title}");

            //breadcrumbBar1.SetPath(title);

            //breadcrumbBar1.CrumbClicked -= (s, e) => { };

            // بر اساس نوع آیتم یا ID آن می‌توانید کارهای مختلفی انجام دهید:
            switch (itemType)
            {
                case DashboardTileItemType.Form:
                    // اگر نوعش فرم است، فرم مربوطه را باز کنید
                    //MessageBox.Show($"Opening form for ID: {Code}");
                    ShowForm(Code);
                    break;
                case DashboardTileItemType.GroupItems:
                    // اگر نوعش گروه است، کارهای مربوط به گروه را انجام دهید
                    //MessageBox.Show($"Grouping items for ID: {Code}");
                    SetTileToTab(Code);
                    break;
                default:
                    // برای انواع دیگر یا IDهای خاص
                    if (Code == "1") // مثال: اگر ID برابر 1 بود
                    {
                        // کاری انجام بده
                    }
                    break;
            }
        }

        #endregion ====[ AnimatedTilePanel Methods ]===



    }
}

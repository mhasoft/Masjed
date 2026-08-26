using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForm.Controls.MapBridge;
using WinForm.Models.Entities;
using WinForm.Services.Database.Insert.setAmakenAllRecord;
using WinForm.Services.Database.Insert.setMahaleAllRecord;
using WinForm.Services.Database.Insert.setMasjedAllRecord;
using WinForm.Services.Database.Select.getAmakenAllRecords;
using WinForm.Services.Database.Select.getMahaleAllRecords;
using WinForm.Services.Database.Select.getMahaleById;
using WinForm.Services.Database.Select.getMasjedAllRecords;

namespace WinForm.UserControls
{
    public enum MapSource
    {
        None,
        Mahale,
        Masjed,
        Amaken
    }


    public partial class ucMap : UserControl, IMapBridgeHost
    {
        #region Fields & Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Mahale> mapMahale { get; set; }
            = new Dictionary<string, Mahale>();


        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Masjed> mapMasjed { get; set; }
            = new Dictionary<string, Masjed>();


        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Amaken> mapAmaken { get; set; }
            = new Dictionary<string, Amaken>();


        private string? _selectedId;

        private string? _selectedMahaleId;

        private MapSource _currentSource =
            MapSource.None;

        private bool _isMapReady;

        private bool _isInitializing;

        private bool _eventsRegistered;

        private MapBridge? _mapBridge;


        /*
         * =============================================================
         * رابطه موقت مسجد با محله
         *
         * کلید:
         *     کلید فعلی مسجد در mapMasjed
         *
         * مقدار:
         *     کلید محله در mapMahale
         *
         * دلیل:
         *
         * برای محله جدید:
         *
         *     Mahale.Id = 0
         *
         * بنابراین نمی‌توانیم قبل از ذخیره محله، مقدار
         * Mahale_Id مسجد را با Id واقعی محله پر کنیم.
         *
         * در این Dictionary رابطه را موقتاً با Key نگه می‌داریم.
         *
         * بعد از ذخیره محله:
         *
         *     Mahale.Id
         *
         * واقعی می‌شود و سپس روی:
         *
         *     Masjed.Mahale_Id
         *
         * قرار می‌گیرد.
         * =============================================================
         */
        private readonly Dictionary<string, string>
            _masjedParentMahaleKey =
                new Dictionary<string, string>();


        #endregion


        #region Constructor / Initialize

        public ucMap()
        {
            InitializeComponent();

            RegisterInternalEvents();
        }


        public void Initialize(MapSource source)
        {
            _currentSource =
                source;

            _isInitializing =
                true;

            try
            {
                LoadInitialData();

                ApplyUIConstraints();
            }
            finally
            {
                _isInitializing =
                    false;
            }


            if (_isMapReady)
            {
                _ = RenderDataOnMapAsync();
            }
        }


        private void LoadInitialData()
        {
            try
            {
                mapMahale =
                    srvGetMahaleAllRecords.Execute()
                    ?? new Dictionary<string, Mahale>();


                mapMasjed =
                    srvGetMasjedAllRecords.Execute()
                    ?? new Dictionary<string, Masjed>();


                mapAmaken =
                    srvGetAmakenAllRecords.Execute()
                    ?? new Dictionary<string, Amaken>();


                RebuildMasjedParentRelations();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Data Load Error: " +
                    ex);


                MessageBox.Show(
                    "خطا در دریافت اطلاعات نقشه:\n" +
                    ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void ApplyUIConstraints()
        {
            switch (_currentSource)
            {
                case MapSource.Mahale:

                    tabControlFields.SelectedTab =
                        tabPageMahale;

                    btnAddNewArea.Visible =
                        true;

                    btnPickLocation.Visible =
                        false;

                    chbShowMahale.Checked =
                        true;

                    chbShowMasjed.Checked =
                        false;

                    chbShowMakan.Checked =
                        false;

                    break;


                case MapSource.Masjed:

                    tabControlFields.SelectedTab =
                        tabPageMasjed;

                    btnAddNewArea.Visible =
                        true;

                    btnPickLocation.Visible =
                        true;

                    chbShowMahale.Checked =
                        true;

                    chbShowMasjed.Checked =
                        true;

                    chbShowMakan.Checked =
                        false;

                    break;


                case MapSource.Amaken:

                    tabControlFields.SelectedTab =
                        tabPageAmaken;

                    btnAddNewArea.Visible =
                        false;

                    btnPickLocation.Visible =
                        true;

                    chbShowMahale.Checked =
                        false;

                    chbShowMasjed.Checked =
                        false;

                    chbShowMakan.Checked =
                        true;

                    break;


                default:

                    tabControlFields.SelectedTab =
                        tabPageMahale;

                    btnAddNewArea.Visible =
                        false;

                    btnPickLocation.Visible =
                        false;

                    break;
            }
        }

        #endregion


        #region Events

        private void RegisterInternalEvents()
        {
            if (_eventsRegistered)
                return;


            _eventsRegistered =
                true;


            btnSaveAll.Click +=
                BtnSaveAll_Click;


            btnAddNewArea.Click +=
                BtnAddNewArea_Click;


            btnPickLocation.Click +=
                BtnPickLocation_Click;


            chbShowMahale.CheckedChanged +=
                ChbShowMahale_CheckedChanged;


            chbShowMasjed.CheckedChanged +=
                ChbShowMasjed_CheckedChanged;


            chbShowMakan.CheckedChanged +=
                ChbShowMakan_CheckedChanged;


            chbShowNames.CheckedChanged +=
                ChbShowNames_CheckedChanged;


            tabControlFields.SelectedIndexChanged +=
                TabControlFields_SelectedIndexChanged;
        }


        protected override async void OnLoad(
            EventArgs e)
        {
            base.OnLoad(e);


            if (LicenseManager.UsageMode ==
                LicenseUsageMode.Designtime)
            {
                return;
            }


            await InitializeWebViewAsync();
        }


        private async Task InitializeWebViewAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);


                _mapBridge =
                    new MapBridge(
                        this,
                        this);


                webView.CoreWebView2
                    .AddHostObjectToScript(
                        "bridge",
                        _mapBridge);


                webView.NavigationCompleted +=
                    WebView_NavigationCompleted;


                string htmlPath =
                    Path.Combine(
                        Application.StartupPath,
                        "HTML",
                        "Map.html");


                if (!File.Exists(htmlPath))
                {
                    MessageBox.Show(
                        "فایل Map.html پیدا نشد:\n" +
                        htmlPath,
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }


                webView.Source =
                    new Uri(htmlPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "WebView Initialize Error: " +
                    ex);


                MessageBox.Show(
                    "خطا در راه‌اندازی نقشه:\n" +
                    ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private async void WebView_NavigationCompleted(
            object? sender,
            CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show(
                    "بارگذاری صفحه نقشه با خطا مواجه شد.",
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }


            _isMapReady =
                true;


            await RenderDataOnMapAsync();

            await UpdateNamesVisibilityAsync();
        }


        private async void BtnAddNewArea_Click(
            object? sender,
            EventArgs e)
        {
            if (!_isMapReady)
            {
                MessageBox.Show(
                    "نقشه هنوز به‌طور کامل بارگذاری نشده است.",
                    "لطفاً چند لحظه صبر کنید",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            // =========================================================
            // رسم محدوده محله
            // =========================================================

            if (tabControlFields.SelectedTab ==
                tabPageMahale)
            {
                int nextNumber =
                    mapMahale.Count + 1;


                string defaultName =
                    $"محله {nextNumber}";


                string nameJson =
                    JsonSerializer.Serialize(
                        defaultName);


                string typeJson =
                    JsonSerializer.Serialize(
                        "mahale");


                await ExecuteJsAsync(
                    $"startNewAreaDrawing(" +
                    $"{nameJson}, " +
                    $"{typeJson}, " +
                    "null);");


                return;
            }


            // =========================================================
            // رسم محدوده مسجد
            // =========================================================

            if (tabControlFields.SelectedTab ==
                tabPageMasjed)
            {
                if (string.IsNullOrWhiteSpace(
                    _selectedMahaleId))
                {
                    MessageBox.Show(
                        "ابتدا یک محدوده محله را روی نقشه انتخاب کنید.",
                        "انتخاب محله",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }


                if (!mapMahale.TryGetValue(
                    _selectedMahaleId,
                    out Mahale? selectedMahale))
                {
                    MessageBox.Show(
                        "محله انتخاب‌شده در اطلاعات برنامه پیدا نشد.",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }


                if (string.IsNullOrWhiteSpace(
                    selectedMahale.CoordinatesJson))
                {
                    MessageBox.Show(
                        "محله انتخاب‌شده فاقد محدوده جغرافیایی معتبر است.",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                int nextNumber =
                    mapMasjed.Count + 1;


                string defaultName =
                    $"مسجد {nextNumber}";


                string nameJson =
                    JsonSerializer.Serialize(
                        defaultName);


                string typeJson =
                    JsonSerializer.Serialize(
                        "masjed");


                string parentMahaleIdJson =
                    JsonSerializer.Serialize(
                        _selectedMahaleId);


                await ExecuteJsAsync(
                    $"startNewAreaDrawing(" +
                    $"{nameJson}, " +
                    $"{typeJson}, " +
                    $"{parentMahaleIdJson});");


                return;
            }


            MessageBox.Show(
                "رسم محدوده فقط برای محله و مسجد قابل استفاده است.",
                "اطلاع",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }


        private async void BtnPickLocation_Click(
            object? sender,
            EventArgs e)
        {
            if (_currentSource != MapSource.Masjed &&
                _currentSource != MapSource.Amaken)
            {
                return;
            }


            if (string.IsNullOrWhiteSpace(
                _selectedId))
            {
                MessageBox.Show(
                    "ابتدا یک مسجد یا مکان را از روی نقشه انتخاب کنید.",
                    "انتخاب آیتم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            if (!_isMapReady)
                return;


            await ExecuteJsAsync(
                "startLocationPicking();");
        }


        private void BtnSaveAll_Click(
            object? sender,
            EventArgs e)
        {
            SaveChanges();
        }


        private void ChbShowMahale_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            if (_isInitializing)
                return;


            _ = ToggleLayerAsync(
                "mahale",
                chbShowMahale.Checked);
        }


        private void ChbShowMasjed_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            if (_isInitializing)
                return;


            _ = ToggleLayerAsync(
                "masjed",
                chbShowMasjed.Checked);
        }


        private void ChbShowMakan_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            if (_isInitializing)
                return;


            _ = ToggleLayerAsync(
                "amaken",
                chbShowMakan.Checked);
        }


        private void ChbShowNames_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            if (_isInitializing)
                return;


            _ = UpdateNamesVisibilityAsync();
        }


        private void TabControlFields_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            string? tag =
                tabControlFields.SelectedTab?
                    .Tag?
                    .ToString()?
                    .Trim()
                    .ToLowerInvariant();


            switch (tag)
            {
                case "mahale":

                    _currentSource =
                        MapSource.Mahale;

                    break;


                case "masjed":

                    _currentSource =
                        MapSource.Masjed;

                    break;


                case "amaken":

                    _currentSource =
                        MapSource.Amaken;

                    break;
            }
        }

        #endregion


        #region Render Map

        private async Task RenderDataOnMapAsync()
        {
            if (!_isMapReady)
                return;


            await ExecuteJsAsync(
                "clearLayers();");


            // =========================================================
            // Mahale
            // =========================================================

            foreach (
                KeyValuePair<string, Mahale> item
                in mapMahale)
            {
                if (item.Value == null)
                    continue;


                await AddToMapAsync(
                    item.Key,
                    "mahale",
                    item.Value.CoordinatesJson,
                    item.Value.Name,
                    null);
            }


            // =========================================================
            // Masjed
            // =========================================================

            foreach (
                KeyValuePair<string, Masjed> item
                in mapMasjed)
            {
                if (item.Value == null)
                    continue;


                string coordinates;


                if (!string.IsNullOrWhiteSpace(
                    item.Value.CoordinatesJson))
                {
                    coordinates =
                        item.Value.CoordinatesJson;
                }
                else
                {
                    coordinates =
                        GetPointJson(
                            item.Value.Latitude,
                            item.Value.Longitude);
                }


                string? parentMahaleKey =
                    GetMasjedParentMahaleKey(
                        item.Key,
                        item.Value);


                await AddToMapAsync(
                    item.Key,
                    "masjed",
                    coordinates,
                    item.Value.Name,
                    parentMahaleKey);
            }


            // =========================================================
            // Amaken
            // =========================================================

            foreach (
                KeyValuePair<string, Amaken> item
                in mapAmaken)
            {
                if (item.Value == null)
                    continue;


                await AddToMapAsync(
                    item.Key,
                    "amaken",
                    GetPointJson(
                        item.Value.Latitude,
                        item.Value.Longitude),
                    item.Value.Name,
                    null);
            }


            await ToggleLayerAsync(
                "mahale",
                chbShowMahale.Checked);


            await ToggleLayerAsync(
                "masjed",
                chbShowMasjed.Checked);


            await ToggleLayerAsync(
                "amaken",
                chbShowMakan.Checked);


            await UpdateNamesVisibilityAsync();
        }


        private async Task AddToMapAsync(
            string id,
            string type,
            string? coordinatesJson,
            string? name,
            string? parentMahaleId)
        {
            if (string.IsNullOrWhiteSpace(
                coordinatesJson))
            {
                return;
            }


            try
            {
                string idJson =
                    JsonSerializer.Serialize(id);


                string typeJson =
                    JsonSerializer.Serialize(type);


                string coordsJson =
                    JsonSerializer.Serialize(
                        coordinatesJson);


                string nameJson =
                    JsonSerializer.Serialize(
                        name ?? string.Empty);


                string parentMahaleJson =
                    JsonSerializer.Serialize(
                        parentMahaleId);


                await ExecuteJsAsync(
                    $"addItem(" +
                    $"{idJson}, " +
                    $"{typeJson}, " +
                    $"{coordsJson}, " +
                    $"{nameJson}, " +
                    $"{parentMahaleJson});");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"خطا در افزودن آیتم {id} روی نقشه: " +
                    ex.Message);
            }
        }


        private static string GetPointJson(
            string? latitude,
            string? longitude)
        {
            if (string.IsNullOrWhiteSpace(latitude) ||
                string.IsNullOrWhiteSpace(longitude))
            {
                return string.Empty;
            }


            if (!double.TryParse(
                latitude,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double lat))
            {
                return string.Empty;
            }


            if (!double.TryParse(
                longitude,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double lng))
            {
                return string.Empty;
            }


            return JsonSerializer.Serialize(
                new[]
                {
                    new
                    {
                        lat,
                        lng
                    }
                });
        }


        private Task ToggleLayerAsync(
            string type,
            bool show)
        {
            string typeJson =
                JsonSerializer.Serialize(type);


            string boolValue =
                show
                    .ToString()
                    .ToLowerInvariant();


            return ExecuteJsAsync(
                $"toggleLayer(" +
                $"{typeJson}, " +
                $"{boolValue});");
        }


        private Task UpdateNamesVisibilityAsync()
        {
            string boolValue =
                chbShowNames.Checked
                    .ToString()
                    .ToLowerInvariant();


            return ExecuteJsAsync(
                $"setItemNamesVisible(" +
                $"{boolValue});");
        }


        private async Task ExecuteJsAsync(
            string script)
        {
            if (!_isMapReady ||
                webView == null ||
                webView.CoreWebView2 == null)
            {
                return;
            }


            try
            {
                await webView.CoreWebView2
                    .ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "JavaScript Error: " +
                    ex);
            }
        }

        #endregion


        #region MapBridge Callbacks

        /*
         * =============================================================
         * نمایش / مخفی کردن Panelهای مربوط به نوع آیتم
         *
         * نکته:
         *
         * TabPageها مخفی نمی‌شوند.
         *
         * فقط Panelهای:
         *
         *     paneTabPageMahale
         *     paneTabPageMasjed
         *     paneTabPageAmaken
         *
         * کنترل می‌شوند.
         *
         * mahale:
         *     فقط Panel محله
         *
         * masjed:
         *     Panel محله + Panel مسجد
         *
         * amaken:
         *     Panel محله + Panel مسجد + Panel مکان
         * =============================================================
         */
        public void OnItemTypeSelected(
            string type)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action(
                        () =>
                            OnItemTypeSelected(type)));

                return;
            }


            panelTabPageMahale.Visible =
                false;

            panelTabPageMasjed.Visible =
                false;

            panelTabPageAmaken.Visible =
                false;


            switch (
                type?
                    .Trim()
                    .ToLowerInvariant())
            {
                case "mahale":

                    panelTabPageMahale.Visible =
                        true;

                    break;


                case "masjed":

                    panelTabPageMahale.Visible =
                        true;

                    panelTabPageMasjed.Visible =
                        true;

                    break;


                case "amaken":

                    panelTabPageMahale.Visible =
                        true;

                    panelTabPageMasjed.Visible =
                        true;

                    panelTabPageAmaken.Visible =
                        true;

                    break;


                case "":
                default:

                    break;
            }
        }


        public void UpdateItemInDictionary(
            string id,
            string coords,
            string name,
            string type)
        {
            _selectedId =
                id;


            switch (
                type?
                    .Trim()
                    .ToLowerInvariant())
            {
                // =====================================================
                // Mahale
                // =====================================================

                case "mahale":

                    if (!mapMahale.TryGetValue(
                        id,
                        out Mahale? mahale))
                    {
                        mahale =
                            mapMahale.Values
                                .FirstOrDefault(
                                    x =>
                                        x != null &&
                                        x.Id > 0 &&
                                        x.Id.ToString() ==
                                        id);


                        if (mahale != null)
                        {
                            string oldKey =
                                mapMahale
                                    .First(
                                        x =>
                                            x.Value ==
                                            mahale)
                                    .Key;


                            mapMahale.Remove(oldKey);

                            mapMahale[id] =
                                mahale;
                        }
                    }


                    if (mahale == null)
                    {
                        mahale =
                            new Mahale();


                        mapMahale[id] =
                            mahale;
                    }


                    mahale.Name =
                        name?.Trim() ??
                        string.Empty;


                    mahale.CoordinatesJson =
                        coords ??
                        string.Empty;


                    _selectedMahaleId =
                        id;


                    tabControlFields.SelectedTab =
                        tabPageMahale;


                    txtMahaleId.Text =
                        mahale.Id > 0
                            ? mahale.Id.ToString()
                            : id;


                    txtMahaleName.Text =
                        mahale.Name;

                    break;


                // =====================================================
                // Masjed
                // =====================================================

                case "masjed":

                    bool isNewMasjed =
                        !mapMasjed.ContainsKey(id);


                    if (!mapMasjed.TryGetValue(
                        id,
                        out Masjed? masjed))
                    {
                        masjed =
                            new Masjed();


                        if (
                            !string.IsNullOrWhiteSpace(
                                _selectedMahaleId) &&
                            mapMahale.ContainsKey(
                                _selectedMahaleId))
                        {
                            _masjedParentMahaleKey[id] =
                                _selectedMahaleId;
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                _selectedMahaleId) &&
                            mapMahale.TryGetValue(
                                _selectedMahaleId,
                                out Mahale? selectedMahale) &&
                            selectedMahale.Id > 0)
                        {
                            masjed.Mahale_Id =
                                selectedMahale.Id;
                        }


                        mapMasjed[id] =
                            masjed;
                    }


                    masjed.Name =
                        name?.Trim() ??
                        string.Empty;


                    masjed.CoordinatesJson =
                        coords ??
                        string.Empty;


                    if (isNewMasjed &&
                        !_masjedParentMahaleKey.ContainsKey(id) &&
                        !string.IsNullOrWhiteSpace(
                            _selectedMahaleId) &&
                        mapMahale.ContainsKey(
                            _selectedMahaleId))
                    {
                        _masjedParentMahaleKey[id] =
                            _selectedMahaleId;
                    }


                    if (
                        masjed.Mahale_Id > 0 &&
                        !_masjedParentMahaleKey.ContainsKey(id))
                    {
                        string? parentKey =
                            FindMahaleKeyById(
                                masjed.Mahale_Id);


                        if (!string.IsNullOrWhiteSpace(
                            parentKey))
                        {
                            _masjedParentMahaleKey[id] =
                                parentKey;
                        }
                    }


                    txtMasjedId.Text =
                        masjed.Id > 0
                            ? masjed.Id.ToString()
                            : id;


                    txtMasjedName.Text =
                        masjed.Name;

                    break;


                // =====================================================
                // Amaken
                // =====================================================

                case "amaken":

                    if (mapAmaken.TryGetValue(
                        id,
                        out Amaken? amaken))
                    {
                        amaken.Name =
                            name?.Trim() ??
                            string.Empty;
                    }

                    break;
            }
        }


        public void OnMapItemClicked(
            string id,
            string type)
        {
            _selectedId =
                id;


            switch (
                type?
                    .Trim()
                    .ToLowerInvariant())
            {
                // =====================================================
                // Mahale
                // =====================================================

                case "mahale":

                    if (
                        mapMahale.TryGetValue(
                            id,
                            out Mahale? mahale))
                    {
                        _selectedMahaleId =
                            id;


                        txtMahaleId.Text =
                            mahale.Id > 0
                                ? mahale.Id.ToString()
                                : id;


                        txtMahaleName.Text =
                            mahale.Name;
                    }

                    break;


                // =====================================================
                // Masjed
                // =====================================================

                case "masjed":

                    if (
                        mapMasjed.TryGetValue(
                            id,
                            out Masjed? masjed))
                    {
                        txtMasjedId.Text =
                            masjed.Id > 0
                                ? masjed.Id.ToString()
                                : id;


                        txtMasjedName.Text =
                            masjed.Name;


                        if (masjed.Mahale_Id > 0)
                        {
                            Mahale? parentMahale =
                                srvGetMahaleById.Execute(
                                    masjed.Mahale_Id);


                            if (parentMahale != null)
                            {
                                _selectedMahaleId =
                                    parentMahale.Id.ToString();


                                txtMahaleId.Text =
                                    parentMahale.Id.ToString();


                                txtMahaleName.Text =
                                    parentMahale.Name;
                            }
                            else
                            {
                                _selectedMahaleId =
                                    string.Empty;


                                txtMahaleId.Text =
                                    "";

                                txtMahaleName.Text =
                                    "";
                            }
                        }
                        else
                        {
                            _selectedMahaleId =
                                string.Empty;


                            txtMahaleId.Text =
                                "";

                            txtMahaleName.Text =
                                "";
                        }
                    }

                    break;


                // =====================================================
                // Amaken
                // =====================================================

                case "amaken":

                    if (
                        mapAmaken.TryGetValue(
                            id,
                            out Amaken? amaken))
                    {
                        txtAmakenId.Text =
                            amaken.Id > 0
                                ? amaken.Id.ToString()
                                : id;


                        txtAmakenName.Text =
                            amaken.Name;
                    }

                    break;
            }
        }


        public void OnLocationPicked(
            double lat,
            double lng)
        {
            if (string.IsNullOrWhiteSpace(
                _selectedId))
            {
                return;
            }


            string latitude =
                lat.ToString(
                    "0.000000",
                    CultureInfo.InvariantCulture);


            string longitude =
                lng.ToString(
                    "0.000000",
                    CultureInfo.InvariantCulture);


            if (
                _currentSource ==
                MapSource.Masjed &&
                mapMasjed.TryGetValue(
                    _selectedId,
                    out Masjed? masjed))
            {
                masjed.Latitude =
                    latitude;


                masjed.Longitude =
                    longitude;


                lblMasjedLocation.Text =
                    $"مختصات: {latitude}, {longitude}";
            }
            else if (
                _currentSource ==
                MapSource.Amaken &&
                mapAmaken.TryGetValue(
                    _selectedId,
                    out Amaken? amaken))
            {
                amaken.Latitude =
                    latitude;


                amaken.Longitude =
                    longitude;


                lblAmakenLocation.Text =
                    $"مختصات: {latitude}, {longitude}";
            }


            _ = RenderDataOnMapAsync();
        }


        public void RequestDeleteArea(
            string id,
            string type)
        {
            DialogResult result =
                MessageBox.Show(
                    "آیا از حذف این محدوده مطمئن هستید؟",
                    "تأیید حذف",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);


            if (result !=
                DialogResult.Yes)
            {
                return;
            }


            DeleteItemFromDictionaryOnly(
                id,
                type);


            _ = RenderDataOnMapAsync();
        }


        public void DeleteItemFromDictionaryOnly(
            string id,
            string type)
        {
            switch (
                type?
                    .Trim()
                    .ToLowerInvariant())
            {
                case "mahale":

                    mapMahale.Remove(id);


                    List<string> masjedKeysToRemove =
                        _masjedParentMahaleKey
                            .Where(
                                x =>
                                    x.Value == id)
                            .Select(
                                x =>
                                    x.Key)
                            .ToList();


                    foreach (
                        string masjedKey
                        in masjedKeysToRemove)
                    {
                        _masjedParentMahaleKey.Remove(
                            masjedKey);


                        if (mapMasjed.TryGetValue(
                            masjedKey,
                            out Masjed? masjed))
                        {
                            masjed.Mahale_Id =
                                0;
                        }
                    }


                    if (_selectedMahaleId == id)
                    {
                        _selectedMahaleId =
                            null;
                    }

                    break;


                case "masjed":

                    mapMasjed.Remove(id);

                    _masjedParentMahaleKey.Remove(id);

                    break;


                case "amaken":

                    mapAmaken.Remove(id);

                    break;
            }


            if (_selectedId == id)
            {
                _selectedId =
                    null;
            }
        }

        #endregion


        #region Save

        private void SaveChanges()
        {
            try
            {
                SyncCurrentFormValuesToDictionary();


                Dictionary<string, Mahale>
                    mahaleBeforeSave =
                        new Dictionary<string, Mahale>(
                            mapMahale);


                string? selectedMahaleOldKey =
                    _selectedMahaleId;


                mapMahale =
                    srvSetMahaleAllRecord.Execute(
                        mapMahale)
                    ?? mapMahale;


                if (!string.IsNullOrWhiteSpace(
                    selectedMahaleOldKey) &&
                    mahaleBeforeSave.TryGetValue(
                        selectedMahaleOldKey,
                        out Mahale? selectedMahaleObject))
                {
                    string? newSelectedMahaleKey =
                        FindMahaleKeyByReference(
                            selectedMahaleObject);


                    if (!string.IsNullOrWhiteSpace(
                        newSelectedMahaleKey))
                    {
                        _selectedMahaleId =
                            newSelectedMahaleKey;
                    }
                }


                ResolveMasjedMahaleIds(
                    mahaleBeforeSave);


                mapMasjed =
                    srvSetMasjedAllRecord.Execute(
                        mapMasjed)
                    ?? mapMasjed;


                RebuildMasjedParentRelations();


                mapAmaken =
                    srvSetAmakenAllRecord.Execute(
                        mapAmaken)
                    ?? mapAmaken;


                _ = RenderDataOnMapAsync();


                MessageBox.Show(
                    "تغییرات با موفقیت ذخیره شد.",
                    "ذخیره تغییرات",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "SaveChanges Error:");

                Debug.WriteLine(
                    ex.ToString());


                MessageBox.Show(
                    "خطا هنگام ذخیره اطلاعات:\n\n" +
                    ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void ResolveMasjedMahaleIds(
            Dictionary<string, Mahale> mahaleBeforeSave)
        {
            foreach (
                KeyValuePair<string, Masjed> item
                in mapMasjed)
            {
                string masjedKey =
                    item.Key;

                Masjed masjed =
                    item.Value;


                if (masjed == null)
                {
                    continue;
                }


                if (_masjedParentMahaleKey.TryGetValue(
                    masjedKey,
                    out string? mahaleKey))
                {
                    Mahale? mahale = null;


                    if (mahaleBeforeSave.TryGetValue(
                        mahaleKey,
                        out Mahale? oldMahale))
                    {
                        mahale =
                            oldMahale;
                    }


                    if (mahale == null &&
                        mapMahale.TryGetValue(
                            mahaleKey,
                            out Mahale? currentMahale))
                    {
                        mahale =
                            currentMahale;
                    }


                    if (mahale != null &&
                        mahale.Id > 0)
                    {
                        masjed.Mahale_Id =
                            mahale.Id;

                        continue;
                    }


                    if (mahale != null)
                    {
                        string? newMahaleKey =
                            FindMahaleKeyByReference(
                                mahale);


                        if (!string.IsNullOrWhiteSpace(
                            newMahaleKey) &&
                            mapMahale.TryGetValue(
                                newMahaleKey,
                                out Mahale? resolvedMahale) &&
                            resolvedMahale.Id > 0)
                        {
                            masjed.Mahale_Id =
                                resolvedMahale.Id;


                            _masjedParentMahaleKey[masjedKey] =
                                newMahaleKey;


                            continue;
                        }
                    }
                }


                if (masjed.Mahale_Id > 0)
                {
                    string? parentKey =
                        FindMahaleKeyById(
                            masjed.Mahale_Id);


                    if (!string.IsNullOrWhiteSpace(
                        parentKey))
                    {
                        _masjedParentMahaleKey[masjedKey] =
                            parentKey;
                    }


                    continue;
                }


                throw new Exception(
                    $"برای مسجد «{masjed.Name}» " +
                    "محله والد مشخص نشده است.");
            }
        }


        private void SyncCurrentFormValuesToDictionary()
        {
            if (string.IsNullOrWhiteSpace(
                _selectedId))
            {
                return;
            }


            switch (_currentSource)
            {
                case MapSource.Mahale:

                    if (mapMahale.TryGetValue(
                        _selectedId,
                        out Mahale? mahale))
                    {
                        mahale.Name =
                            txtMahaleName.Text.Trim();
                    }

                    break;


                case MapSource.Masjed:

                    if (mapMasjed.TryGetValue(
                        _selectedId,
                        out Masjed? masjed))
                    {
                        masjed.Name =
                            txtMasjedName.Text.Trim();
                    }

                    break;


                case MapSource.Amaken:

                    if (mapAmaken.TryGetValue(
                        _selectedId,
                        out Amaken? amaken))
                    {
                        amaken.Name =
                            txtAmakenName.Text.Trim();
                    }

                    break;
            }
        }

        #endregion


        #region Mahale / Masjed Relationship Helpers

        private string? FindMahaleKeyById(
            int mahaleId)
        {
            if (mahaleId <= 0)
                return null;


            KeyValuePair<string, Mahale>? result =
                mapMahale
                    .Where(
                        x =>
                            x.Value != null &&
                            x.Value.Id == mahaleId)
                    .Select(
                        x =>
                            (KeyValuePair<string, Mahale>?)x)
                    .FirstOrDefault();


            return result?.Key;
        }


        private string? FindMahaleKeyByReference(
            Mahale mahale)
        {
            if (mahale == null)
                return null;


            foreach (
                KeyValuePair<string, Mahale> item
                in mapMahale)
            {
                if (ReferenceEquals(
                    item.Value,
                    mahale))
                {
                    return item.Key;
                }
            }


            if (mahale.Id > 0)
            {
                return FindMahaleKeyById(
                    mahale.Id);
            }


            return null;
        }


        private string? GetMasjedParentMahaleKey(
            string masjedKey,
            Masjed masjed)
        {
            if (masjed == null)
                return null;


            if (_masjedParentMahaleKey.TryGetValue(
                masjedKey,
                out string? parentKey))
            {
                if (mapMahale.ContainsKey(parentKey))
                {
                    return parentKey;
                }
            }


            if (masjed.Mahale_Id > 0)
            {
                return FindMahaleKeyById(
                    masjed.Mahale_Id);
            }


            return null;
        }


        private void RebuildMasjedParentRelations()
        {
            _masjedParentMahaleKey.Clear();


            foreach (
                KeyValuePair<string, Masjed> item
                in mapMasjed)
            {
                Masjed? masjed =
                    item.Value;


                if (masjed == null)
                    continue;


                if (masjed.Mahale_Id <= 0)
                    continue;


                string? mahaleKey =
                    FindMahaleKeyById(
                        masjed.Mahale_Id);


                if (!string.IsNullOrWhiteSpace(
                    mahaleKey))
                {
                    _masjedParentMahaleKey[item.Key] =
                        mahaleKey;
                }
            }
        }

        #endregion


        /*
         * =============================================================
         * این متد قبلاً:
         *
         *     tabPageMahale.Hide();
         *
         * انجام می‌داد که با منطق جدید صحیح نیست.
         *
         * TabPageها نباید مخفی شوند.
         *
         * بنابراین فعلاً فقط برای جلوگیری از خطای احتمالی Designer
         * نگه داشته شده و هیچ TabPageای را Hide نمی‌کند.
         * =============================================================
         */
        private void button1_Click(
            object sender,
            EventArgs e)
        {
            OnItemTypeSelected(
                "mahale");
        }
    }
}
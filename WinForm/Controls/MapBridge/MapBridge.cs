using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinForm.Controls.MapBridge
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class MapBridge
    {
        private readonly Control _uiControl;
        private readonly IMapBridgeHost _host;

        public MapBridge(
            Control uiControl,
            IMapBridgeHost host)
        {
            _uiControl =
                uiControl
                ?? throw new ArgumentNullException(
                    nameof(uiControl));

            _host =
                host
                ?? throw new ArgumentNullException(
                    nameof(host));
        }


        private void ExecuteOnUiThread(
            Action action)
        {
            if (_uiControl.IsDisposed ||
                _uiControl.Disposing ||
                !_uiControl.IsHandleCreated)
            {
                return;
            }


            if (_uiControl.InvokeRequired)
            {
                _uiControl.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }


        public void OnAreaUpdated(
            string id,
            string coords,
            string name,
            string type)
        {
            ExecuteOnUiThread(
                () =>
                    _host.UpdateItemInDictionary(
                        id,
                        coords,
                        name,
                        type));
        }


        public void RequestDeleteArea(
            string id,
            string type)
        {
            ExecuteOnUiThread(
                () =>
                    _host.RequestDeleteArea(
                        id,
                        type));
        }


        public void OnAreaDeletedFromJS(
            string id,
            string type)
        {
            ExecuteOnUiThread(
                () =>
                    _host.DeleteItemFromDictionaryOnly(
                        id,
                        type));
        }


        public void OnMarkerOrAreaClicked(
            string id,
            string type)
        {
            ExecuteOnUiThread(
                () =>
                    _host.OnMapItemClicked(
                        id,
                        type));
        }


        public void OnLocationPicked(
            double lat,
            double lng)
        {
            ExecuteOnUiThread(
                () =>
                    _host.OnLocationPicked(
                        lat,
                        lng));
        }


        public void OnItemTypeSelected(
            string type)
        {
            ExecuteOnUiThread(
                () =>
                    _host.OnItemTypeSelected(
                        type));
        }
    }
}
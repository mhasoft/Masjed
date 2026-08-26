namespace WinForm.Controls.MapBridge
{
    public interface IMapBridgeHost
    {
        void UpdateItemInDictionary(
            string id,
            string coords,
            string name,
            string type);

        void RequestDeleteArea(
            string id,
            string type);

        void DeleteItemFromDictionaryOnly(
            string id,
            string type);

        void OnMapItemClicked(
            string id,
            string type);

        void OnLocationPicked(
            double lat,
            double lng);

        void OnItemTypeSelected(
            string type);
    }
}
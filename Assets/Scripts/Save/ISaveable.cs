using CI.QuickSave;

namespace AF
{
    public interface ISaveable
    {
        void OnLoadData(QuickSaveReader quickSaveReader);
        void OnSaveData(QuickSaveWriter quickSaveWriter);
    }
}
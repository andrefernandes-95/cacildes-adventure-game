using AF.ModTools;
using CI.QuickSave;

namespace AF
{
    public interface IModSaveable
    {
        SerializedModData<T> OnSaveData<T>();
    }
}

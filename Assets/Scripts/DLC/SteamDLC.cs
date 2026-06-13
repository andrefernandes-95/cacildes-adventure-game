using UnityEngine;
using Steamworks;

namespace AF
{
    [CreateAssetMenu(menuName = "AF/DLC/SteamDLC")]
    public class SteamDLC : ScriptableObject
    {
        [Header("Steam DLC")]
        public AppId_t appId;

        public bool IsOwned()
        {
            if (!SteamManager.Initialized) return false;
            return SteamApps.BIsDlcInstalled(appId);
        }
    }
}

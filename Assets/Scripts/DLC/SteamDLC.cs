using UnityEngine;
using Steamworks;

namespace AF
{
    [CreateAssetMenu(menuName = "AF/DLC/SteamDLC")]
    public class SteamDLC : ScriptableObject
    {
        [Header("Steam DLC")]
        public AppId_t appId;

        [Header("Is Enabled?")]
        public bool enabled = true;

        public bool IsOwned()
        {
            if (!enabled || !SteamManager.Initialized)
            {
                return false;
            }

            return SteamApps.BIsDlcInstalled(appId);
        }
    }
}

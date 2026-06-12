using UnityEngine;
using Steamworks;

namespace AF
{
    [CreateAssetMenu(menuName = "AF/DLC/SteamDLC")]
    public class SteamDLC : ScriptableObject
    {
        [Header("Steam DLC")]
        public AppId_t appId;

<<<<<<< HEAD
        public bool IsOwned()
        {
            if (!SteamManager.Initialized) return false;
=======
        [Header("Is Enabled?")]
        public bool enabled = true;

        public bool IsOwned()
        {
            if (!enabled || !SteamManager.Initialized)
            {
                return false;
            }

>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
            return SteamApps.BIsDlcInstalled(appId);
        }
    }
}

using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ManagePlayerControl : EventBase
    {
        [SerializeField] bool canControlPlayer = false;

        PlayerManager _playerManager;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (canControlPlayer)
            {
                GetPlayerManager().playerComponentManager.EnablePlayerControl();
                GetPlayerManager().thirdPersonController.canRotateCharacter = true;
            }
            else
            {
                GetPlayerManager().playerComponentManager.DisablePlayerControl();
                GetPlayerManager().thirdPersonController.canRotateCharacter = false;
            }
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

    }
}

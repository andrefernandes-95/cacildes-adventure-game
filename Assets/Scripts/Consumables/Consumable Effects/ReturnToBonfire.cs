using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Return To Bonfire")]
    public class ReturnToBonfire : ConsumableEffect
    {
        public override void OnStart(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.HideEquipment();

            characterBaseManager.PlayCrossFadeBusyAnimationWithRootMotion("Activate", .1f);
        }

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            if (characterBaseManager is not PlayerManager)
            {
                // Only player should be allowed to return to the last bonfire
                return;
            }

            TeleportManager teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);

            if (teleportManager != null)
            {
                teleportManager.TeleportToLastRestedBonfire();
            }
        }

        public override void OnEnd(CharacterBaseManager characterBaseManager)
        {
        }
    }
}

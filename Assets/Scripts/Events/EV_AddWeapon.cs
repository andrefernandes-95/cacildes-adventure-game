namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class EV_AddWeapon : EventBase
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] Weapon weapon;
        [SerializeField] bool autoEquip = false;
        [SerializeField] bool isRightHand = true;

        public override IEnumerator Dispatch()
        {
            // Add items
            Weapon addedWeapon = playerManager.playerInventory.AddWeapon(weapon);

            if (autoEquip)
            {
                playerManager.characterBaseEquipment.EquipWeapon(addedWeapon, 0, isRightHand);
            }

            yield return null;
        }
    }

}

using System;
using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_AddAndAutoEquipBowAndArrow : EventBase
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] Weapon bow;
        [SerializeField] Arrow arrow;
        [SerializeField] int arrowAmount = 15;

        public override IEnumerator Dispatch()
        {
            // Add items
            Weapon addedBow = playerManager.playerInventory.AddWeapon(bow);
            playerManager.characterBaseEquipment.EquipWeapon(Instantiate(addedBow), 0, false);

            // Added arrows
            for (int i = 0; i < arrowAmount; i++)
            {
                playerManager.playerInventory.AddArrow(arrow);
            }

            playerManager.equipmentDatabase.EquipArrow(arrow, 0);

            yield return null;
        }
    }

}

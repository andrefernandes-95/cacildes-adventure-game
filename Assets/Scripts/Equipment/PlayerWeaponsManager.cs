using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Localization;

namespace AF.Equipment
{
    public class PlayerWeaponsManager : CharacterBaseWeaponsManager
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] NotificationManager notificationManager;

        [Header("Databases")]
        [SerializeField] EquipmentDatabase equipmentDatabase;

        [Header("Localization")]

        // "Can not apply buff to this weapon"
        public LocalizedString CanNotApplyBuffToThisWeapon;
        // "Weapon is already buffed"
        public LocalizedString WeaponIsAlreadyBuffed;
        // "Not enough mana to use weapon special"
        public LocalizedString NotEnoughManaToUseWeaponSpecial;

        private void Awake()
        {
            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateEquipment);
        }

        protected override void UpdateCurrentWeapon()
        {
            base.UpdateCurrentWeapon();

            RefreshPlayer();
        }

        protected override void UpdateCurrentLeftWeapon()
        {
            base.UpdateCurrentLeftWeapon();

            RefreshPlayer();
        }

        void RefreshPlayer()
        {
            playerManager.UpdateAnimatorOverrideControllerClips();
            playerManager.statsBonusController.RecalculateEquipmentBonus();
        }

        public override void ShowRightWeapon()
        {
            if (playerManager.playerShootingManager.isAiming)
            {
                return;
            }

            base.ShowRightWeapon();
        }

        protected override bool CanApplyBuff()
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null)
            {
                notificationManager.ShowNotification(
                    CanNotApplyBuffToThisWeapon.GetLocalizedString(), notificationManager.systemError);
                return false;
            }
            else if (currentWeaponInstance.characterWeaponBuffs.HasOnGoingBuff())
            {
                notificationManager.ShowNotification(
                    WeaponIsAlreadyBuffed.GetLocalizedString(), notificationManager.systemError);
                return false;
            }

            return base.CanApplyBuff();
        }

        public override int GetCurrentBlockStaminaCost()
        {
            if (playerManager.playerWeaponsManager.currentShieldInstance == null)
            {
                return playerManager.characterAbstractBlockController.unarmedStaminaCostPerBlock;
            }

            return base.GetCurrentBlockStaminaCost();
        }

        protected override float GetCharacterUnarmedDefenseAbsorption()
        {
            return playerManager.characterAbstractBlockController.unarmedDefenseAbsorption;
        }

        public override Weapon GetCurrentRightWeapon()
        {
            return equipmentDatabase.GetCurrentWeapon();
        }

        public override Weapon GetCurrentLeftWeapon()
        {
            return equipmentDatabase.GetCurrentLeftWeapon();
        }

        public override bool IsTwoHanding()
        {
            return equipmentDatabase.isTwoHanding;
        }

        public override bool HasRangeWeapon()
        {
            return equipmentDatabase.IsRangeWeaponEquipped();
        }

        protected override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }

        public override Weapon[] GetRightWeapons()
        {
            return equipmentDatabase.weapons;
        }

        public override Weapon[] GetLeftWeapons()
        {
            return equipmentDatabase.shields;
        }

        public override int GetCurrentRightWeaponIndex()
        {
            return equipmentDatabase.currentWeaponIndex;
        }

        public override int GetCurrentLeftWeaponIndex()
        {
            return equipmentDatabase.currentShieldIndex;
        }
    }
}

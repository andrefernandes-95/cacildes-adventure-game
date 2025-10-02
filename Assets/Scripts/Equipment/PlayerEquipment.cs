namespace AF
{
    public class PlayerEquipment : CharacterBaseEquipment
    {
        public PlayerManager playerManager;

        protected override void UpdateEquipmentValues()
        {
            base.UpdateEquipmentValues();

            playerManager.uIDocumentPlayerHUDV2.UpdateHealthManaAndStaminaBars();
        }

        protected override void SetHelmet(Helmet helmet)
        {
            playerManager.equipmentDatabase.EquipHelmet(helmet);
        }

        protected override void ClearHelmet()
        {
            playerManager.equipmentDatabase.UnequipHelmet();
        }

        protected override void SetAccessory(Accessory accessory, int slotIndex)
        {
            playerManager.equipmentDatabase.EquipAccessory(accessory, slotIndex);
        }

        protected override void ClearAccessory(int slotIndex)
        {
            playerManager.equipmentDatabase.UnequipAccessory(slotIndex);
        }

        protected override void SetArmor(Armor armor)
        {
            playerManager.equipmentDatabase.EquipArmor(armor);
        }

        protected override void ClearArmor()
        {
            playerManager.equipmentDatabase.UnequipArmor();
        }

        protected override void SetGauntlets(Gauntlet gauntlet)
        {
            playerManager.equipmentDatabase.EquipGauntlet(gauntlet);
        }

        protected override void ClearGauntlets()
        {
            playerManager.equipmentDatabase.UnequipGauntlet();
        }

        protected override void SetLegwear(Legwear legwear)
        {
            playerManager.equipmentDatabase.EquipLegwear(legwear);
        }

        protected override void ClearLegwear()
        {
            playerManager.equipmentDatabase.UnequipLegwear();
        }

        public override Helmet GetEquippedHelmet()
        {
            return playerManager.equipmentDatabase.helmet;
        }

        public override Armor GetEquippedArmor()
        {
            return playerManager.equipmentDatabase.armor;
        }

        public override Gauntlet GetEquippedGauntlet()
        {
            return playerManager.equipmentDatabase.gauntlet;
        }

        public override Legwear GetEquippedLegwear()
        {
            return playerManager.equipmentDatabase.legwear;
        }

        public override Accessory[] GetEquippedAccessories()
        {
            return playerManager.equipmentDatabase.accessories;
        }

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }

        public override void EquipWeapon(Weapon weapon, int slotIndex, bool rightHand)
        {
            if (rightHand)
            {
                playerManager.equipmentDatabase.EquipWeapon(weapon, slotIndex, playerManager);
            }
            else
            {
                playerManager.equipmentDatabase.EquipShield(weapon, slotIndex, playerManager);
            }
        }

        public override void UnequipWeapon(int slotIndex, bool rightHand)
        {
            if (rightHand)
            {
                playerManager.equipmentDatabase.UnequipWeapon(slotIndex, playerManager);
            }
            else
            {
                playerManager.equipmentDatabase.UnequipShield(slotIndex, playerManager);
            }
        }

        public override Spell[] GetEquippedSpells()
        {
            return playerManager.equipmentDatabase.spells;
        }

        public override void EquipSpell(Spell spell, int slotIndex)
        {
            playerManager.equipmentDatabase.EquipSpell(spell, slotIndex);
        }

        public override void UnequipSpell(int slotIndex)
        {
            playerManager.equipmentDatabase.UnequipSpell(slotIndex);
        }

        public override Spell GetCurrentEquippedSpell()
        {
            return playerManager.equipmentDatabase.GetCurrentSpell();
        }
    }
}

namespace AF
{
    public class PlayerEquipment : CharacterBaseEquipment
    {
        public PlayerManager playerManager;

        protected override void SetHelmet(Helmet helmet)
        {
            playerManager.equipmentDatabase.helmet = Instantiate(helmet);
        }

        protected override void ClearHelmet()
        {
            playerManager.equipmentDatabase.helmet = null;
        }

        protected override void SetAccessory(Accessory accessory, int slotIndex)
        {
            playerManager.equipmentDatabase.accessories[slotIndex] = Instantiate(accessory);
        }

        protected override void ClearAccessory(int slotIndex)
        {
            playerManager.equipmentDatabase.accessories[slotIndex] = null;
        }

        protected override void SetArmor(Armor armor)
        {
            playerManager.equipmentDatabase.armor = Instantiate(armor);
        }

        protected override void ClearArmor()
        {
            playerManager.equipmentDatabase.armor = null;
        }

        protected override void SetGauntlets(Gauntlet gauntlet)
        {
            playerManager.equipmentDatabase.gauntlet = Instantiate(gauntlet);
        }

        protected override void ClearGauntlets()
        {
            playerManager.equipmentDatabase.gauntlet = null;
        }

        protected override void SetLegwear(Legwear legwear)
        {
            playerManager.equipmentDatabase.legwear = Instantiate(legwear);
        }

        protected override void ClearLegwear()
        {
            playerManager.equipmentDatabase.legwear = null;
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
                playerManager.equipmentDatabase.EquipWeapon(weapon, slotIndex);
            }
            else
            {
                playerManager.equipmentDatabase.EquipShield(weapon, slotIndex);
            }
        }

        public override void UnequipWeapon(int slotIndex, bool rightHand)
        {
            if (rightHand)
            {
                playerManager.equipmentDatabase.UnequipWeapon(slotIndex);
            }
            else
            {
                playerManager.equipmentDatabase.UnequipShield(slotIndex);
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
    }
}

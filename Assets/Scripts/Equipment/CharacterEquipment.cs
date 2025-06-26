using UnityEngine;

namespace AF
{
    public class CharacterEquipment : CharacterBaseEquipment
    {
        public CharacterManager characterManager;

        [Header("Equipment")]
        Helmet helmet;
        Gauntlet gauntlet;
        Armor armor;
        Legwear legwear;
        Accessory[] accessories = new Accessory[4];

        protected override void SetHelmet(Helmet helmet)
        {
            this.helmet = Instantiate(helmet);
        }

        protected override void ClearHelmet()
        {
            this.helmet = null;
        }

        protected override void SetAccessory(Accessory accessory, int slotIndex)
        {
            this.accessories[slotIndex] = Instantiate(accessory);
        }

        protected override void ClearAccessory(int slotIndex)
        {
            this.accessories[slotIndex] = null;
        }

        protected override void SetArmor(Armor armor)
        {
            this.armor = Instantiate(armor);
        }

        protected override void ClearArmor()
        {
            this.armor = null;
        }

        protected override void SetGauntlets(Gauntlet gauntlet)
        {
            this.gauntlet = Instantiate(gauntlet);
        }

        protected override void ClearGauntlets()
        {
            this.gauntlet = null;
        }

        protected override void SetLegwear(Legwear legwear)
        {
            this.legwear = Instantiate(legwear);
        }

        protected override void ClearLegwear()
        {
            this.legwear = null;
        }

        public override Helmet GetEquippedHelmet()
        {
            return this.helmet;
        }

        public override Armor GetEquippedArmor()
        {
            return this.armor;
        }

        public override Gauntlet GetEquippedGauntlet()
        {
            return this.gauntlet;
        }

        public override Legwear GetEquippedLegwear()
        {
            return this.legwear;
        }

        public override Accessory[] GetEquippedAccessories()
        {
            return this.accessories;
        }

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}

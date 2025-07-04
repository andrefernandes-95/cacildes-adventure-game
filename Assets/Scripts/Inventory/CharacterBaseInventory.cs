namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class CharacterBaseInventory : MonoBehaviour
    {
        public abstract List<Weapon> GetWeapons();
        public abstract List<Shield> GetShields();
        public abstract List<Arrow> GetArrows();
        public abstract List<Spell> GetSpells();
        public abstract List<Accessory> GetAccessories();
        public abstract List<Consumable> GetConsumables();
        public abstract List<Helmet> GetHelmets();
        public abstract List<Armor> GetArmors();
        public abstract List<Gauntlet> GetGauntlets();
        public abstract List<Legwear> GetLegwears();
        public abstract List<CraftingMaterial> GetCraftingMaterials();
        public abstract List<UpgradeMaterial> GetUpgradeMaterials();
        public abstract List<KeyItem> GetKeyItems();

        protected string GenerateItemId()
        {
            return Guid.NewGuid().ToString();
        }

        public abstract Weapon AddWeapon(Weapon weapon);
        public abstract Shield AddShield(Shield shield);
        public abstract Helmet AddHelmet(Helmet helmet);
        public abstract Armor AddArmor(Armor armor);
        public abstract Gauntlet AddGauntlet(Gauntlet gauntlet);
        public abstract Legwear AddLegwear(Legwear legwear);
        public abstract Accessory AddAccessory(Accessory accessory);
        public abstract Arrow AddArrow(Arrow arrow);
        public abstract Spell AddSpell(Spell spell);
        public abstract Consumable AddConsumable(Consumable consumable);
        public abstract UpgradeMaterial AddUpgradeMaterial(UpgradeMaterial upgradeMaterial);
        public abstract CraftingMaterial AddCraftingMaterial(CraftingMaterial craftingMaterial);
        public abstract KeyItem AddKeyItem(KeyItem keyItem);

        public void ReplenishItems()
        {
            foreach (Consumable consumable in GetConsumables())
            {
                if (consumable.isRenewable)
                {
                    // consumable.wasUsed = false;
                }
            }
        }

        public int GetCraftingMaterialAmount(Item item)
        {
            return GetCraftingMaterials().Count(craftingMaterial => craftingMaterial.name.Replace("(Clone)", "") == item.name);
        }
        public int GetUpgradeMaterialAmount(Item item)
        {
            return GetUpgradeMaterials().Count(upgradeMaterial => upgradeMaterial.name.Replace("(Clone)", "") == item.name);
        }

    }
}
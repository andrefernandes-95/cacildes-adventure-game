namespace AF
{
    using System.Collections.Generic;
    using System.Linq;

    public class CharacterInventory : CharacterBaseInventory
    {
        public List<Weapon> ownedWeapons = new();
        public List<Spell> ownedSpells = new();
        public List<Arrow> ownedArrows = new();
        public List<Helmet> ownedHelmets = new();
        public List<Armor> ownedArmors = new();
        public List<Legwear> ownedLegwears = new();
        public List<Gauntlet> ownedGauntlets = new();
        public List<Accessory> ownedAccessories = new();
        public List<Consumable> ownedConsumables = new();
        public List<KeyItem> ownedKeyItems = new();
        public List<CraftingMaterial> ownedCraftingMaterials = new();
        public List<UpgradeMaterial> ownedUpgradeMaterials = new();

        // Getters
        public override List<Weapon> GetWeapons() => ownedWeapons;

        public override List<Shield> GetShields() => ownedWeapons.Where(item => item is Shield).OfType<Shield>().ToList();

        public override List<Arrow> GetArrows() => ownedArrows;

        public override List<Spell> GetSpells() => ownedSpells;

        public override List<Accessory> GetAccessories() => ownedAccessories;

        public override List<Consumable> GetConsumables() => ownedConsumables;

        public override List<Helmet> GetHelmets() => ownedHelmets;

        public override List<Armor> GetArmors() => ownedArmors;

        public override List<Gauntlet> GetGauntlets() => ownedGauntlets;

        public override List<Legwear> GetLegwears() => ownedLegwears;

        public override List<CraftingMaterial> GetCraftingMaterials() => ownedCraftingMaterials;

        public override List<UpgradeMaterial> GetUpgradeMaterials() => ownedUpgradeMaterials;

        public override List<KeyItem> GetKeyItems() => ownedKeyItems;

        // Adders
        // Adders
        public override Weapon AddWeapon(Weapon weapon)
        {
            Weapon clone = Instantiate(weapon);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedWeapons.Add(clone);
            return clone;
        }

        public override Shield AddShield(Shield shield)
        {
            Shield clone = Instantiate(shield);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedWeapons.Add(clone);
            return clone;
        }

        public override Helmet AddHelmet(Helmet helmet)
        {
            Helmet clone = Instantiate(helmet);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedHelmets.Add(clone);
            return clone;
        }

        public override Armor AddArmor(Armor armor)
        {
            Armor clone = Instantiate(armor);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedArmors.Add(clone);
            return clone;
        }

        public override Gauntlet AddGauntlet(Gauntlet gauntlet)
        {
            Gauntlet clone = Instantiate(gauntlet);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedGauntlets.Add(clone);
            return clone;
        }

        public override Legwear AddLegwear(Legwear legwear)
        {
            Legwear clone = Instantiate(legwear);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedLegwears.Add(clone);
            return clone;
        }

        public override Accessory AddAccessory(Accessory accessory)
        {
            Accessory clone = Instantiate(accessory);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedAccessories.Add(clone);
            return clone;
        }

        public override Arrow AddArrow(Arrow arrow)
        {
            Arrow clone = Instantiate(arrow);
            clone.itemID = GenerateItemId();
            ownedArrows.Add(clone);
            return clone;
        }

        public override Spell AddSpell(Spell spell)
        {
            Spell clone = Instantiate(spell);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            ownedSpells.Add(clone);
            return clone;
        }

        public override Consumable AddConsumable(Consumable consumable)
        {
            Consumable clone = Instantiate(consumable);
            clone.itemID = GenerateItemId();
            ownedConsumables.Add(clone);
            return clone;
        }

        public override UpgradeMaterial AddUpgradeMaterial(UpgradeMaterial upgradeMaterial)
        {
            UpgradeMaterial clone = Instantiate(upgradeMaterial);
            clone.itemID = GenerateItemId();
            ownedUpgradeMaterials.Add(clone);
            return clone;
        }

        public override CraftingMaterial AddCraftingMaterial(CraftingMaterial craftingMaterial)
        {
            CraftingMaterial clone = Instantiate(craftingMaterial);
            clone.itemID = GenerateItemId();
            ownedCraftingMaterials.Add(clone);
            return clone;
        }

        public override KeyItem AddKeyItem(KeyItem keyItem)
        {
            KeyItem clone = Instantiate(keyItem);
            clone.itemID = GenerateItemId();
            ownedKeyItems.Add(clone);
            return clone;
        }

        public override int GetConsumableAmount(Consumable consumable)
        {
            return ownedConsumables.Count(ownedConsumable => ownedConsumable.EqualsTo(consumable));
        }

        public override void RemoveConsumable(Consumable consumable)
        {
            int idx = ownedConsumables.FindIndex(x => x.EqualsTo(consumable));
            if (idx != -1)
            {
                ownedConsumables.RemoveAt(idx);
            }
        }
    }
}

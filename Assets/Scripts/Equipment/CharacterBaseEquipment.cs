using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterBaseEquipment : MonoBehaviour
    {
        [Header("Skills")]
        [SerializeField] Spell[] defaultSkills = new Spell[10];

        [Header("Default Gear")]
        [SerializeField] Helmet defaultHelmet;
        [SerializeField] Armor defaultArmor;
        [SerializeField] Gauntlet defaultGauntlet;
        [SerializeField] Legwear defaultLegwear;
        [SerializeField] Accessory[] defaultAccessories = new Accessory[4];

        protected virtual void Start()
        {
            SetupDefaultEquipment();
            CallOnEquipEventsForEquippedItemsOnStart();
            UpdateEquipmentValues();
        }

        void CallOnEquipEventsForEquippedItemsOnStart()
        {
            Helmet possibleEquippedHelmet = GetEquippedHelmet();
            if (possibleEquippedHelmet != null)
            {
                possibleEquippedHelmet.OnEquip(GetCharacter());
            }

            Armor possibleEquippedArmor = GetEquippedArmor();
            if (possibleEquippedArmor != null)
            {
                possibleEquippedArmor.OnEquip(GetCharacter());
            }

            Gauntlet possibleEquippedGauntlets = GetEquippedGauntlet();
            if (possibleEquippedGauntlets != null)
            {
                possibleEquippedGauntlets.OnEquip(GetCharacter());
            }

            Legwear possibleEquippedLegwears = GetEquippedLegwear();
            if (possibleEquippedLegwears != null)
            {
                possibleEquippedLegwears.OnEquip(GetCharacter());
            }

            foreach (Accessory accessory in GetEquippedAccessories())
            {
                if (accessory != null)
                {
                    accessory.OnEquip(GetCharacter());
                }
            }
        }

        public void SetupDefaultEquipment()
        {
            for (int slot = 0; slot < defaultSkills.Length; slot++)
            {
                Spell skill = defaultSkills[slot];

                if (skill == null)
                    continue;

                Spell addedSkill = GetCharacter().characterBaseInventory.AddSpell(skill);
                EquipSpell(addedSkill, slot);
            }
            for (int slot = 0; slot < defaultAccessories.Length; slot++)
            {
                Accessory accessory = defaultAccessories[slot];

                if (accessory == null)
                    continue;

                Accessory addedAccessory = GetCharacter().characterBaseInventory.AddAccessory(accessory);
                EquipAccessory(addedAccessory, slot);
            }

            if (defaultHelmet != null)
            {
                Helmet addedHelmet = GetCharacter().characterBaseInventory.AddHelmet(defaultHelmet);
                EquipHelmet(addedHelmet);
            }

            if (defaultArmor != null)
            {
                Armor addedArmor = GetCharacter().characterBaseInventory.AddArmor(defaultArmor);
                EquipArmor(addedArmor);
            }

            if (defaultGauntlet != null)
            {
                Gauntlet addedGauntlet = GetCharacter().characterBaseInventory.AddGauntlet(defaultGauntlet);
                EquipGauntlets(addedGauntlet);
            }

            if (defaultLegwear != null)
            {
                Legwear addedLegwear = GetCharacter().characterBaseInventory.AddLegwear(defaultLegwear);
                EquipLegwear(addedLegwear);
            }
        }

        public abstract Helmet GetEquippedHelmet();
        public abstract Armor GetEquippedArmor();
        public abstract Gauntlet GetEquippedGauntlet();
        public abstract Legwear GetEquippedLegwear();
        public abstract Accessory[] GetEquippedAccessories();
        public abstract Spell[] GetEquippedSpells();

        protected abstract void SetAccessory(Accessory accessory, int slotIndex);
        protected abstract void ClearAccessory(int slotIndex);
        public void EquipAccessory(Accessory accessory, int slotIndex)
        {
            // If accessory already equipped, unequip it
            Accessory possibleEquippedAccessory = GetAccessoryInSlot(slotIndex);
            if (possibleEquippedAccessory != null)
            {
                bool isSameAccessory = accessory != null && possibleEquippedAccessory.itemID == accessory.itemID;

                UnequipAccessory(slotIndex);

                if (isSameAccessory)
                {
                    return;
                }
            }

            if (accessory != null)
            {
                accessory.OnEquip(GetCharacter());
            }

            SetAccessory(accessory, slotIndex);
            UpdateEquipmentValues();
        }
        public void UnequipAccessory(int slotIndex)
        {
            Accessory possibleEquippedAccessory = GetAccessoryInSlot(slotIndex);
            if (possibleEquippedAccessory != null)
            {
                possibleEquippedAccessory.OnUnequip(GetCharacter());
            }

            ClearAccessory(slotIndex);
            UpdateEquipmentValues();
        }


        protected abstract void SetHelmet(Helmet helmet);
        protected abstract void ClearHelmet();
        public void EquipHelmet(Helmet helmet)
        {
            Helmet possibleEquippedHelmet = GetEquippedHelmet();
            if (possibleEquippedHelmet != null)
            {
                bool isSameHelmet = helmet != null && possibleEquippedHelmet.itemID == helmet.itemID;

                UnequipHelmet();

                if (isSameHelmet)
                {
                    return;
                }
            }

            if (helmet != null)
            {
                helmet.OnEquip(GetCharacter());
            }

            SetHelmet(helmet);
            UpdateEquipmentValues();
        }

        public void UnequipHelmet()
        {
            Helmet possibleEquippedHelmet = GetEquippedHelmet();

            if (possibleEquippedHelmet != null)
            {
                possibleEquippedHelmet.OnUnequip(GetCharacter());
            }

            ClearHelmet();
            UpdateEquipmentValues();
        }

        protected abstract void SetArmor(Armor armor);
        protected abstract void ClearArmor();
        public void EquipArmor(Armor armor)
        {
            Armor possibleEquippedArmor = GetEquippedArmor();

            if (possibleEquippedArmor != null)
            {
                bool isSameArmor = armor != null && possibleEquippedArmor.itemID == armor.itemID;

                UnequipArmor();

                if (isSameArmor)
                {
                    return;
                }
            }

            if (armor != null)
            {
                armor.OnEquip(GetCharacter());
            }

            SetArmor(armor);
            UpdateEquipmentValues();
        }
        public void UnequipArmor()
        {
            Armor possibleEquippedArmor = GetEquippedArmor();

            if (possibleEquippedArmor != null)
            {
                possibleEquippedArmor.OnUnequip(GetCharacter());
            }

            ClearArmor();
            UpdateEquipmentValues();
        }


        protected abstract void SetGauntlets(Gauntlet gauntlet);
        protected abstract void ClearGauntlets();
        public void EquipGauntlets(Gauntlet gauntlet)
        {
            Gauntlet possibleEquippedGauntlets = GetEquippedGauntlet();

            if (possibleEquippedGauntlets != null)
            {
                bool isSameGauntlets = gauntlet != null && possibleEquippedGauntlets.itemID == gauntlet.itemID;
                UnequipGauntlets();

                if (isSameGauntlets)
                {
                    return;
                }
            }

            if (gauntlet != null)
            {
                gauntlet.OnEquip(GetCharacter());
            }

            SetGauntlets(gauntlet);
            UpdateEquipmentValues();
        }
        public void UnequipGauntlets()
        {
            Gauntlet possibleEquippedGauntlets = GetEquippedGauntlet();

            if (possibleEquippedGauntlets != null)
            {
                possibleEquippedGauntlets.OnUnequip(GetCharacter());
            }

            ClearGauntlets();
            UpdateEquipmentValues();
        }

        protected abstract void SetLegwear(Legwear legwear);
        protected abstract void ClearLegwear();
        public void EquipLegwear(Legwear legwear)
        {
            Legwear possibleEquippedLegwears = GetEquippedLegwear();

            if (possibleEquippedLegwears != null)
            {
                bool isSameLegwear = legwear != null && possibleEquippedLegwears.itemID == legwear.itemID;

                UnequipLegwear();

                if (isSameLegwear)
                {
                    return;
                }
            }

            if (legwear != null)
            {
                legwear.OnEquip(GetCharacter());
            }

            SetLegwear(legwear);
            UpdateEquipmentValues();
        }
        public void UnequipLegwear()
        {
            Legwear possibleEquippedLegwears = GetEquippedLegwear();

            if (possibleEquippedLegwears != null)
            {
                possibleEquippedLegwears.OnUnequip(GetCharacter());
            }

            ClearLegwear();
            UpdateEquipmentValues();
        }

        protected virtual void UpdateEquipmentValues()
        {
            GetCharacter().statsBonusController.RecalculateEquipmentBonus();
            GetCharacter().characterBaseDefenseManager.RecalculateDamageAbsorbed();
            GetCharacter().statusController.RecalculateResistances();
        }

        public bool IsAccessoryEquiped(Accessory accessory)
        {
            return GetEquippedAccessories().Any(acc => acc != null && acc.itemID == accessory.itemID);
        }

        public Accessory GetAccessoryInSlot(int slot)
        {
            var accessories = GetEquippedAccessories();

            if (slot < 0 || slot >= accessories.Length)
            {
                return null; // Invalid index
            }

            return accessories[slot];
        }

        public abstract void EquipWeapon(Weapon weapon, int slotIndex, bool rightHand);
        public abstract void UnequipWeapon(int slotIndex, bool rightHand);

        public abstract void EquipSpell(Spell spell, int slotIndex);
        public abstract void UnequipSpell(int slotIndex);

        public bool IsNaked()
        {
            return
                GetEquippedHelmet() == null
                && GetEquippedArmor() == null
                && GetEquippedGauntlet() == null
                && GetEquippedLegwear() == null;
        }

        public abstract CharacterBaseManager GetCharacter();

    }
}

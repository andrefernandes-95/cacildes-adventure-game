namespace AF
{
    using System.Collections.Generic;
    using AF.Companions;
    using UnityEngine;

    public class CompanionEquipment : CharacterBaseEquipment
    {
        public CharacterManager characterManager;
        public CompanionsDatabase companionsDatabase;

        CompanionState GetCompanionState() => companionsDatabase.companionsInParty[characterManager.companionID.GetCompanionID()];

        bool IsInParty() => companionsDatabase.IsInParty(characterManager.companionID.GetCompanionID());

        protected override void Start()
        {
            base.Start();

            if (IsInParty())
            {
                Weapon[] rightWeapons = GetCompanionState().rightWeapons;

                for (int i = 0; i < rightWeapons.Length - 1; i++)
                {
                    if (rightWeapons[i] != null)
                    {
                        characterManager.characterWeaponsManager.EquipWeapon(Instantiate(rightWeapons[i]), i, true);
                    }
                }

                Weapon[] leftWeapons = GetCompanionState().leftWeapons;

                for (int i = 0; i < leftWeapons.Length - 1; i++)
                {
                    if (leftWeapons[i] != null)
                    {
                        characterManager.characterWeaponsManager.EquipWeapon(Instantiate(leftWeapons[i]), i, false);
                    }
                }
            }
        }

        protected override void SetHelmet(Helmet helmet)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().helmet = Instantiate(helmet);
        }

        protected override void ClearHelmet()
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().helmet = null;
        }

        protected override void SetAccessory(Accessory accessory, int slotIndex)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().accessories[slotIndex] = Instantiate(accessory);
        }

        protected override void ClearAccessory(int slotIndex)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().accessories[slotIndex] = null;
        }

        protected override void SetArmor(Armor armor)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().armor = Instantiate(armor);
        }

        protected override void ClearArmor()
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().armor = null;
        }

        protected override void SetGauntlets(Gauntlet gauntlet)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().gauntlet = Instantiate(gauntlet);
        }

        protected override void ClearGauntlets()
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().gauntlet = null;
        }

        protected override void SetLegwear(Legwear legwear)
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().legwear = Instantiate(legwear);
        }

        protected override void ClearLegwear()
        {
            if (!IsInParty())
            {
                return;
            }
            GetCompanionState().legwear = null;
        }

        public override void EquipWeapon(Weapon weapon, int slotIndex, bool rightHand)
        {
            if (!IsInParty())
            {
                return;
            }

            if (rightHand)
            {
                GetCompanionState().rightWeapons[slotIndex] = weapon;
            }
            else
            {
                GetCompanionState().leftWeapons[slotIndex] = weapon;
            }

            characterManager.characterWeaponsManager.EquipWeapon(weapon, slotIndex, rightHand);
        }

        public override void UnequipWeapon(int slotIndex, bool rightHand)
        {
            if (!IsInParty())
            {
                return;
            }
            if (rightHand)
            {
                GetCompanionState().rightWeapons[slotIndex] = null;
            }
            else
            {
                GetCompanionState().leftWeapons[slotIndex] = null;
            }

            characterManager.characterWeaponsManager.EquipWeapon(null, slotIndex, rightHand);
        }

        public override Helmet GetEquippedHelmet()
        {
            if (!IsInParty())
            {
                return null;
            }
            return GetCompanionState().helmet;
        }

        public override Armor GetEquippedArmor()
        {
            if (!IsInParty())
            {
                return null;
            }
            return GetCompanionState().armor;
        }

        public override Gauntlet GetEquippedGauntlet()
        {
            if (!IsInParty())
            {
                return null;
            }
            return GetCompanionState().gauntlet;
        }

        public override Legwear GetEquippedLegwear()
        {
            if (!IsInParty())
            {
                return null;
            }
            return GetCompanionState().legwear;
        }

        public override Accessory[] GetEquippedAccessories()
        {
            if (!IsInParty())
            {
                return new List<Accessory>().ToArray();
            }
            return GetCompanionState().accessories;
        }

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}

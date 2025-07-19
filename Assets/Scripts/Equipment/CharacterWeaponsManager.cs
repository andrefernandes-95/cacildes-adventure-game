using System.Linq;
using UnityEngine;

namespace AF.Equipment
{
    public class CharacterWeaponsManager : CharacterBaseWeaponsManager
    {
        [Header("Character")]
        [SerializeField] CharacterManager characterManager;

        [Header("Right Weapons")]
        [SerializeField] Weapon[] rightHandWeapons = new Weapon[3];
        [SerializeField] Weapon[] leftHandWeapons = new Weapon[3];
        int currentRightWeaponIndex = 0;
        int currentLeftWeaponIndex = 0;

        [Header("Two Handing Option")]
        [SerializeField] bool isTwoHanding = false;

        [Header("Custom Weapons")]
        public Hitbox[] weapons;
        public GameObject bow;
        public GameObject shield;
        public bool shouldHideShield = true;

        [Header("Backpack Options")]
        public GameObject unequippedShieldInTheBack;

        void Awake()
        {
            foreach (Weapon wp in rightHandWeapons)
            {
                if (wp != null)
                {
                    Weapon clone = Instantiate(wp);

                    if (clone is Shield shield)
                    {
                        characterManager.characterBaseInventory.AddShield(shield);
                    }
                    else
                    {
                        characterManager.characterBaseInventory.AddWeapon(clone);
                    }
                }
            }
            foreach (Weapon wp in leftHandWeapons)
            {
                if (wp != null)
                {
                    Weapon clone = Instantiate(wp);

                    if (clone is Shield shield)
                    {
                        characterManager.characterBaseInventory.AddShield(shield);
                    }
                    else
                    {
                        characterManager.characterBaseInventory.AddWeapon(clone);
                    }
                }
            }
        }

        void Start()
        {
            UpdateEquipment();
        }

        public override void ShowEquipment()
        {
            base.ShowEquipment();

            ShowWeapon();
            ShowBow();
            ShowShield();
        }

        public override void HideEquipment()
        {
            base.HideEquipment();

            HideWeapon();
            HideBow();
            HideShield();
        }

        public void ShowWeapon()
        {
            if (weapons.Length > 0)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon != null)
                    {
                        weapon.gameObject.SetActive(true);
                    }
                }
            }
        }
        public void HideWeapon()
        {
            if (weapons.Length > 0)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon != null)
                    {
                        weapon.gameObject.SetActive(false);
                    }
                }
            }
        }

        public override void ShowShield()
        {
            base.ShowShield();

            if (shield != null)
            {
                shield.SetActive(true);
            }

            if (unequippedShieldInTheBack != null)
            {
                unequippedShieldInTheBack.SetActive(false);
            }
        }

        public override void HideShield()
        {
            base.HideShield();

            if (shield != null && shouldHideShield)
            {
                shield.SetActive(false);
            }

            if (unequippedShieldInTheBack != null)
            {
                unequippedShieldInTheBack.SetActive(true);
            }
        }

        public void ShowBow()
        {
            if (bow != null)
            {
                bow.SetActive(true);
            }
        }

        public void HideBow()
        {
            if (bow != null)
            {
                bow.SetActive(false);
            }
        }

        public void OpenCharacterWeaponHitbox()
        {
            if (weapons.Length > 0)
            {
                OpenCharacterWeaponHitbox(weapons[0]);
            }
        }

        public void CloseCharacterWeaponHitbox()
        {
            if (weapons.Length > 0)
            {
                CloseCharacterWeaponHitbox(weapons[0]);
            }
        }

        public void OpenCharacterWeaponHitbox(Hitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.EnableHitbox();
        }

        public void CloseCharacterWeaponHitbox(Hitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.DisableHitbox();
        }

        public override void CloseAllWeaponHitboxes()
        {
            base.CloseAllWeaponHitboxes();

            foreach (Hitbox characterWeaponHitbox in weapons)
            {
                characterWeaponHitbox?.DisableHitbox();
            }
        }

        public void OnWeaponSpecial()
        {
            if (weapons.Length > 0)
            {
                foreach (Hitbox weapon in weapons)
                {
                    if (weapon.gameObject.activeSelf)
                    {
                        weapon.onWeaponSpecial?.Invoke();
                    }
                }
            }
        }

        public void SwitchWeapon(int idx, CharacterWeaponHitbox newWeapon)
        {
            if (newWeapon == null)
            {
                return;
            }

            if (weapons.Length > 0)
            {
                if (weapons[idx] != null)
                {
                    weapons[idx].gameObject.SetActive(false);
                }

                weapons[idx] = newWeapon;
                weapons[idx].gameObject.SetActive(true);
            }
        }

        public override Weapon GetCurrentRightWeapon()
        {
            return rightHandWeapons[currentRightWeaponIndex];
        }

        public override Weapon GetCurrentLeftWeapon()
        {
            return leftHandWeapons[currentLeftWeaponIndex];
        }

        public override bool IsTwoHanding()
        {
            return isTwoHanding;
        }

        public override bool HasRangeWeapon()
        {
            return false;
        }

        protected override float GetCharacterUnarmedDefenseAbsorption()
        {
            return characterManager.characterAbstractBlockController.unarmedDefenseAbsorption;
        }

        protected override void UpdateCurrentWeapon()
        {
            base.UpdateCurrentWeapon();
            RefreshAnimations();
        }

        protected override void UpdateCurrentLeftWeapon()
        {
            base.UpdateCurrentLeftWeapon();
            RefreshAnimations();
        }

        protected override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }

        void RefreshAnimations()
        {
            characterManager.UpdateAnimationsBasedOnEquippedWeapons();
        }

        public Weapon GetRangeWeapon() => leftHandWeapons.FirstOrDefault(leftHandWeapon => leftHandWeapon != null && leftHandWeapon.damage.weaponAttackType == WeaponAttackType.Range);

        public Shield FindPotentialShield() => leftHandWeapons.FirstOrDefault(leftHandWeapon => leftHandWeapon != null && leftHandWeapon is Shield) as Shield;

        public void EquipWeapon(Weapon weapon, int slot, bool isRightHand)
        {
            if (isRightHand)
            {
                rightHandWeapons[slot] = weapon;
            }
            else
            {
                leftHandWeapons[slot] = weapon;
            }

            UpdateEquipment();
        }
    }
}

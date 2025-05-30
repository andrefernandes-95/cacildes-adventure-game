using System.Linq;
using UnityEngine;

namespace AF.Equipment
{
    public class CharacterWeaponsManager : MonoBehaviour
    {

        public CharacterWeaponHitbox[] weapons;
        public GameObject bow;
        public GameObject shield;
        public bool shouldHideShield = true;

        [Header("Backpack Options")]
        public GameObject unequippedShieldInTheBack;

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        public void ShowEquipment()
        {
            ShowWeapon();
            ShowBow();
            ShowShield();
        }

        public void HideEquipment()
        {
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
                    weapon.gameObject.SetActive(true);
                }
            }
        }
        public void HideWeapon()
        {
            if (weapons.Length > 0)
            {
                foreach (var weapon in weapons)
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }

        public void ShowShield()
        {
            if (shield != null)
            {
                shield.SetActive(true);
            }

            if (unequippedShieldInTheBack != null)
            {
                unequippedShieldInTheBack.SetActive(false);
            }
        }
        public void HideShield()
        {
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

        public void OpenCharacterWeaponHitbox(CharacterWeaponHitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.EnableHitbox();
        }

        public void CloseCharacterWeaponHitbox(CharacterWeaponHitbox characterWeaponHitbox)
        {
            characterWeaponHitbox?.DisableHitbox();
        }

        public void CloseAllWeaponHitboxes()
        {
            foreach (CharacterWeaponHitbox characterWeaponHitbox in weapons)
            {
                characterWeaponHitbox?.DisableHitbox();
            }
        }

        public void OnWeaponSpecial()
        {
            if (weapons.Length > 0)
            {
                foreach (CharacterWeaponHitbox weapon in weapons)
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
    }
}

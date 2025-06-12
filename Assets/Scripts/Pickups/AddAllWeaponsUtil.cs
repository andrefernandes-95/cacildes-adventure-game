using UnityEngine;

namespace AF
{

    public class AddAllWeaponsUtil : MonoBehaviour
    {
        [SerializeField] PlayerInventory playerInventory;

        [SerializeField] int defaultQuantity = 1;

        public void OnAddAllWeapons()
        {
            Weapon[] weapons = Resources.LoadAll<Weapon>("Items");
            foreach (var weapon in weapons)
            {
                playerInventory.AddItem(weapon, defaultQuantity);
            }
        }
    }
}

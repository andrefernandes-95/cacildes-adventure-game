using UnityEngine;

namespace AF
{

    public class AddAllArmorsUtil : MonoBehaviour
    {
        [SerializeField] PlayerInventory playerInventory;

        [SerializeField] int defaultQuantity = 1;

        public void OnAddAllWeapons()
        {
            ArmorBase[] armors = Resources.LoadAll<ArmorBase>("Items");
            foreach (var armor in armors)
            {
                playerInventory.AddItem(armor, defaultQuantity);
            }
        }
    }
}

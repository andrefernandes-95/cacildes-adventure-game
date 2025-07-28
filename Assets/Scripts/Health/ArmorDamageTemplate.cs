using AF.Health;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(fileName = "Armor Damage Template", menuName = "Data / New Armor Damage Template", order = 0)]
    public class ArmorDamageTemplate : ScriptableObject
    {
        public Damage damage;

    }
}

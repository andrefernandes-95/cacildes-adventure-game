namespace AF
{
    using System.Collections.Generic;
    using AF.Equipment;
    using UnityEngine;

    public class WeaponsManager : MonoBehaviour
    {
        public List<CharacterWeaponHitbox> weaponInstances = new();
        public List<ShieldInstance> shieldInstances = new();

    }
}
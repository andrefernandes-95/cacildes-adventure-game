using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Type")]
    public class WeaponType : ScriptableObject
    {
        [Header("Stamina Settings")]
        [SerializeField] int staminaCost = 30;
        [SerializeField] int heavyAttackStaminaCostMultiplier = 2;

        public int GetLightAttackStaminaCost()
        {
            return staminaCost;
        }
        public int GetHeavyAttackStaminaCost()
        {
            return staminaCost * heavyAttackStaminaCostMultiplier;
        }
    }
}

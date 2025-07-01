using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Sound")]
    public class WeaponSound : ScriptableObject
    {
        public AudioClip swing;
        public AudioClip impact;
    }
}

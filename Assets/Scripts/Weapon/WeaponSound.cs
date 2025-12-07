using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Sound / New Weapon Sound")]
    public class WeaponSound : ScriptableObject
    {
        [SerializeField] AudioClip[] swing;
        [SerializeField] AudioClip[] impact;

        public AudioClip GetSwing() => swing[Random.Range(0, swing.Length)];
        public AudioClip GetImpact() => impact[Random.Range(0, impact.Length)];
    }
}

namespace AF
{
    using System;
    using AF.Combat;
    using UnityEngine;

    public class Destroyable : MonoBehaviour, IDamageable
    {
        [SerializeField] GameObject onDestroyVfx;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnDamageEvent()
        {
            OnDamage(null, null);
        }

        public void OnDamage(CharacterBaseManager attacker, Action onDamageInflicted)
        {
            Instantiate(onDestroyVfx, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

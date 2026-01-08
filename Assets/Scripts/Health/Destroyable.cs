namespace AF
{
    using System;
    using AF.Combat;
    using UnityEngine;
    using UnityEngine.Events;

    public class Destroyable : MonoBehaviour, IDamageable
    {
        [SerializeField] GameObject onDestroyVfx;

        [SerializeField] UnityEvent onDestroy;

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
            onDestroy?.Invoke();
            Destroy(this.gameObject);
        }

        public CharacterBaseManager GetCharacter()
        {
            return null;
        }
    }
}

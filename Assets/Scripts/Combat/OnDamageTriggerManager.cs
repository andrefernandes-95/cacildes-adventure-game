using System.Linq;
using UnityEngine;

namespace AF
{
    public class OnDamageTriggerManager : OnDamageCollisionAbstractManager, IAbilityInstance
    {
        public string[] tagsToDetect;

        public bool onTriggerEnter = true;
        public bool onTriggerStay = false;

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            damageOwner = caster;
            if (caster is PlayerManager)
            {
                tagsToDetect = new string[] { "Enemy" };
            }
            else
            {
                tagsToDetect = new string[] { "Player", "Enemy" };
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!tagsToDetect.Contains(other.gameObject.tag))
            {
                return;
            }

            if (onTriggerEnter)
            {
                OnCollision(other.gameObject);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (!tagsToDetect.Contains(other.gameObject.tag))
            {
                return;
            }

            if (onTriggerStay)
            {
                OnCollision(other.gameObject);
            }
        }

    }
}

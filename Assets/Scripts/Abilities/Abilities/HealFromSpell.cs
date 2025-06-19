namespace AF
{
    using AF.Health;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Heal From Spell", menuName = "Abilities / Spells / New Heal From Spell", order = 0)]
    public class HealFromSpell : CastFromSpell
    {
        [Header("Options")]
        [SerializeField] bool healTargetInstead = false;

        protected override GameObject ReleaseSpellGameObject(CharacterBaseManager damageOwner, string[] tagsToDetect)
        {
            GameObject instance = base.ReleaseSpellGameObject(damageOwner, tagsToDetect);

            HandleHealing(healTargetInstead ? target : damageOwner);

            return instance;
        }

        void HandleHealing(CharacterBaseManager targetToHeal)
        {
            if (targetToHeal == null)
            {
                return;
            }

            if (damage.physical > 0)
            {
                targetToHeal.health.RestoreHealth(damage.physical);
            }

            if (damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                foreach (StatusEffectEntry statusEffect in damage.statusEffects)
                {
                    targetToHeal.statusController.RemoveStatusEffect(statusEffect.statusEffect);
                }
            }
        }
    }
}

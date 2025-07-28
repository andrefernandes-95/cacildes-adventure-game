namespace AF
{
    using System.Linq;
    using AF.Health;
    using EditorAttributes;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Cast Spell", menuName = "Abilities / Spells / New Cast From Spell", order = 0)]
    public class CastFromSpell : Ability
    {
        [HelpBox("For player, it is assigned automatically on PlayerShooter(), but enemies may also use it, so always assign the spell")]
        public Spell spell;

        [Header("FX")]
        public GameObject chargingSpellFX;

        [Header("Settings")]
        [Tooltip("Because the player modal has wrong scaling, when we parent the spell, we need to rescale it appropriately")]
        [SerializeField] float chargingAbilityLocalScale = 100f;

        // Private
        CharacterBaseManager caster;
        protected CharacterBaseManager target;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityBaseManager.SetCurrentAbility(this);
            characterManager.characterWeaponsManager.HideEquipment();
            characterManager.characterAbilityManager.SetIsCharging(true);

            if (chargingSpellFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingSpellFX, characterManager.characterTransformHelper.rightHand);

                chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                characterManager.characterAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion("Cast Spell", 0.1f);

            caster = characterManager;
            target = characterManager.targetManager.currentTarget;
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.SetCurrentAbility(this);
            playerManager.playerWeaponsManager.HideEquipment();
            playerManager.playerAbilityManager.SetIsCharging(true);

            if (chargingSpellFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingSpellFX, playerManager.characterTransformHelper.rightHand);

                chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion("Cast Spell", 0.1f);

            caster = playerManager;
            target = playerManager.lockOnManager.nearestLockOnTarget != null
                ? playerManager.lockOnManager.nearestLockOnTarget.characterManager : null;
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);

            ReleaseSpellGameObject(playerManager, new[] { "Enemy" });
        }

        public override void OnUse(CharacterManager characterManager)
        {
            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);

            ReleaseSpellGameObject(characterManager, new[] { "Player" });
        }

        protected virtual GameObject ReleaseSpellGameObject(CharacterBaseManager damageOwner, string[] tagsToDetect)
        {
            GameObject instance = Instantiate(spell.projectile, caster.transform.position + caster.transform.up, caster.transform.rotation);
            instance.transform.parent = null;

            OnDamageCollisionAbstractManager[] damageCollisionAbstractManagers = Utils.CollectComponentsFromGameObject<OnDamageCollisionAbstractManager>(instance);
            foreach (OnDamageCollisionAbstractManager entry in damageCollisionAbstractManagers)
            {
                entry.damageOwner = damageOwner;
                entry.damage = damage;
                if (entry is OnDamageTriggerManager onDamageTriggerManager)
                {
                    onDamageTriggerManager.tagsToDetect = tagsToDetect;
                }
            }

            if (instance.TryGetComponent(out IAbilityInstance abilityInstance))
            {
                abilityInstance.CastAbility(caster, target);
            }

            ApplySpellStatusEffects(caster);

            return instance;
        }

        void ApplySpellStatusEffects(CharacterBaseManager caster)
        {
            foreach (StatusEffect statusEffect in spell.statusEffects)
            {
                caster.statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();

                caster.statusController.InflictStatusEffect(statusEffect);
            }
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.ClearChargingEffects();
        }

        public override void OnFinished(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.ClearChargingEffects();
        }
    }
}

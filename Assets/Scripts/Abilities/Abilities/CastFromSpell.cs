namespace AF
{
    using System.Linq;
    using System.Security.Cryptography;
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
        [SerializeField] bool shouldHideEquipment = true;
        [SerializeField] bool isChargeable = true;

        [Header("Animations")]
        [SerializeField] string chargedSpellAnimationHash = "Cast Spell";
        [SerializeField] string simpleCastAnimationHash = "Simple Cast";

        // Private
        CharacterBaseManager caster;
        protected CharacterBaseManager target;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityBaseManager.SetCurrentAbility(this);

            if (shouldHideEquipment)
            {
                characterManager.characterWeaponsManager.HideEquipment();
            }

            if (isChargeable)
            {
                characterManager.characterAbilityManager.SetIsCharging(true);

                if (chargingSpellFX != null)
                {
                    GameObject chargingAbilityFXInstance = Instantiate(
                        chargingSpellFX, characterManager.characterTransformHelper.rightHand);

                    chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                    characterManager.characterAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
                }
            }

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(isChargeable ? chargedSpellAnimationHash : simpleCastAnimationHash, 0.1f);

            caster = characterManager;
            target = characterManager.targetManager.currentTarget;
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.SetCurrentAbility(this);

            if (shouldHideEquipment)
            {
                playerManager.playerWeaponsManager.HideEquipment();
            }

            if (isChargeable)
            {
                playerManager.playerAbilityManager.SetIsCharging(true);

                if (chargingSpellFX != null)
                {
                    GameObject chargingAbilityFXInstance = Instantiate(
                        chargingSpellFX, playerManager.characterTransformHelper.rightHand);

                    chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                    playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
                }
            }

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(isChargeable ? chargedSpellAnimationHash : simpleCastAnimationHash, 0.1f);

            caster = playerManager;
            target = playerManager.lockOnManager.nearestLockOnTarget != null
                ? playerManager.lockOnManager.nearestLockOnTarget.characterManager : null;
        }

        public override void OnUse(PlayerManager playerManager)
        {
            Damage clonedDamage = ScalingUtils.GetAbilityDamageForPlayerSpell(
                GetDamage(playerManager),
                playerManager,
                playerManager.characterBaseEquipment.GetCurrentEquippedSpell());

            if (isChargeable)
            {
                clonedDamage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            }

            this.damage = clonedDamage;

            ReleaseSpellGameObject(playerManager, new[] { "Enemy" });
        }

        public override void OnUse(CharacterManager characterManager)
        {
            if (isChargeable)
            {
                damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            }

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

            IAbilityInstance[] abilityInstances = instance.GetComponents<IAbilityInstance>();

            if (abilityInstances.Length > 0)
            {
                foreach (IAbilityInstance entry in abilityInstances)
                {
                    entry.CastAbility(caster, target);
                }
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
            if (isChargeable)
            {
                characterManager.characterAbilityManager.ClearChargingEffects();
            }
        }

        public override void OnFinished(PlayerManager playerManager)
        {
            if (isChargeable)
            {
                playerManager.playerAbilityManager.ClearChargingEffects();
            }
        }
    }
}

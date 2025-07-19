namespace AF
{
    using AF.Health;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Throw Spell Ability", menuName = "Abilities / Spells / New Throw Spell Ability", order = 0)]
    public class ThrowSpellAbility : Ability
    {
        [Header("FX")]
        public GameObject chargingSpellFX;
        public GameObject releaseSpellFX;

        [Header("Settings")]
        [Tooltip("Because the player modal has wrong scaling, when we parent the spell, we need to rescale it appropriately")]
        [SerializeField] float chargingAbilityLocalScale = 100f;

        // Private
        CharacterBaseManager caster;
        CharacterBaseManager target;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.characterAbilityManager.SetIsCharging(true);
            characterManager.characterWeaponsManager.HideEquipment();

            if (chargingSpellFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingSpellFX, characterManager.characterTransformHelper.rightHand ?? characterManager.transform);

                chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                characterManager.characterAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion("Cast Spell", 0.1f);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetCurrentAbility(this);
            playerManager.playerAbilityManager.SetIsCharging(true);
            playerManager.playerWeaponsManager.HideEquipment();

            if (chargingSpellFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingSpellFX, playerManager.characterTransformHelper.rightHand);

                chargingAbilityFXInstance.transform.localScale *= chargingAbilityLocalScale;

                playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion("Cast Spell", 0.1f);
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);

            caster = playerManager;
            target = playerManager.lockOnManager.nearestLockOnTarget != null
                ? playerManager.lockOnManager.nearestLockOnTarget.characterManager : null;

            ReleaseSpellGameObject(playerManager, new[] { "Enemy" });
        }

        public override void OnUse(CharacterManager characterManager)
        {
            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);

            caster = characterManager;
            target = characterManager.targetManager.currentTarget != null
                ? characterManager.targetManager.currentTarget : null;

            characterManager.FaceTarget();

            ReleaseSpellGameObject(characterManager, new[] { "Player" });
        }


        void ReleaseSpellGameObject(CharacterBaseManager damageOwner, string[] tagsToDetect)
        {
            GameObject instance = Instantiate(releaseSpellFX, caster.transform.position + caster.transform.up, Quaternion.identity);
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
            foreach (var abilityInstance in abilityInstances)
            {
                abilityInstance.CastAbility(caster, target);
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

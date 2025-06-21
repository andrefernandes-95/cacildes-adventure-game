namespace AF
{
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
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetCurrentAbility(this);
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
    }
}

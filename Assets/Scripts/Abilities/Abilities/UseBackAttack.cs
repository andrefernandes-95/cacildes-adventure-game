using AF.Equipment;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Back Attack", menuName = "Abilities / AI / New Use Back Attack", order = 0)]
    public class UseBackAttack : Ability
    {
        [Header("Animation Settings")]
        [SerializeField] private string backAttackName = "Back Attack";
        [SerializeField] private float crossFade = 0.025f;

        [Header("Back Attack Settings")]
        [SerializeField, Tooltip("Maximum angle (in degrees) within which the attacker is considered behind the target.")]
        private float maxBackAngle = 60f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            if (!IsBehindTarget(characterManager))
                return;

            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(backAttackName, crossFade);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
                return;

            playerManager.playerAbilityManager.SetCurrentAbility(this);
            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(backAttackName, crossFade);
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);
            playerManager.characterBaseAttackManager.damageBonus = damage;
        }

        public override void OnUse(CharacterManager characterManager)
        {
            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);
            characterManager.characterBaseAttackManager.damageBonus = damage;
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return IsBehindTarget(character);
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return AbilityUtils.GetAbilityDamageForAIAttack(attacker, damage);
        }

        public override void OnFinished(CharacterManager characterManager) { }

        public override void OnFinished(PlayerManager playerManager) { }

        /// <summary>
        /// Returns true if the attacker is positioned behind their target within the allowed angle.
        /// </summary>
        private bool IsBehindTarget(CharacterBaseManager attacker)
        {
            if (attacker.GetTarget() == null)
                return false;

            Transform targetTransform = attacker.GetTarget().transform;
            Vector3 toTarget = (targetTransform.transform.position - attacker.transform.position).normalized;

            // Compare how much the attacker's position is aligned with the target's backward direction
            float angle = Vector3.Angle(attacker.transform.forward * -1f, toTarget);

            return angle <= maxBackAngle;
        }
    }
}

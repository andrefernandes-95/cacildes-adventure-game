using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Slow Down")]
    public class SlowDown : StatusEffectBehaviour
    {
        [SerializeField] float slowedDownSpeed = 0.75f;

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);

            if (characterBaseManager is PlayerManager playerManager)
            {
                playerManager.thirdPersonController.targetSpeedModifier = slowedDownSpeed;
            }
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);

            characterBaseManager.animator.speed = slowedDownSpeed;
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            characterBaseManager.animator.speed = characterBaseManager.GetDefaultAnimatorSpeed();

            if (characterBaseManager is PlayerManager playerManager)
            {
                playerManager.thirdPersonController.targetSpeedModifier = 1f;
            }
        }
    }
}

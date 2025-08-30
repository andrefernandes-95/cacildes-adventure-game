using AF.Equipment;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Teleport", menuName = "Abilities / AI / Teleport", order = 0)]
    public class Teleport : Ability
    {
        [SerializeField] string animationName = "";
        [SerializeField] float crossFade = 0.1f;

        [SerializeField] GameObject teleportBeginVfx;
        [SerializeField] GameObject teleportVfx;
        [SerializeField] bool hasHyperArmor = false;
        [SerializeField] bool shouldReleaseLockOn = false;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(animationName, crossFade);

            if (hasHyperArmor)
            {
                (characterManager.characterPoise as CharacterPoise).hasHyperArmor = true;
            }

            if (teleportBeginVfx != null)
            {
                Instantiate(teleportBeginVfx, characterManager.transform.position, Quaternion.identity);
            }
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
        }

        public override void OnUse(PlayerManager playerManager)
        {
        }

        public override void OnUse(CharacterManager characterManager)
        {
            characterManager.characterTeleportManager.TeleportEnemy();

            if (shouldReleaseLockOn)
            {
                FindAnyObjectByType<LockOnManager>()?.DisableLockOn();
            }

            if (hasHyperArmor)
            {
                (characterManager.characterPoise as CharacterPoise).hasHyperArmor = false;
            }

            if (teleportVfx != null)
            {
                Instantiate(teleportVfx, characterManager.transform.position, Quaternion.identity);
            }
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            if (character is CharacterManager characterManager)
            {
                return characterManager.characterTeleportManager.teleportPoints.Count > 0;
            }

            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return AbilityUtils.GetAbilityDamageForAIAttack(attacker, damage);
        }

        public override void OnFinished(CharacterManager characterManager)
        {
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}

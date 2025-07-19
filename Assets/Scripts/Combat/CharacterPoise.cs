using System.Collections;
using UnityEngine;

namespace AF
{
    public class CharacterPoise : CharacterAbstractPoise
    {
        public int maxPoiseHits = 3;

        public bool hasHyperArmor = false;

        bool ignorePoiseDamage = false;
        readonly float recoverPoiseCooldown = 3f;

        Coroutine ResetIgnorePoiseDamageCoroutine;

        public override void ResetStates()
        {
            hasHyperArmor = false;
        }

        public override bool CanCallPoiseDamagedEvent()
        {
            if (hasHyperArmor)
            {
                return false;
            }

            if (ignorePoiseDamage)
            {
                return false;
            }

            return true;
        }

        public override int GetMaxPoiseHits()
        {
            if (characterManager.combatant != null)
            {
                return characterManager.combatant.maximumPoise;
            }

            return maxPoiseHits;
        }

        public override bool TakePoiseDamage(int poiseDamage)
        {
            bool result = base.TakePoiseDamage(poiseDamage);

            if (result)
            {
                ignorePoiseDamage = true;

                if (ResetIgnorePoiseDamageCoroutine != null)
                {
                    StopCoroutine(ResetIgnorePoiseDamageCoroutine);
                }

                ResetIgnorePoiseDamageCoroutine = StartCoroutine(ResetIgnorePoiseDamage_Coroutine());
            }

            return result;
        }

        IEnumerator ResetIgnorePoiseDamage_Coroutine()
        {
            yield return new WaitForSeconds(recoverPoiseCooldown);
            ignorePoiseDamage = false;
        }

        public override void PlayHitReaction()
        {
            if (characterManager is CharacterManager aiCharacter && aiCharacter.combatant != null && aiCharacter.combatant.isHumanoid)
            {
                // Directional damage
                PlayDirectionalDamage();
                return;
            }
            characterManager.PlayBusyAnimationWithRootMotion("TakingDamage");
        }

        void PlayDirectionalDamage()
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                characterManager.PlayBusyAnimationWithRootMotion("Take Damage - Front");
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                characterManager.PlayBusyAnimationWithRootMotion("Take Damage - Front");
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                characterManager.PlayBusyAnimationWithRootMotion("Take Damage - Back");
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                characterManager.PlayBusyAnimationWithRootMotion("Take Damage - Left");
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                characterManager.PlayBusyAnimationWithRootMotion("Take Damage - Right");
            }
        }
    }
}

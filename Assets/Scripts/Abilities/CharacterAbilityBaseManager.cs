namespace AF
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class CharacterAbilityBaseManager : MonoBehaviour
    {
        public Ability currentAbility;

        [Header("Charging Ability")]
        public float chargingAbilityAmount = 0f;
        public float chargingAbilityMultiplierBonus = 1f;
        public float chargingAbilityMultiplierBonusForFullCharge = 1.5f;
        [HideInInspector] public GameObject chargingAbilityFX;

        [Header("Default Animation Clips")]
        [SerializeField] AnimationClip spellStart;
        [SerializeField] AnimationClip spellHold;
        [SerializeField] AnimationClip spellRelease;
        public bool hasOverridenAnimations = false;

        public abstract void ResetStates();

        public void SetCurrentAbility(Ability ability)
        {
            this.currentAbility = ability;
            chargingAbilityAmount = 0f;
        }

        protected void ResetChargeAnimations(CharacterBaseManager character)
        {
            if (hasOverridenAnimations)
            {
                SetAnimations(character, spellStart, spellHold, spellRelease);
                hasOverridenAnimations = false;
            }
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public abstract void OnPrepareAbility();

        /// <summary>
        /// Animation Event
        /// </summary>
        public abstract void OnUseAbility();

        public virtual bool CanUseAbility()
        {
            if (currentAbility != null)
            {
                return false;
            }

            return true;
        }

        public void SetAnimations(CharacterBaseManager characterBaseManager, AnimationClip start, AnimationClip loop, AnimationClip end)
        {
            Dictionary<string, AnimationClip> clips = new()
            {
                { "unarmed_main_charged_attack_02_charge", start },
                { "unarmed_main_charged_attack_02_hold", loop },
                { "unarmed_main_charged_attack_02_release", end }
            };
            characterBaseManager.UpdateAnimatorOverrideControllerClipsUsingDictionary(clips);
        }

        protected void CleanupChargingAbilitySpell()
        {
            // Clean up charging spells if we were interrupted before
            if (chargingAbilityFX != null)
            {
                foreach (ParticleSystem ps in Utils.CollectComponentsFromGameObject<ParticleSystem>(chargingAbilityFX))
                {
                    if (ps != null)
                    {
                        ps.Stop();
                    }
                }
                foreach (AudioSource audioSource in Utils.CollectComponentsFromGameObject<AudioSource>(chargingAbilityFX))
                {
                    if (audioSource != null)
                    {
                        audioSource.Stop();
                    }
                }

                Destroy(chargingAbilityFX, 2f);

                chargingAbilityFX = null;
            }
        }

        protected void EndChargeAbility()
        {
            GetCharacter().animator.SetBool("isCharging", false);
        }

        protected abstract CharacterBaseManager GetCharacter();
    }
}

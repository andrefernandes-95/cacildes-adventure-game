using System;
using System.Collections.Generic;
using AF.Equipment;
using AF.Events;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF
{
    public class CharacterBattlePhaseShift : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] CharacterManager characterManager;

        [Header("Switch Optins")]
        [SerializeField]
        [Range(0, 1f)]
        float healthPercentageToTriggerPhaseShift = .5f;
        [SerializeField] Moment momentWhenTriggeringPhaseShift;

        [Header("Weapon Change Settings")]
        [SerializeField] SerializedDictionary<CharacterWeaponHitbox, CharacterWeaponHitbox> weaponChanges = new();

        [Header("Animation Changes")]
        [SerializeField] AIHumanoidAnimationOverrideHelper aIHumanoidAnimationOverrideHelper;

        bool hasTriggeredPhaseShift = false;

        PlayerManager _playerManager;

        private void Awake()
        {
            characterManager.health.onHealthChange.AddListener(() =>
            {
                HandlePhaseShift();
            });

            if (momentWhenTriggeringPhaseShift != null)
            {
                momentWhenTriggeringPhaseShift.onMoment_End.AddListener(ResumeCombat);
            }
        }

        void HandlePhaseShift()
        {
            if (hasTriggeredPhaseShift)
            {
                return;
            }

            if (characterManager.health.GetCurrentHealthPercentage() > healthPercentageToTriggerPhaseShift * 100f)
            {
                return;
            }

            hasTriggeredPhaseShift = true;

            PauseCombat();
        }

        void PauseCombat()
        {
            GetPlayerManager().playerComponentManager.DisablePlayerControl();

            characterManager.characterCombatController.SetIsPaused(true);

            if (momentWhenTriggeringPhaseShift != null)
            {
                momentWhenTriggeringPhaseShift.Trigger();
            }
        }

        void ResumeCombat()
        {
            GetPlayerManager().playerComponentManager.EnablePlayerControl();

            characterManager.characterCombatController.SetIsPaused(false);
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ChangeWeapons()
        {
            if (weaponChanges != null && weaponChanges.Count > 0)
            {
                CharacterWeaponsManager characterWeaponsManager = characterManager.characterWeaponsManager;

                foreach (var entry in weaponChanges)
                {
                    CharacterWeaponHitbox oldWeapon = entry.Key;
                    CharacterWeaponHitbox newWeapon = entry.Value;

                    int idxOfOldWeapon = Array.IndexOf(characterWeaponsManager.weapons, oldWeapon);

                    if (idxOfOldWeapon != -1)
                    {
                        characterManager.characterWeaponsManager.SwitchWeapon(idxOfOldWeapon, newWeapon);
                    }
                }
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ChangeAnimations()
        {
            if (aIHumanoidAnimationOverrideHelper != null)
            {
                Dictionary<string, AnimationClip> clipOverridesForAIHumanoid = aIHumanoidAnimationOverrideHelper.GetClipOverrides();
                foreach (var entry in clipOverridesForAIHumanoid)
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(entry.Key, entry.Value);
                }
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AF.Animations;
using AF.Events;
using AF.Health;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{
    public class CharacterCombatController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Combat Actions")]
        public List<CombatAction> reactionsToTarget = new();
        public List<CombatAction> combatActions = new();
        public List<CombatAction> chaseActions = new();

        [Header("Abilities")]
        public List<Ability> reactionsToTargetAbilities = new();
        public List<Ability> chaseCombatAbilities = new();
        public List<Ability> combatAbilities = new();

        [System.Serializable]
        public class HealthDependantAbility
        {
            public Ability ability;
            [Range(0f, 100f)] public float minimumHealthToUse = 0f;
            [Range(0f, 100f)] public float maximumHealthToUse = 100f;
        }

        [Header("Conditional Abilities")]
        [SerializeField] List<HealthDependantAbility> healthDependantAbilities = new();

        [Header("Directional")]
        public CombatAction reactionToTargetBehindBack;
        [Range(0, 100f)] public float chanceToReactToTargetBehindBack = 100f;
        public CombatAction currentCombatAction = null;

        [Header("Combat Options")]
        [Range(0, 100f)] public float chanceToReact = 90f;

        public List<CombatAction> usedCombatActions = new();
        public Dictionary<Ability, float> usedAbilities = new();

        [Header("Animation Settings")]
        public string ANIMATION_CLIP_TO_OVERRIDE_NAME = "Cacildes - Light Attack - 1";
        public string PRE_PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Pre Pre Combo Attack";
        public string PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Pre Combo Attack";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Combo Attack";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK = "Cacildes - Combo Attack - Follow Up";
        public string hashLightAttack1 = "Light Attack 1";
        public string hashComboAttack = "Combo Attack Initiator";
        public string hashPreComboAttack = "Pre Combo Attack Initiator";
        public string hashPrePreComboAttack = "Pre Pre Combo Attack Initiator";


        [Header("Unity Events")]
        public UnityEvent onResetState;

        public const string AttackSpeedHash = "AttackSpeed";

        [Header("Dodge Counter")]

        public bool listenForDodgeInput = false;
        CharacterAnimationEventListener characterAnimationEventListener;
        public CombatAction combatActionToRespondToDodgeInput;
        public float chanceToReactToDodgeInput = 0.75f;

        [Header("Pause Options")]
        [SerializeField] bool isPaused = false;

        float timeSinceLastAttack;
        [SerializeField] float waitTimeBetweenCombatActions = 1f;

        private void Awake()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);


            if (characterManager.characterCombatController.listenForDodgeInput)
            {
                EventManager.StartListening(EventMessages.ON_PLAYER_DODGING_FINISHED, OnPlayerDodgeFinished);
            }
        }

        public void ResetStates()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);

            onResetState?.Invoke();

            OnAttackEnd();

            timeSinceLastAttack = Time.time;
        }

        bool CanReact()
        {
            if (reactionsToTarget.Count <= 0 && reactionsToTargetAbilities.Count <= 0)
            {
                return false;
            }

            if (Random.Range(0, 100) > chanceToReact)
            {
                return false;
            }

            return characterManager.targetManager.IsTargetBusy() || characterManager.targetManager.IsTargetShooting();
        }

        public bool IsReactingAgainstBackstab()
        {
            // If is not in combat, do not react against backstab since that would be unfair to stealth players
            if (characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            if (reactionToTargetBehindBack != null && Random.Range(0, 100) < chanceToReactToTargetBehindBack)
            {
                UseCombatAction(reactionToTargetBehindBack);
                return true;
            }

            return false;
        }

        CombatAction GetCombatAction()
        {
            if (CanReact())
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        return possibleReaction;
                    }
                }
            }

            if (combatActions.Count > 0)
            {
                var shuffledCombatActions = Randomize(combatActions.ToArray());

                foreach (CombatAction possibleCombatAction in shuffledCombatActions)
                {
                    if (possibleCombatAction != null && possibleCombatAction.CanUseCombatAction())
                    {
                        return possibleCombatAction;
                    }
                }
            }

            return null;
        }

        Ability GetCombatAbility(List<Ability> abilities)
        {
            if (abilities.Count > 0)
            {
                var shuffledAbilities = Randomize(abilities.ToArray());

                foreach (Ability ability in shuffledAbilities)
                {
                    if (usedAbilities.ContainsKey(ability))
                    {
                        continue;
                    }

                    if (ability != null && ability.CanUseAbility(characterManager))
                    {
                        return ability;
                    }
                }
            }

            return null;
        }

        public bool InCooldown()
        {
            if (characterManager.characterAbilityManager.currentAbility != null)
            {
                return true;
            }

            return Time.time < timeSinceLastAttack + waitTimeBetweenCombatActions;
        }

        bool TryUseAbility()
        {
            List<Ability> allCombatAbilities = new();
            foreach (Ability ability in combatAbilities)
            {
                allCombatAbilities.Add(ability);
            }

            foreach (HealthDependantAbility healthDependantAbility in healthDependantAbilities)
            {
                if (characterManager.health.GetCurrentHealthPercentage() >= healthDependantAbility.minimumHealthToUse &&
                    characterManager.health.GetCurrentHealthPercentage() <= healthDependantAbility.maximumHealthToUse
                )
                {
                    allCombatAbilities.Add(healthDependantAbility.ability);
                }
            }

            Ability combatAbility = GetCombatAbility(allCombatAbilities);
            if (combatAbility != null)
            {
                characterManager.characterAbilityManager.QueueAbility(Instantiate(combatAbility));
                AddAbilityToUsedAbilities(combatAbility);
                return true;
            }

            return false;
        }

        public void UseCombatAction()
        {
            if (InCooldown())
            {
                return;
            }

            CheckAbilityCooldowns();

            if (CanReact())
            {
                Ability reactionAbility = GetCombatAbility(reactionsToTargetAbilities);

                if (reactionAbility != null && reactionAbility.CanUseAbility(characterManager))
                {
                    characterManager.characterAbilityManager.QueueAbility(Instantiate(reactionAbility));
                    AddAbilityToUsedAbilities(reactionAbility);
                    return;
                }
            }

            if (TryUseAbility())
            {
                return;
            }

            CombatAction newCombatAction = GetCombatAction();
            if (newCombatAction == null)
            {
                return;
            }

            this.currentCombatAction = newCombatAction;
            ExecuteCurrentCombatAction(0f);
        }

        void UseCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            ExecuteCurrentCombatAction(0f);
        }

        public void UseChaseAction()
        {
            Ability combatAbility = GetCombatAbility(chaseCombatAbilities);
            if (combatAbility != null)
            {
                characterManager.characterAbilityManager.QueueAbility(Instantiate(combatAbility));
                return;
            }

            CombatAction newCombatAction = null;

            // If target is aiming, let us try to dodge the aim
            if (reactionsToTarget.Count > 0 && characterManager.targetManager.IsTargetShooting())
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        newCombatAction = possibleReaction;
                        break;
                    }
                }
            }
            else if (chaseActions.Count > 0)
            {
                var shuffledChaseActions = Randomize(chaseActions.ToArray());

                foreach (CombatAction possibleChaseAction in shuffledChaseActions)
                {
                    if (possibleChaseAction.CanUseCombatAction())
                    {
                        newCombatAction = possibleChaseAction;
                        break;
                    }
                }
            }

            if (newCombatAction != null)
            {
                this.currentCombatAction = newCombatAction;
                ExecuteCurrentCombatAction(0f);
            }
        }

        public void ExecuteCurrentCombatAction(float crossFade)
        {
            if (isPaused)
            {
                return;
            }

            if (currentCombatAction.hasHyperArmor)
            {
                (characterManager.characterPoise as CharacterPoise).hasHyperArmor = true;
            }

            if (currentCombatAction.attackAnimationClip != null)
            {
                if (currentCombatAction.comboClip != null)
                {
                    if (currentCombatAction.comboClip2 != null)
                    {
                        if (currentCombatAction.comboClip3 != null)
                        {
                            characterManager.UpdateAnimatorOverrideControllerClips(PRE_PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                            characterManager.UpdateAnimatorOverrideControllerClips(PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip);
                            characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip2);
                            characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip3);
                        }
                        else
                        {
                            characterManager.UpdateAnimatorOverrideControllerClips(PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                            characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip);
                            characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip2);
                        }
                    }
                    else
                    {
                        characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                        characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip);
                    }
                }
                else
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(ANIMATION_CLIP_TO_OVERRIDE_NAME, currentCombatAction.attackAnimationClip);
                }

#pragma warning disable CS0618 // Type or member is obsolete
                characterManager.animator.ForceStateNormalizedTime(0f);
#pragma warning restore CS0618 // Type or member is obsolete

                if (currentCombatAction.animationSpeed != 1f)
                {
                    characterManager.animator.SetFloat(AttackSpeedHash, currentCombatAction.animationSpeed);
                }

                if (currentCombatAction.comboClip != null)
                {
                    if (currentCombatAction.comboClip2 != null)
                    {
                        if (currentCombatAction.comboClip3 != null)
                        {
                            characterManager.PlayBusyAnimationWithRootMotion(hashPrePreComboAttack);
                        }
                        else
                        {
                            characterManager.PlayBusyAnimationWithRootMotion(hashPreComboAttack);
                        }
                    }
                    else
                    {
                        characterManager.PlayBusyAnimationWithRootMotion(hashComboAttack);
                    }
                }
                else if (crossFade > 0)
                {
                    characterManager.PlayAnimationWithCrossFade(hashLightAttack1, true, true, crossFade);
                }
                else
                {
                    characterManager.PlayBusyAnimationWithRootMotion(hashLightAttack1);
                }
            }
            else if (!string.IsNullOrEmpty(currentCombatAction.attackAnimationName))
            {
                characterManager.PlayBusyAnimationWithRootMotion(currentCombatAction.attackAnimationName);
            }

            StartCoroutine(ClearCombatActionFromCooldownList(currentCombatAction));

            this.usedCombatActions.Add(currentCombatAction);

            OnAttackStart();
        }

        void OnPlayerDodgeFinished()
        {
            if (combatActionToRespondToDodgeInput == null)
            {
                return;
            }

            if (Random.Range(0, 1f) < chanceToReactToDodgeInput)
            {
                return;
            }

            if (characterAnimationEventListener == null)
            {
                characterAnimationEventListener = characterManager.GetComponent<CharacterAnimationEventListener>();
            }

            characterManager.FaceTarget();
            characterAnimationEventListener.RestoreDefaultAnimatorSpeed();

            this.currentCombatAction = combatActionToRespondToDodgeInput;
            ExecuteCurrentCombatAction(0.15f);
        }

        IEnumerator ClearCombatActionFromCooldownList(CombatAction combatActionToClear)
        {
            yield return new WaitForSeconds(combatActionToClear.maxCooldown);

            if (usedCombatActions.Contains(combatActionToClear))
            {
                usedCombatActions.Remove(combatActionToClear);
            }
        }

        public void OnAttackStart()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_Start?.Invoke();
            }
        }
        public void OnAttack_HitboxOpen()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_HitboxOpen?.Invoke();
            }
        }
        public void OnAttackEnd()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_End?.Invoke();
                currentCombatAction = null;
            }
        }

        public IEnumerable<T> Randomize<T>(T[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }

        public void SetCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
        }

        public Damage GetCurrentDamage()
        {
            return currentCombatAction?.damage;
        }

        public void SetIsPaused(bool value)
        {
            this.isPaused = value;
        }

        void AddAbilityToUsedAbilities(Ability combatAbility)
        {
            if (usedAbilities.ContainsKey(combatAbility))
            {
                return;
            }

            usedAbilities.Add(combatAbility, Time.time + combatAbility.cooldown);
        }

        void CheckAbilityCooldowns()
        {
            List<Ability> abilitiesToRemove = new();

            foreach (var usedAbility in usedAbilities)
            {
                if (Time.time > usedAbility.Value)
                {
                    abilitiesToRemove.Add(usedAbility.Key);
                }
            }

            foreach (Ability abilityToRemove in abilitiesToRemove)
            {
                if (usedAbilities.ContainsKey(abilityToRemove))
                {
                    usedAbilities.Remove(abilityToRemove);
                }
            }
        }
    }
}

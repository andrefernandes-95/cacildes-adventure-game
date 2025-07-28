using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using AF.Health;

namespace AF.StatusEffects
{
    public abstract class StatusController : MonoBehaviour
    {
        [Header("Character")]
        public CharacterBaseManager characterBaseManager;

        [Header("Status Effect Instances")]
        [SerializedDictionary("Status Effect", "Instance")]
        public SerializedDictionary<StatusEffect, StatusEffectInstance> statusEffectInstances;

        [Header("UI")]
        public UIDocumentStatusEffectApplied uIDocumentStatusEffectApplied;

        [Header("Unity Events")]
        public UnityEvent onAwake;

        [Header("Game Session")]
        public GameSession gameSession;

        [SerializeField] SerializedDictionary<StatusEffect, float> calculatedResistances = new();
        [SerializeField] SerializedDictionary<StatusEffect, float> calculatedDelayRates = new();
        [SerializeField] SerializedDictionary<StatusEffect, GameObject> startEffectsVfx = new();
        [SerializeField] SerializedDictionary<StatusEffect, GameObject> updateEffectsVfx = new();

        List<StatusEffect> effectsToRemove = new();

        private void Awake()
        {
            onAwake?.Invoke();

            if (gameSession != null && gameSession.currentGameIteration > 0)
            {
                ScaleResistancesToNewGamePlus();
            }
        }

        /// <summary>
        /// Inflicts the status effect with maximum buildup, causing the effect to be applied immediately
        /// </summary>
        /// <param name="effect"></param>
        public void InflictStatusEffect(StatusEffect effect)
        {
            float amount = GetMaximumResistance(effect);
            InflictStatusEffect(effect, amount);
        }

        /// <summary>
        /// Inflicts the status effect with the buildup given by the raw amount
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="rawAmount"></param>
        public void InflictStatusEffect(StatusEffect effect, float rawAmount)
        {
            var activeEffects = GetActiveEffects();
            float amount = ApplyResistanceModifiers(effect, rawAmount);
            float maximumResistance = GetMaximumResistance(effect);

            // Try to get current state
            if (!activeEffects.TryGetValue(effect, out var state))
            {
                state = new StatusEffectState
                {
                    currentAmount = 0f,
                    hasReachedTotalAmount = false
                };
                activeEffects.Add(effect, state);
                characterBaseManager.characterHUD.AddStatusEffectBar(effect); // Add bar for new effect
            }

            // Prevent further buildup if effect is already applied
            if (state.hasReachedTotalAmount)
            {
                return;
            }

            // Apply buildup
            state.currentAmount = Mathf.Clamp(state.currentAmount + amount, 0, maximumResistance);

            // Trigger effect when max reached
            if (state.currentAmount >= maximumResistance && !state.hasReachedTotalAmount)
            {
                state.hasReachedTotalAmount = true;
                ApplyEffect(effect, state);
            }

            characterBaseManager.characterHUD.UpdateStatusEffectBar(effect, state.currentAmount, maximumResistance, state.hasReachedTotalAmount);
        }

        private void ApplyEffect(StatusEffect effect, StatusEffectState state)
        {
            if (uIDocumentStatusEffectApplied != null)
            {
                uIDocumentStatusEffectApplied.Display(effect);
            }

            StatusEffectBehaviour statusEffectBehaviour = effect.statusEffectBehaviour;
            if (statusEffectBehaviour != null)
            {
                statusEffectBehaviour.OnApplied(characterBaseManager, effect);
            }
        }

        void Update()
        {
            var activeEffects = GetActiveEffects();

            if (activeEffects.Count == 0)
            {
                return;
            }

            effectsToRemove.Clear();

            foreach (var kvp in activeEffects.ToList())
            {
                var effect = kvp.Key;
                var state = kvp.Value;

                if (ShouldRemove(effect, state))
                {
                    effectsToRemove.Add(effect);
                    continue;
                }

                float decayRate = state.hasReachedTotalAmount
                    ? effect.decreaseRateWithDamage
                    : effect.decreaseRateWithoutDamage;

                state.currentAmount -= decayRate * Time.deltaTime;

                characterBaseManager.characterHUD.UpdateStatusEffectBar(
                    effect,
                    state.currentAmount,
                    GetMaximumResistance(effect),
                    state.hasReachedTotalAmount);

                if (state.hasReachedTotalAmount)
                {
                    effect.statusEffectBehaviour?.OnUpdate(characterBaseManager, effect);
                }
            }

            foreach (var effectToRemove in effectsToRemove)
            {
                RemoveEffect(effectToRemove);
            }
        }

        private bool ShouldRemove(StatusEffect effect, StatusEffectState state)
        {
            if (characterBaseManager?.health?.GetCurrentHealth() <= 0)
            {
                return true;
            }

            if (state.hasReachedTotalAmount && effect.isAppliedImmediately)
            {
                return true;
            }

            return state.currentAmount <= 0;
        }

        public void RemoveEffect(StatusEffect effect)
        {
            if (!GetActiveEffects().ContainsKey(effect))
            {
                return;
            }

            StatusEffectBehaviour statusEffectBehaviour = effect.statusEffectBehaviour;
            if (statusEffectBehaviour != null)
            {
                statusEffectBehaviour.OnRemoved(characterBaseManager, effect);
            }

            characterBaseManager.characterHUD.RemoveStatusEffectBar(effect);
            GetActiveEffects().Remove(effect);
            RemoveVfx(effect);
        }

        public void RemoveAllEffects()
        {
            List<StatusEffect> activeEffects = GetActiveEffects().Select(x => x.Key).ToList();

            foreach (var effect in activeEffects)
            {
                RemoveEffect(effect);
            }
        }

        private float ApplyResistanceModifiers(StatusEffect effect, float amount)
        {
            if (calculatedDelayRates.TryGetValue(effect, out var rate))
                amount *= rate;

            return amount;
        }

        private float GetMaximumResistance(StatusEffect effect)
        {
            if (calculatedResistances.TryGetValue(effect, out var resistance))
            {
                return resistance;
            }

            return effect.fallbackResistance; // fallback
        }

        public abstract SerializedDictionary<StatusEffect, StatusEffectState> GetActiveEffects();

        public int GetCurrentResistanceForStatusEffect(StatusEffect statusEffect)
        {
            if (calculatedResistances.ContainsKey(statusEffect))
            {
                return (int)calculatedResistances[statusEffect];
            }

            return 0;
        }

        public void RecalculateResistances()
        {
            calculatedResistances.Clear();
            AddOrUpdate(calculatedResistances, characterBaseManager.combatant?.statusEffectResistances);
            AddOrUpdate(calculatedResistances, characterBaseManager.statsBonusController.statusEffectResistances);

            calculatedDelayRates.Clear();
            AddOrUpdate(calculatedDelayRates, characterBaseManager.combatant?.statusEffectDelayRates, true);
            AddOrUpdate(calculatedDelayRates, characterBaseManager.statsBonusController.statusEffectDelayRates, true);
        }

        void AddOrUpdate(Dictionary<StatusEffect, float> target, Dictionary<StatusEffect, float> source, bool isDelay = false)
        {
            source ??= new();

            foreach (var kvp in source)
            {
                if (target.ContainsKey(kvp.Key))
                {
                    if (isDelay)
                        target[kvp.Key] = Mathf.Clamp(target[kvp.Key] * kvp.Value, 0.01f, 1f);
                    else
                        target[kvp.Key] += kvp.Value;
                }
                else
                {
                    target[kvp.Key] = kvp.Value;
                }
            }
        }

        void ScaleResistancesToNewGamePlus()
        {
            // Future resistance scaling logic (optional)
        }

        public void InstantiateStartVfx(StatusEffect statusEffect, GameObject vfx)
        {
            if (startEffectsVfx.TryGetValue(statusEffect, out var existing) && existing != null)
            {
                Destroy(existing);
                startEffectsVfx.Remove(statusEffect);
            }

            GameObject instance = Instantiate(vfx, characterBaseManager.characterTransformHelper.torso);
            startEffectsVfx[statusEffect] = instance;
        }

        public void InstantiateUpdateVfx(StatusEffect statusEffect, GameObject vfx)
        {
            if (updateEffectsVfx.TryGetValue(statusEffect, out var existing) && existing != null)
            {
                return;
            }

            GameObject instance = Instantiate(vfx, characterBaseManager.characterTransformHelper.torso);
            updateEffectsVfx[statusEffect] = instance;
        }

        public void RemoveVfx(StatusEffect statusEffect)
        {
            if (updateEffectsVfx.ContainsKey(statusEffect))
            {
                GameObject tmpInstance = updateEffectsVfx[statusEffect];
                updateEffectsVfx.Remove(statusEffect);

                if (tmpInstance != null)
                {
                    Destroy(tmpInstance);
                }
            }
        }
    }
}

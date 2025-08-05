namespace AF
{
    using System.Collections.Generic;
    using AF.Health;
    using UnityEngine;

    public class CharacterHUD : MonoBehaviour
    {
        CharacterBaseManager characterBaseManager;

        [Header("UI")]
        [SerializeField] CharacterHealthUI characterHealthUI;
        [SerializeField] CharacterPostureUI characterPostureUI;
        [SerializeField] CharacterDamageUI characterDamageUI;
        [SerializeField] CombatantNameUI combatantNameUI;

        [Header("Prefabs")]
        [SerializeField] CharacterStatusEffectUI characterStatusEffectUIPrefab;

        [Header("References")]
        [SerializeField] Transform hudContainerToSpawnObjects;

        public Dictionary<StatusEffect, CharacterStatusEffectUI> statusEffectBars = new();

        // Subscribe after Awake() to ensure all health posture values have been calculated first
        private void Start()
        {
            characterBaseManager = GetComponentInParent<CharacterBaseManager>();

            if (characterBaseManager is CharacterManager characterManager)
            {
                combatantNameUI.SetupCombatantName(characterManager);
            }
            else
            {
                combatantNameUI.gameObject.SetActive(false);
            }

            characterDamageUI.SetupEvents(characterBaseManager);
            characterHealthUI.SetupEvents(characterBaseManager);
            characterPostureUI.SetupEvents(characterBaseManager);
        }

        public void AddStatusEffectBar(StatusEffect statusEffectToAdd)
        {
            if (statusEffectBars.ContainsKey(statusEffectToAdd))
            {
                return;
            }

            CharacterStatusEffectUI instance = Instantiate(characterStatusEffectUIPrefab, hudContainerToSpawnObjects);

            statusEffectBars.Add(statusEffectToAdd, instance);
        }

        public void UpdateStatusEffectBar(StatusEffect statusEffectToUpdate, float amount, float maxAmount, bool isApplied)
        {
            if (statusEffectBars.ContainsKey(statusEffectToUpdate))
            {
                statusEffectBars[statusEffectToUpdate].UpdateUI(statusEffectToUpdate, amount, maxAmount, isApplied);
            }
        }

        public void RemoveStatusEffectBar(StatusEffect statusEffectToRemove)
        {
            if (statusEffectBars.ContainsKey(statusEffectToRemove))
            {
                GameObject tmp = statusEffectBars[statusEffectToRemove].gameObject;
                statusEffectBars.Remove(statusEffectToRemove);
                Destroy(tmp);
            }
        }
    }
}

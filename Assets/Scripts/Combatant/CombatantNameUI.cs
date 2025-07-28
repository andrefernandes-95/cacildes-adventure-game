namespace AF
{
    using EditorAttributes;
    using UnityEngine;

    public class CombatantNameUI : MonoBehaviour
    {
        CharacterManager characterManager;
        public TMPro.TextMeshProUGUI textMeshPro;

        [HelpBox("For enemies that hidden - ambush state, for example - check this checkbox")]
        [SerializeField] bool showOnlyInCombat = false;

        public void SetupCombatantName(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
            characterManager.targetManager.onTargetSet_Event.AddListener(ShowCombatantName);
            characterManager.targetManager.onClearTarget_Event.AddListener(HideCombatantName);

            HandleOnEnable();
        }

        void OnEnable()
        {
            HandleOnEnable();
        }

        void HandleOnEnable()
        {
            HideCombatantName();

            if (!showOnlyInCombat)
            {
                ShowCombatantName();
            }
        }

        void ShowCombatantName()
        {
            if (characterManager == null || characterManager.combatant == null || characterManager.combatant.combatantName.IsEmpty)
            {
                textMeshPro.text = "";
            }
            else
            {
                textMeshPro.text = characterManager.combatant.combatantName.GetLocalizedString();
            }
        }

        void HideCombatantName()
        {
            if (showOnlyInCombat)
            {
                textMeshPro.text = "";
            }
        }
    }
}

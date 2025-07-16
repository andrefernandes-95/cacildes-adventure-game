namespace AF
{
    using EditorAttributes;
    using UnityEngine;

    public class CombatantNameUI : MonoBehaviour
    {
        [SerializeField] CharacterManager characterManager;
        public TMPro.TextMeshProUGUI textMeshPro;

        [HelpBox("For enemies that hidden - ambush state, for example - check this checkbox")]
        [SerializeField] bool showOnlyInCombat = false;

        void Awake()
        {
            characterManager.targetManager.onTargetSet_Event.AddListener(ShowCombatantName);
            characterManager.targetManager.onClearTarget_Event.AddListener(HideCombatantName);
        }

        void OnEnable()
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

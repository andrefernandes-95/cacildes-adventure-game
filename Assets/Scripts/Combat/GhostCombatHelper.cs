namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;

    public class GhostCombatHelper : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material ghostMaterial;

        [Header("Events")]
        [SerializeField] private UnityEvent onExitingGhostForm;

        private void Start()
        {
            if (characterManager == null)
            {
                Debug.LogError($"{nameof(GhostCombatHelper)}: CharacterManager not assigned.");
                return;
            }

            SetMaterial(ghostMaterial);
            characterManager.isInGhostForm = true;

            characterManager.characterAbilityManager.OnDequeueAbility.AddListener(OnAttack);
            characterManager.onResetStates.AddListener(OnResetStates);
        }

        private void OnDestroy()
        {
            if (characterManager == null) return;

            characterManager.characterAbilityManager.OnDequeueAbility.RemoveListener(OnAttack);
            characterManager.onResetStates.RemoveListener(OnResetStates);
        }

        private void OnAttack()
        {
            onExitingGhostForm?.Invoke();
            SetMaterial(activeMaterial);
            characterManager.isInGhostForm = false;
        }

        private void OnResetStates()
        {
            SetMaterial(ghostMaterial);
            characterManager.isInGhostForm = true;
        }

        private void SetMaterial(Material material)
        {
            if (meshRenderers == null || meshRenderers.Length == 0) return;

            foreach (var meshRenderer in meshRenderers)
            {
                if (meshRenderer != null)
                {
                    meshRenderer.sharedMaterial = material;
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;

namespace AF
{
    public class UIDocumentKeyPrompt : MonoBehaviour
    {
        public UIDocument uiDocument => GetComponent<UIDocument>();

        [Header("Components")]
        public Soundbank soundbank;

        public GenericTrigger currentGenericTrigger;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayPrompt(string key, string action)
        {
            DisplayPrompt(key, action, null);
        }

        public void DisplayPrompt(string key, string action, Item item)
        {
            this.gameObject.SetActive(true);

            VisualElement root = uiDocument.rootVisualElement;

            soundbank.PlayUIHoverSound();

            DOTween.To(
                  () => root.contentContainer.style.opacity.value,
                  (value) => root.contentContainer.style.opacity = value,
                  1,
                  .25f
            );

            root.Q<Label>("InputText").text = action;
            root.Q<Label>("IngredientDescription").style.display = DisplayStyle.None;

            if (item != null)
            {
                HandleAlchemyInfoTooltip(root, item);
            }
        }

        void HandleAlchemyInfoTooltip(VisualElement root, Item item)
        {
            if (item == null)
            {
                return;
            }

            UIDocumentCraftScreen uIDocumentCraftScreen = FindAnyObjectByType<UIDocumentCraftScreen>(FindObjectsInactive.Include);

            if (CraftingUtils.IsItemAnIngredientOfCurrentLearnedRecipes(uIDocumentCraftScreen, item))
            {
                CraftingRecipe[] resultingRecipes = CraftingUtils.GetRecipesUsingItem(uIDocumentCraftScreen, item).ToArray();
                if (resultingRecipes != null && resultingRecipes.Length > 0)
                {
                    root.Q<Label>("IngredientDescription").text = CraftingUtils.GetFormattedTextForRecipesUsingItem(resultingRecipes);
                    root.Q<Label>("IngredientDescription").style.display = DisplayStyle.Flex;
                }
            }
        }
    }
}

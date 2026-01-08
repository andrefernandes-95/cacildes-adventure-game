using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public static class CraftingUtils
    {
        public static int GetCraftingMaterialAmountInInventory(InventoryDatabase inventoryDatabase, Item ingredient)
        {
            return inventoryDatabase
                .ownedCraftingMaterials
                .Sum(craftingMaterial => craftingMaterial != null && craftingMaterial.EqualsTo(ingredient) ? 1 : 0);

        }

        public static bool CanCraftItem(InventoryDatabase inventoryDatabase, CraftingRecipe recipe)
        {
            bool hasEnoughMaterial = true;

            foreach (var ingredient in recipe.ingredients)
            {
                if (GetCraftingMaterialAmountInInventory(inventoryDatabase, ingredient.ingredient) >= ingredient.amount)
                {
                    hasEnoughMaterial = true;
                }
                else
                {
                    hasEnoughMaterial = false;
                    break;
                }
            }

            return hasEnoughMaterial;
        }

        public static bool CanImproveItem(CharacterBaseManager characterBaseManager, UpgradableItem upgradableItem, int ownedGold)
        {
            UpgradeMaterialData.UpgradeMaterialEntry upgradeData = upgradableItem.upgradeMaterialData.upgradeMaterials.ElementAtOrDefault(upgradableItem.level);

            if (upgradeData == null)
            {
                return false;
            }

            if (characterBaseManager.characterBaseInventory.GetUpgradeMaterialAmount(upgradeData.upgradeMaterial) < upgradeData.amount)
            {
                return false;
            }

            if (ownedGold < upgradeData.goldCostForUpgrade)
            {
                return false;
            }

            return true;
        }

        public static void UpgradeItem(
            UpgradableItem upgradableItem,
            UnityAction<int> onUpgrade,
            UnityAction<KeyValuePair<UpgradeMaterial, int>> onUpgradeMaterialUsed
        )
        {
            var currentWeaponLevel = upgradableItem.level;

            UpgradeMaterialData.UpgradeMaterialEntry upgradeData = upgradableItem.upgradeMaterialData.upgradeMaterials.ElementAtOrDefault(currentWeaponLevel);

            onUpgrade(upgradeData.goldCostForUpgrade);

            onUpgradeMaterialUsed(new KeyValuePair<UpgradeMaterial, int>(upgradeData.upgradeMaterial, upgradeData.amount));

            upgradableItem.level++;
        }

        public static bool IsItemAnIngredientOfCurrentLearnedRecipes(UIDocumentCraftScreen uIDocumentCraftScreen, Item item)
        {
            if (uIDocumentCraftScreen.availableRecipes.Count == 0)
            {
                return false;
            }

            foreach (var recipe in uIDocumentCraftScreen.availableRecipes)
            {
                if (recipe.ingredients.Exists(craftingIngredientEntry =>
                    craftingIngredientEntry != null
                    && craftingIngredientEntry.ingredient != null
                    && craftingIngredientEntry.ingredient.EqualsTo(item)))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<CraftingRecipe> GetRecipesUsingItem(UIDocumentCraftScreen uIDocumentCraftScreen, Item item)
        {
            List<CraftingRecipe> recipesUsingItem = new List<CraftingRecipe>();

            foreach (var recipe in uIDocumentCraftScreen.availableRecipes)
            {
                if (recipe.ingredients.Exists(craftingIngredientEntry =>
                    craftingIngredientEntry != null
                    && craftingIngredientEntry.ingredient != null
                    && craftingIngredientEntry.ingredient.EqualsTo(item)))
                {
                    recipesUsingItem.Add(recipe);
                }
            }

            return recipesUsingItem;
        }

        public static string GetFormattedTextForRecipesUsingItem(CraftingRecipe[] resultingRecipes)
        {
            string text = LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Use to prepare:");

            for (int i = 0; i < resultingRecipes.Length; i++)
            {
                text += "- " + resultingRecipes[i].resultingItem.GetName() + $" ({resultingRecipes[i].resultingAmount})";
                if (i < resultingRecipes.Length - 1)
                {
                    text += "\n";
                }
            }

            return text;
        }
    }
}

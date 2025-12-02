namespace AF
{
    using UnityEngine;

    public class UpgradableItem : Item
    {
        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;
        public UpgradeMaterialData upgradeMaterialData;

        public virtual float GetBonusStep(int level)
        {
            if (level <= 3)
            {
                return 5;
            }
            else if (level <= 6)
            {
                return 10;
            }
            else if (level <= 9)
            {
                return 15;
            }
            else
            {
                return 20;
            }
        }

        public virtual float GetElementalBonusStep(int level)
        {
            if (level <= 3)
            {
                return 3;
            }
            else if (level <= 6)
            {
                return 6;
            }
            else if (level <= 9)
            {
                return 9;
            }
            else
            {
                return 12;
            }
        }

        public int GetBonusAttackPerLevel(int level, bool isElementalDamage)
        {
            if (level == 0)
            {
                return 0;
            }

            float total = 0;

            for (int i = 1; i <= level; i++)
            {
                total += isElementalDamage ? GetElementalBonusStep(i) : GetBonusStep(i);
            }

            return Mathf.RoundToInt(total);
        }

        public int GetBonusPoisePerLevel(int initialPoise, int level)
        {
            if (initialPoise == 0)
            {
                return 0;
            }

            if (level == 0)
            {
                return initialPoise;
            }

            float total = initialPoise;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += .15f;
                else if (i <= 6)
                    total += .25f;
                else if (i <= 9)
                    total += 0.5f;
                else
                    total += .75f;
            }

            return Mathf.CeilToInt(total);
        }

        public int GetBonusPosturePerLevel(int initialPosture, int level)
        {
            if (initialPosture == 0)
            {
                return 0;
            }

            if (level == 0)
            {
                return initialPosture;
            }

            float total = initialPosture;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += .5f;
                else if (i <= 6)
                    total += 1f;
                else if (i <= 9)
                    total += 1.5f;
                else
                    total += 2f;
            }

            return Mathf.CeilToInt(total);
        }

        public int GetBonusStatusEffectAmountPerHitPerLevel(float initialAmountPerHit, int level)
        {
            if (initialAmountPerHit <= 0)
            {
                return 0;
            }

            if (level == 0)
            {
                return Mathf.CeilToInt(initialAmountPerHit);
            }

            float total = initialAmountPerHit;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += 0.75f;
                else if (i <= 6)
                    total += 1.25f;
                else if (i <= 9)
                    total += 1.75f;
                else
                    total += 3f;
            }

            return Mathf.CeilToInt(total);
        }


        public bool CanBeUpgradedFurther()
        {
            return canBeUpgraded && upgradeMaterialData != null && upgradeMaterialData.upgradeMaterials.Length > 0 && this.level <= upgradeMaterialData.upgradeMaterials.Length - 1;
        }

        public string GetMaterialCostForNextLevel(CharacterBaseManager characterBaseManager)
        {
            if (CanBeUpgradedFurther() && upgradeMaterialData != null && upgradeMaterialData.upgradeMaterials[this.level] != null)
            {
                int nextLevel = this.level + 1;
                string text = Utils.IsPortuguese() ? $"<size=80%>Itens necessários para melhorar arma para nível +{nextLevel}:" : $"<size=80%>Required items to upgrade weapon to level +{nextLevel}:";
                text += "\n";
                text += "<size=100%>";

                UpgradeMaterialData.UpgradeMaterialEntry upgradeData = upgradeMaterialData.upgradeMaterials[this.level];

                if (upgradeData != null)
                {
                    int amountOwned = characterBaseManager.characterBaseInventory.GetCraftingMaterialAmount(upgradeData.upgradeMaterial);
                    text += $"x{upgradeData.amount} {upgradeData.upgradeMaterial.GetName()} ";
                    text += Utils.IsPortuguese() ? $"(Possuis {amountOwned})" : $"(You Own {amountOwned})";
                    text += "\n";
                }

                return text;
            }

            return "";
        }


    }
}

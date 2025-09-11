using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseWeight : MonoBehaviour
    {
        [SerializeField] CharacterBaseManager characterBaseManager;

        public int GetMaximumCarryingWeight()
        {
            return characterBaseManager.combatant.maximumCarryingWeight
                + ScalingUtils.GetBonusWeightCarriedPerLevel(characterBaseManager.characterBaseStats.GetEndurance(), characterBaseManager.characterBaseStats.GetStrength())
                + characterBaseManager.statsBonusController.bonusWeightLoad;
        }

        public bool ShouldHeavyroll() => characterBaseManager.statsBonusController.weightPenalty >= GetMaximumCarryingWeight();
        public bool ShouldMidroll() => characterBaseManager.statsBonusController.weightPenalty >= GetMaximumCarryingWeight() * 2 / 3;

        public bool WillHeavyroll(int nextWeightPenalty) => nextWeightPenalty >= GetMaximumCarryingWeight();
        public bool WillMidroll(int nextWeightPenalty) => nextWeightPenalty >= GetMaximumCarryingWeight() * 2 / 3;

    }
}

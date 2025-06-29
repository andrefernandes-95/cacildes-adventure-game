using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseWeight : MonoBehaviour
    {
        [SerializeField] CharacterBaseManager characterBaseManager;

        public int currentCarryingWeight = 0;

        public int GetMaximumCarryingWeight()
        {
            return characterBaseManager.combatant.maximumCarryingWeight;
        }

        public bool ShouldHeavyroll() => currentCarryingWeight >= GetMaximumCarryingWeight();
        public bool ShouldMidroll() => currentCarryingWeight >= GetMaximumCarryingWeight() * 2 / 3;
    }
}

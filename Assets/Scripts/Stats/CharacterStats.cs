namespace AF
{
    using UnityEngine;

    public class CharacterStats : CharacterBaseStats
    {
        [SerializeField] CharacterManager characterManager;

        public override int GetVitality()
        {
            return characterManager.combatant.vitality + characterManager.statsBonusController.GetCurrentVitalityBonus();
        }

        public override int GetEndurance()
        {
            return characterManager.combatant.endurance + characterManager.statsBonusController.GetCurrentEnduranceBonus();
        }

        public override int GetIntelligence()
        {
            return characterManager.combatant.intelligence + characterManager.statsBonusController.GetCurrentIntelligenceBonus();
        }

        public override int GetStrength()
        {
            return characterManager.combatant.strength + characterManager.statsBonusController.GetCurrentStrengthBonus();
        }

        public override int GetDexterity()
        {
            return characterManager.combatant.dexterity + characterManager.statsBonusController.GetCurrentDexterityBonus();
        }

        public override int GetReputation()
        {
            return characterManager.combatant.reputation + characterManager.statsBonusController.GetCurrentReputationBonus();
        }

        public override int GetCurrentLevel()
        {
            return characterManager.combatant.GetCurrentLevel();
        }

        public override void ResetStats()
        {
        }
    }
}

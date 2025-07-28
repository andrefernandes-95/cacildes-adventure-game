namespace AF
{
    using UnityEngine;

    public class CharacterStats : CharacterBaseStats
    {
        [SerializeField] CharacterManager characterManager;

        public override int GetVitality()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.vitality + characterManager.statsBonusController.GetCurrentVitalityBonus();
        }

        public override int GetEndurance()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.endurance + characterManager.statsBonusController.GetCurrentEnduranceBonus();
        }

        public override int GetIntelligence()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.intelligence + characterManager.statsBonusController.GetCurrentIntelligenceBonus();
        }

        public override int GetStrength()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.strength + characterManager.statsBonusController.GetCurrentStrengthBonus();
        }

        public override int GetDexterity()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.dexterity + characterManager.statsBonusController.GetCurrentDexterityBonus();
        }

        public override int GetReputation()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.reputation + characterManager.statsBonusController.GetCurrentReputationBonus();
        }

        public override int GetCurrentLevel()
        {
            if (characterManager.combatant == null)
            {
                return 1;
            }

            return characterManager.combatant.GetCurrentLevel();
        }

        public override void ResetStats()
        {
        }
    }
}

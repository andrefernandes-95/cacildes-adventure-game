using UnityEngine;

namespace AF
{
    public class PlayerLevelManager : MonoBehaviour
    {
        public PlayerStatsDatabase playerStatsDatabase;

        [SerializeField] PlayerManager playerManager;

        public int GetCurrentLevel()
        {
            return playerStatsDatabase.GetCurrentLevel();
        }
        public float GetRequiredExperienceForNextLevel()
        {
            return LevelUtils.GetRequiredExperienceForLevel(this.GetCurrentLevel() + 1);
        }

        public void LevelUp(int desiredVitality, int desiredEndurance, int desiredStrength, int desiredDexterity, int desiredIntelligence, int virtualGold)
        {
            playerStatsDatabase.vitality = desiredVitality;
            playerStatsDatabase.endurance = desiredEndurance;
            playerStatsDatabase.strength = desiredStrength;
            playerStatsDatabase.dexterity = desiredDexterity;
            playerStatsDatabase.intelligence = desiredIntelligence;

            playerStatsDatabase.gold = virtualGold;

            OnLevelChanged();
        }

        public void IncreaseVitality(int bonus)
        {
            playerStatsDatabase.vitality += bonus;
            OnLevelChanged();
        }

        public void IncreaseEndurance(int bonus)
        {
            playerStatsDatabase.endurance += bonus;
            OnLevelChanged();
        }

        public void IncreaseStrength(int bonus)
        {
            playerStatsDatabase.strength += bonus;
            OnLevelChanged();
        }

        public void IncreaseDexterity(int bonus)
        {
            playerStatsDatabase.dexterity += bonus;
            OnLevelChanged();
        }

        public void IncreaseIntelligence(int bonus)
        {
            playerStatsDatabase.intelligence += bonus;
            OnLevelChanged();
        }

        void OnLevelChanged()
        {
            // On Levelling Up, we must make sure we recalculate the current damages
            playerManager.characterBaseAttackManager.CalculateCurrentDamage();
        }
    }
}

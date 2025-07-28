using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPosture : CharacterAbstractPosture
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public PlayerManager playerManager;

        public float POSTURE_DECREASE_RATE_BONUS = 2.25f;

        public void ResetPosture()
        {
            currentPostureDamage = 0;
        }

        public override float GetPostureDecreateRate()
        {
            return POSTURE_DECREASE_RATE_BONUS + playerManager.statsBonusController.postureDecreaseRateBonus;
        }

        public override bool CanPlayPostureDamagedEvent()
        {
            return playerManager.thirdPersonController.isSwimming == false;
        }
    }
}

using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerPoise : CharacterAbstractPoise
    {
        public PlayerStatsDatabase playerStatsDatabase;
        public StatsBonusController statsBonusController;

        public PlayerManager playerManager;

        public override void ResetStates()
        {
        }


        public override bool CanCallPoiseDamagedEvent()
        {
            return playerManager.thirdPersonController.isSwimming == false;
        }

        public override void PlayHitReaction()
        {
        }
    }
}

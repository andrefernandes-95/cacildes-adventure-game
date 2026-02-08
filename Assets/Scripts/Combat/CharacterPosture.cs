using AF.Events;
using TigerForge;

namespace AF
{
    public class CharacterPosture : CharacterAbstractPosture
    {
        public int maxPostureDamage = 100;
        int defaultMaxPostureDamage;

        public GameSession gameSession;

        private void Awake()
        {
            defaultMaxPostureDamage = characterBaseManager.combatant != null ? characterBaseManager.combatant.maximumPosture : maxPostureDamage;
        }

        private void Start()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                maxPostureDamage = defaultMaxPostureDamage;
            });
        }

        public override bool CanPlayPostureDamagedEvent()
        {
            return characterBaseManager.characterBaseDamageReceiver.isBackstabbed == false;
        }

        public override int GetMaxPostureDamage()
        {
            int basePostureDamage = Utils.ScaleWithCurrentNewGameIteration(base.GetMaxPostureDamage(), gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);

            return (int)(basePostureDamage * GetCumulativeMultiplier());
        }

        public override float GetPostureDecreateRate()
        {
            return 1f;
        }

        public void ResetPosture()
        {
            this.currentPostureDamage = 0;
        }
        public override bool TakePostureDamage(int extraPostureDamage)
        {
            bool hasBrokenPosture = base.TakePostureDamage(extraPostureDamage);

            // If taking posture damage, increase the max posture
            if (hasBrokenPosture)
            {
                maxPostureDamage += (int)(maxPostureDamage / 2);
                IncreaseCumulativePosture();
            }

            return hasBrokenPosture;
        }
    }
}

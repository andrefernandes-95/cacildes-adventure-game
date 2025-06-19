namespace AF
{
    using AF.Health;
    using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        public Damage damage;

        public abstract void OnPrepare(CharacterManager characterManager);
        public abstract void OnPrepare(PlayerManager characterManager);
        public abstract void OnUse(CharacterManager characterManager);
        public abstract void OnUse(PlayerManager characterManager);

        public void ApplyDamageScaling(PlayerManager playerManager)
        {
            damage.ScaleWithStats(
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence());
        }
    }
}

namespace AF
{
    using AF.Health;
    using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        public Damage damage;

        [Header("Combo")]
        public Ability next;
        public float chanceToCombo = 0.5f;

        public abstract void OnPrepare(CharacterManager characterManager);
        public abstract void OnPrepare(PlayerManager characterManager);
        public abstract void OnUse(CharacterManager characterManager);
        public abstract void OnUse(PlayerManager characterManager);

        public void ApplyDamageScaling(PlayerManager playerManager)
        {
            damage.ScaleWithStats(
                playerManager.playerStats.GetStrength(),
                playerManager.playerStats.GetDexterity(),
                playerManager.playerStats.GetIntelligence());
        }

        public void ApplyDamageScaling(CharacterManager characterManager)
        {
        }

        public abstract bool CanUseAbility(CharacterBaseManager character);

        public abstract Damage GetDamage(CharacterBaseManager attacker);
    }
}

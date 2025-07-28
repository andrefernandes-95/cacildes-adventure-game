namespace AF.StatusEffects
{
    [System.Serializable]
    public class AppliedStatusEffect
    {
        public StatusEffect statusEffect;

        public bool hasReachedTotalAmount;

        public float currentAmount;
    }

    [System.Serializable]
    public class StatusEffectState
    {
        public float currentAmount;
        public bool hasReachedTotalAmount;
    }
}

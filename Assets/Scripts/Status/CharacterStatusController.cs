namespace AF
{
    using System.Collections.Generic;
    using AF.StatusEffects;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;

    public class CharacterStatusController : StatusController
    {
        [SerializeField] SerializedDictionary<StatusEffect, StatusEffectState> activeEffects = new();

        public override SerializedDictionary<StatusEffect, StatusEffectState> GetActiveEffects()
        {
            return activeEffects;
        }
    }
}

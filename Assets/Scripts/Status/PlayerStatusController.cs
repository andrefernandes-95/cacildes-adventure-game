using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace AF.StatusEffects
{

    public class PlayerStatusController : StatusController
    {

        public StatusDatabase statusDatabase;

        public override SerializedDictionary<StatusEffect, StatusEffectState> GetActiveEffects()
        {
            return statusDatabase.activeEffects;
        }
    }
}

using UnityEngine;

namespace AF
{
    public abstract class ConsumableEffect : ScriptableObject
    {
        public abstract void OnStart(CharacterBaseManager characterBaseManager);

        public abstract void OnUse(CharacterBaseManager characterBaseManager);

        public abstract void OnEnd(CharacterBaseManager characterBaseManager);
    }
}

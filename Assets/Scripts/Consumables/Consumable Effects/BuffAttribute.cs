using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    public abstract class BuffAttribute : DrinkableConsumableEffect
    {
        public new LocalizedString name;
        public Sprite icon;
        public Color barColor;

        [Header("Options")]
        public float durationInSeconds = 60;

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            base.OnUse(characterBaseManager);

            characterBaseManager.characterBaseBuffManager.StartBuffAttribute(this);
        }

        public abstract void OnAppliedStart(CharacterBaseManager characterBaseManager);

        public abstract void OnAppliedUpdate(CharacterBaseManager characterBaseManager);

        public abstract void OnAppliedEnd(CharacterBaseManager characterBaseManager);
    }
}

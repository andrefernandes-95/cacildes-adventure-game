using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class PlayerAppearanceManager : CharacterBaseAppearance
    {

        [Header("Male Parts")]
        [SerializeField] List<string> maleTorso = new();
        [SerializeField] List<string> maleHands = new();
        [SerializeField] List<string> maleLegs = new();

        [Header("Female Parts")]
        [SerializeField] List<string> femaleHead = new();
        [SerializeField] List<string> femaleTorso = new();
        [SerializeField] List<string> femaleHands = new();
        [SerializeField] List<string> femaleLegs = new();

        [Header("Components")]
        [SerializeField] GameSettings gameSettings;
        [SerializeField] SyntyCharacterModelManager syntyCharacterModelManager;

        [Header("Default Apperance")]
        public string defaultHair = "Chr_Hair_23";
        public string defaultFace = "_Chr_Head_Male_00";

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_CHARACTER_CUSTOMIZED, syntyCharacterModelManager.UpdateAvatar);
        }

        public override List<string> GetBeard()
        {
            return new List<string>() { gameSettings.beard };
        }

        public override List<string> GetEyebrows()
        {
            return new List<string>() { gameSettings.eyebrows };
        }

        public override List<string> GetHairs()
        {
            if (string.IsNullOrEmpty(gameSettings.hair))
            {
                return new List<string>() { defaultHair };
            }

            return new List<string>() { gameSettings.hair };
        }

        public override List<string> GetFace()
        {
            if (string.IsNullOrEmpty(gameSettings.face))
            {
                return new List<string>() { defaultFace };
            }

            return new List<string>() { gameSettings.face };
        }

        public override List<string> GetHands()
        {
            if (gameSettings.isMale)
            {
                return maleHands;
            }

            return femaleHands;
        }

        public override List<string> GetLegs()
        {
            if (gameSettings.isMale)
            {
                return maleLegs;
            }

            return femaleLegs;
        }

        public override List<string> GetTorso()
        {
            if (gameSettings.isMale)
            {
                return maleTorso;
            }

            return femaleTorso;
        }

        public override bool IsMale()
        {
            return gameSettings.isMale;
        }

        public override Color GetHairColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.hairColor, out var hairColor))
            {
                return hairColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.defaultHairColor, out var defaultHairColor);
            return defaultHairColor;
        }

        public override Color GetEyesColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.eyeColor, out var eyeColor))
            {
                return eyeColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.defaultEyeColor, out var defaultEyeColor);
            return defaultEyeColor;
        }

        public override Color GetSkinColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.skinColor, out var skinColor))
            {
                return skinColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.defaultSkinColor, out var defaultSkinColor);
            return defaultSkinColor;
        }

        public override Color GetTattooColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.tattooColor, out var tattooColor))
            {
                return tattooColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.defaultTattooColor, out var defaultTattooColor);
            return defaultTattooColor;
        }

    }
}

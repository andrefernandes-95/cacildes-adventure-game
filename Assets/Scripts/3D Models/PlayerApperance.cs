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
        [SerializeField] List<string> femaleTorso = new();
        [SerializeField] List<string> femaleHands = new();
        [SerializeField] List<string> femaleLegs = new();

        [Header("Components")]
        [SerializeField] GameSettings gameSettings;
        [SerializeField] GameSession gameSession;
        [SerializeField] SyntyCharacterModelManager syntyCharacterModelManager;
        [SerializeField] PlayerManager playerManager;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_CHARACTER_CUSTOMIZED, syntyCharacterModelManager.UpdateAvatar);
            EventManager.StartListening(EventMessages.ON_BODY_TYPE_CHANGED, () =>
            {
                if (gameSession.gameState == GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN)
                {
                    syntyCharacterModelManager.UpdateAvatar();

                    Armor tmpArmor = playerManager.characterBaseEquipment.GetEquippedArmor();

                    playerManager.characterBaseEquipment.UnequipArmor();

                    syntyCharacterModelManager.ToggleTorso(true);

                    if (tmpArmor != null)
                    {
                        playerManager.characterBaseEquipment.EquipArmor(tmpArmor);
                    }
                }
            });
        }

        public override List<string> GetBeard()
        {
            if (string.IsNullOrEmpty(gameSettings.GetBeard()))
            {
                return new List<string>() { gameSettings.GetDefaultBeard() };
            }

            return new List<string>() { gameSettings.GetBeard() };
        }

        public override List<string> GetEyebrows()
        {
            if (string.IsNullOrEmpty(gameSettings.GetEyebrows()))
            {
                return new List<string>() { gameSettings.GetDefaultEyebrows() };
            }

            return new List<string>() { gameSettings.GetEyebrows() };
        }

        public override List<string> GetHairs()
        {
            if (string.IsNullOrEmpty(gameSettings.GetHair()))
            {
                return new List<string>() { gameSettings.GetDefaultHair() };
            }

            return new List<string>() { gameSettings.GetHair() };
        }

        public override List<string> GetFace()
        {
            if (string.IsNullOrEmpty(gameSettings.GetFace()))
            {
                return new List<string>() { gameSettings.GetDefaultFace() };
            }

            return new List<string>() { gameSettings.GetFace() };
        }

        public override List<string> GetHands()
        {
            if (IsMale())
            {
                return maleHands;
            }

            return femaleHands;
        }

        public override List<string> GetLegs()
        {
            if (IsMale())
            {
                return maleLegs;
            }

            return femaleLegs;
        }

        public override List<string> GetTorso()
        {
            if (IsMale())
            {
                return maleTorso;
            }

            return femaleTorso;
        }

        public override bool IsMale()
        {
            return gameSettings.IsMale();
        }

        public override Color GetHairColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.GetHairColor(), out var hairColor))
            {
                return hairColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.GetDefaultHairColor(), out var defaultHairColor);
            return defaultHairColor;
        }

        public override Color GetEyesColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.GetEyeColor(), out var eyeColor))
            {
                return eyeColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.GetDefaultEyeColor(), out var defaultEyeColor);
            return defaultEyeColor;
        }

        public override Color GetSkinColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.GetSkinColor(), out var skinColor))
            {
                return skinColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.GetDefaultSkinColor(), out var defaultSkinColor);
            return defaultSkinColor;
        }

        public override Color GetTattooColor()
        {
            if (ColorUtility.TryParseHtmlString(gameSettings.GetTattooColor(), out var tattooColor))
            {
                return tattooColor;
            }

            ColorUtility.TryParseHtmlString(gameSettings.GetDefaultTattooColor(), out var defaultTattooColor);
            return defaultTattooColor;
        }
    }
}

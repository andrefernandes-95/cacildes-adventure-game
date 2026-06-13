namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Game", menuName = "System/New Game", order = 0)]
    public class Game : ScriptableObject
    {
        [Header("Player Settings")]
        [Header("Player Apperance")]
        public string defaultPlayerName = "Cacildes";
        public string playerName = "Cacildes";
        public string defaultHairColor = "#4F412D";
        public string defaultSkinColor = "#FFCCAE";
        public string defaultEyeColor = "#000000";
        public string defaultTattooColor = "#874FA5";
        public string defaultHair = "Chr_Hair_23";
        public string defaultEyebrows = "";
        public string defaultBeard = "";
        public string defaultFace = "_Chr_Head_Male_00";
        public string hairColor = "#4F412D";
        public string skinColor = "#FFCCAE";
        public string eyeColor = "#000000";
        public string tattooColor = "#874FA5";
        public string hair = "";
        public string eyebrows = "";
        public string beard = "";
        public string face = "";
        public bool isMale = true;
        public int playerPortrait = 0;

        [Header("World Settings")]
        [SerializeField] bool useDayAndNightCycle = true;

        [Header("Title Screen")]
        public AudioClip titleScreenMusic;

        public bool UseDayAndNightCycle() => useDayAndNightCycle;

    }
}

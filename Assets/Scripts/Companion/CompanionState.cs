using UnityEngine;

namespace AF.Companions
{
    [System.Serializable]
    public class SerializedCompanionState
    {
        public string companionId;

        public bool isWaitingForPlayer = false;
        public string sceneNameWhereCompanionsIsWaitingForPlayer;
        public Vector3 waitingPosition;

        // For items, we store the file paths to the items
        public string[] rightWeapons = new string[3];
        public string[] leftWeapons = new string[3];
        public string[] spells = new string[10];
        public string helmet;
        public string armor;
        public string gauntlet;
        public string legwear;
        public string[] accessories = new string[4];
    }

    [System.Serializable]
    public class CompanionState
    {
        public bool isWaitingForPlayer = false;
        public string sceneNameWhereCompanionsIsWaitingForPlayer;
        public Vector3 waitingPosition;
        public Weapon[] rightWeapons = new Weapon[3];
        public Weapon[] leftWeapons = new Weapon[3];
        public Spell[] spells = new Spell[10];
        public Helmet helmet;
        public Armor armor;
        public Gauntlet gauntlet;
        public Legwear legwear;
        public Accessory[] accessories = new Accessory[4];
    }
}

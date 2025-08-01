using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Bonfire Site", menuName = "Data/New Bonfire Site", order = 0)]
    public class BonfireSite : ScriptableObject
    {
        [Header("Bonfire Name")]
        public string englishName;
        public string portugueseName;

        public Sprite image;

        [Header("Teleport Settings")]
        public SceneLocation sceneLocation;
        public SpawnLocationData spawnLocationData;

        [Header("Settings")]
        public bool isUnlockable = true;
        public bool canFastTravel = true;

        public string GetName()
        {
            if (Utils.IsPortuguese())
            {
                return portugueseName;
            }

            return englishName;
        }
    }
}

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
        public string sceneName;
        public string spawnGameObjectNameRef = "Bonfire Spawnref";
    }
}

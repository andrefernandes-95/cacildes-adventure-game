namespace AF.ModTools
{

    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(fileName = "Object Mod Asset", menuName = "Modding / New Object Mod Asset", order = 0)]
    public class ObjectModAsset : ModAsset
    {
        public override string GetName()
        {
            if (customDisplayName.IsEmpty && prefab != null)
            {
                return prefab.name;
            }

            return name;
        }

        public override string GetResourcePath()
        {
            return "Modding/Assets/Objects/" + name;
        }
    }
}

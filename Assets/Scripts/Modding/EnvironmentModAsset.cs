namespace AF.ModTools
{

    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(fileName = "Mod Asset", menuName = "Modding / New Environment Mod Asset", order = 0)]
    public class EnvironmentModAsset : ModAsset
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
            return "Modding/Assets/Environment/" + name;
        }
    }
}

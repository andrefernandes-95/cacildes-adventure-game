using EditorAttributes;
using UnityEngine;
using UnityEngine.Localization;

namespace AF.Loading
{
    [CreateAssetMenu(menuName = "System / New Loading Screen")]

    public class LoadingScreen : ScriptableObject
    {
        [AssetPreview]
        public Sprite image;

        [Header("Text")]
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string enText;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string ptText;

        [Header("Settings")]
        public string[] mapNames;

        public string GetDisplayText()
        {
            return Utils.IsPortuguese() ? ptText : enText;
        }
    }
}

namespace AF
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.TextCore.Text;

    [CreateAssetMenu(fileName = "Font Database", menuName = "System/New Font Database", order = 0)]
    public class FontDatabase : ScriptableObject
    {
        public Font defaultFont;
        public Font japaneseFont;
    }
}

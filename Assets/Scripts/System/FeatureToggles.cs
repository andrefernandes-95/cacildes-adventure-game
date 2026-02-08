using UnityEngine;

[CreateAssetMenu(fileName = "Feature Toggles", menuName = "System/New Feature Toggles", order = 0)]
public class FeatureToggles : ScriptableObject
{
    /// <summary>
    /// Use Unity UI, replaces old UI Toolkit
    /// </summary>
    public bool useExperimentalUI = false;

}

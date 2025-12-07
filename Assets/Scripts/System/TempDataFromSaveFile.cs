using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Temp Data From Save File", menuName = "System/New Temp Data From Save File", order = 0)]
public class TempDataFromSaveFile : ScriptableObject
{
    public bool loadSavedPlayerPositionAndRotation = false;
    public Vector3 savedPlayerPosition;
    public Quaternion savedPlayerRotation;

    public Cavern cavernFromSaveFile;

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif

    void Clear()
    {
        loadSavedPlayerPositionAndRotation = false;
        cavernFromSaveFile = null;
    }
}

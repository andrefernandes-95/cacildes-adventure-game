using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace AF
{

    [CreateAssetMenu(fileName = "Quests Database", menuName = "System/New Quests Database", order = 0)]
    public class QuestsDatabase : ScriptableObject
    {

#if UNITY_EDITOR 

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
        }
    }
}

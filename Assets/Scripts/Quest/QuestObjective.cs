using System;
using System.Linq;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest Objective")]

    public class QuestObjective : ScriptableObject
    {
        public QuestParent questParent;

        [AssetPreview]
        public Sprite objectiveImage;

        public LocalizedString objectiveDescription;

        [TextArea] public string objectiveDescription_English;
        [TextArea] public string objectiveDescription_Portuguese;

        public SceneLocation location;
        public BonfireSite closestBonfire;

        public string GetDescription()
        {
            if (objectiveDescription != null && objectiveDescription.IsEmpty == false)
            {
                return objectiveDescription.GetLocalizedString();
            }

            if (Utils.IsPortuguese())
            {
                return objectiveDescription_Portuguese;
            }

            return objectiveDescription_English;
        }
    }
}

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

        [TextArea] public string objectiveDescription_English;
        [TextArea] public string objectiveDescription_Portuguese;

        public SceneLocation location;
        public BonfireSite closestBonfire;

        public string GetDescription()
        {
            if (Utils.IsPortuguese())
            {
                return objectiveDescription_Portuguese;
            }

            return objectiveDescription_English;
        }
    }
}

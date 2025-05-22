using System;
using System.Linq;
using AF.Events;
using GameAnalyticsSDK;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [System.Serializable]
    public class QuestObjectiveInfo
    {
        public Sprite objectiveImage;
        public LocalizedString objectiveDescription;
        public LocalizedString location;
        public Sprite locationImage;
        public LocalizedString closestBonfire;

    }
}

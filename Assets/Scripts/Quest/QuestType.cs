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
    [CreateAssetMenu(menuName = "Data / New Quest Type")]

    public class QuestType : ScriptableObject
    {
        public LocalizedString questType;
    }
}

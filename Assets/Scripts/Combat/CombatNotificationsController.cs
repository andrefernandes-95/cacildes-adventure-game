using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class CombatNotificationsController : MonoBehaviour
    {
        public Transform notificationRootTransform;
        public List<CombatNotificationEntry> combatNotificationEntries = new();
        CombatNotificationManager _combatNotificationManager;

        public float yOffset = .25f;

        CombatNotificationManager GetCombatNotificationManager()
        {
            if (_combatNotificationManager == null)
            {
                _combatNotificationManager = FindAnyObjectByType<CombatNotificationManager>(FindObjectsInactive.Include);
            }

            return _combatNotificationManager;
        }

        public void AddNotification(string text, Color color)
        {
            // First, check if combatNotificationEntries contains inactive game objects, which essentialy means we need to clean up the list
            combatNotificationEntries = combatNotificationEntries.Where(x => x != null && x.transform != null && x.isActiveAndEnabled).ToList();

            // Update vertical position of existing elements 
            combatNotificationEntries.ForEach(element =>
            {
                element.transform.position = new Vector3(element.transform.position.x, element.transform.position.y + yOffset, element.transform.position.z);
            });

            var instance = GetCombatNotificationManager()?.GetInstance();
            if (instance == null)
            {
                return;
            }

            instance.transform.position = notificationRootTransform.transform.position;
            instance.transform.SetParent(notificationRootTransform);
            instance.currentDuration = 0f;
            instance.textMeshPro.color = color;
            instance.textMeshPro.text = text;

            combatNotificationEntries.Add(instance);
        }

        public void ShowDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().damage);
        }
        public void ShowFireDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().fireDamage);
        }
        public void ShowFrostDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().frostDamage);
        }
        public void ShowLightningDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().lightningDamage);
        }
        public void ShowDarknessDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().darknessDamage);
        }
        public void ShowWaterDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().waterDamage);
        }
        public void ShowMagicDamage(int amount)
        {
            AddNotification("- " + amount, GetCombatNotificationManager().magicDamage);
        }
        public void ShowStatusFullAmountEffect(string displayedStatusEffectName, Color statusEffectColor)
        {
            AddNotification(displayedStatusEffectName.ToLower(), statusEffectColor);
        }
        public void ShowStatusEffectAmount(string statusEffect, float amount, Color statusEffectColor)
        {
            if (amount <= 0)
            {
                return;
            }

            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from") + " " + statusEffect.ToLower(), statusEffectColor);
        }
        public void ShowCritical(float amount)
        {
            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from critical attack"), GetCombatNotificationManager().criticalDamage);
        }
        public void ShowBackstab(float amount)
        {
            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from backstab attack"), GetCombatNotificationManager().criticalDamage);
        }
        public void ShowGuardCounter(float amount)
        {
            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from guard counter attack"), GetCombatNotificationManager().criticalDamage);
        }
        public void ShowRageCounter(float amount)
        {
            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from rage buildup attack"), GetCombatNotificationManager().criticalDamage);
        }
        public void ShowPostureBroken(float amount)
        {
            AddNotification("- " + amount + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "from broken posture"), GetCombatNotificationManager().criticalDamage);
        }
        public void ShowHealthRestored(int amount)
        {
            AddNotification("+ " + amount, GetCombatNotificationManager().healthRestored);
        }

    }

}

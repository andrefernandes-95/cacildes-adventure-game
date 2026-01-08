using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

namespace AF
{

    public static class AnalyticsUtils
    {
        private static void Track(string eventName, Dictionary<string, object> data)
        {
            if (AnalyticsConsentManager.Instance == null ||
                !AnalyticsConsentManager.Instance.CanTrack())
                return;

            var ev = new CustomEvent(eventName);

            foreach (var pair in data)
            {
                ev.Add(pair.Key, pair.Value);
            }

            AnalyticsService.Instance.RecordEvent(ev);
        }

        public static void OnEnemyKilled(CharacterBaseManager enemy)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
            {
                return;
            }

            CharacterBaseManager killer = enemy.GetTarget();
            string enemyName = enemy?.combatant?.name ?? "Unknown";
            bool isBoss = false;

            if (enemy is CharacterManager enemyManager && enemyManager.characterBossController != null && enemyManager.characterBossController.IsBoss())
            {
                isBoss = true;
                enemyName = enemyManager.characterBossController.bossName;
            }

            PlayerManager player = killer as PlayerManager;

            var rightWeapon = killer?.characterBaseWeaponsManager?.GetCurrentRightWeapon();
            string rightWeaponInfo = rightWeapon != null
                ? $"{rightWeapon.name.Replace("(Clone)", "")} +{rightWeapon.level}"
                : "";

            if (player != null)
            {
                if (isBoss)
                {
                    // Analytics
                    Track("bossKilled", new Dictionary<string, object>
                        {
                            { "enemyName", enemyName },
                            { "playerLevel", (int)player.playerStats.GetCurrentLevel()},
                            { "weapon", rightWeaponInfo },
                        });

                    // Discord Message
                    string weaponText = "with 🗡️ " + (string.IsNullOrEmpty(rightWeaponInfo) ? "bare hands" : rightWeaponInfo);
                    string message = $"👑 Boss defeated: {enemyName} - Slayed with {weaponText}";

                    player.discordNotifier.SendToDiscord(message);
                }
                else
                {
                    if (!isBoss)
                    {
                        Track("enemyKilled", new Dictionary<string, object>
                        {
                            { "enemyName", enemyName },
                            { "playerLevel", (int)player.playerStats.GetCurrentLevel()},
                            { "weapon", rightWeaponInfo },
                            { "scene", SceneManager.GetActiveScene().name }
                        });
                    }
                }
            }
        }

        public static void OnPlayerKilled(CharacterBaseManager enemy, PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string enemyText = enemy != null ? enemy.combatant.name : "the environment";

            if (enemy is CharacterManager enemyManager &&
                enemyManager.characterBossController != null &&
                enemyManager.characterBossController.IsBoss())
            {
                enemyText = enemyManager.characterBossController.bossName;
            }

            string locationText = $"🗺️ {SceneManager.GetActiveScene().name}";

            Track("playerKilled", new Dictionary<string, object>
            {
                { "enemyName", enemy == null ? "Environment" : enemyText },
                { "playerLevel", (int)playerManager.playerStats.GetCurrentLevel()},
                { "scene", SceneManager.GetActiveScene().name }
            });

            // Send to Discord
            string message = $"🪦 Cacildes was defeated by {enemyText} in {locationText}.";
            playerManager.discordNotifier.SendToDiscord(message);
        }

        public static void OnItemUpgrade(UpgradableItem upgradableItem, PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string itemName = upgradableItem.name.Replace("(Clone)", "");

            Track("itemUpgraded", new Dictionary<string, object>
            {
                { "itemName", itemName },
                { "itemLevelAfterUpgrade", upgradableItem.level },
            });
        }

        public static void OnItemCrafted(Item itemCrafted, PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string itemName = itemCrafted.name.Replace("(Clone)", "");
            Track("itemCrafted", new Dictionary<string, object>
            {
                { "itemName", itemName },
            });
        }

        public static void OnArenaWon(PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
            {
                return;
            }

            Track("arenaWon", new Dictionary<string, object>
            {
                { "playerLevel", (int)playerManager.playerStats.GetCurrentLevel() },
                { "scene", SceneManager.GetActiveScene().name }
            });

            // Send Discord Message
            playerManager.discordNotifier.SendToDiscord(
                $"🏟️ Cacildes has conquered all rounds of the arena in **{SceneManager.GetActiveScene().name}**! 🏆 Well fought, champion!"
            );
        }

        public static void OnUnlockBonfire(string bonfireName, PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
            {
                return;
            }

            Track("bonfireUnlocked", new Dictionary<string, object>
            {
                { "bonfire", bonfireName },
                { "playerLevel", (int)playerManager.playerStats.GetCurrentLevel() }
            });

            playerManager.discordNotifier.SendToDiscord(
                $"🔥 Bonfire **{bonfireName}** has been lit!"
            );
        }

        public static void OnBeginNewGame(DiscordNotifier discordNotifier)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
            {
                return;
            }

            Track("beginNewGame", new Dictionary<string, object>
            {
            });

            discordNotifier.SendToDiscord(
                $"🎒 Somewhere, a new adventure begins!"
            );
        }

        public static void OnBeginNewGamePlus(PlayerManager playerManager)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
            {
                return;
            }

            Track("beginNewGamePlus", new Dictionary<string, object>
            {
            });

            playerManager.discordNotifier.SendToDiscord(
                $"🚀 Somewhere, a new game+ has begun!"
            );
        }

        public static void OnGithubVisit()
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            Track("externalLinkOpened", new Dictionary<string, object>
            {
                { "urlDestination", "github" }
            });
        }

        public static void OnBandcampVisit()
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            Track("externalLinkOpened", new Dictionary<string, object>
            {
                { "urlDestination", "bandcamp" }
            });
        }

        public static void OnDiscordVisit()
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            Track("externalLinkOpened", new Dictionary<string, object>
            {
                { "urlDestination", "discord" }
            });
        }

        public static void OnCompanionJoinParty(DiscordNotifier discordNotifier, string companionName)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            Track("companionJoins", new Dictionary<string, object>
            {
                { "companion", companionName }
            });

            // Send Discord Message
            discordNotifier.SendToDiscord(
                $"🤝 **{companionName}** has joined a party!"
            );
        }

        public static void OnCompanionLeaveParty(DiscordNotifier discordNotifier, string companionName)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            Track("companionLeaves", new Dictionary<string, object>
            {
                { "companion", companionName }
            });

            discordNotifier.SendToDiscord(
                $"🤝 **{companionName}** has left a party!"
            );
        }

        public static void OnQuestStarted(PlayerManager playerManager, QuestParent questParent)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string questName = questParent.name;

            Track("questStarted", new Dictionary<string, object>
            {
                { "quest", questName }
            });
        }

        public static void OnQuestCompleted(PlayerManager playerManager, QuestParent questParent)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string questName = questParent.name;

            Track("questCompleted", new Dictionary<string, object>
            {
                { "quest", questName }
            });

            playerManager.discordNotifier.SendToDiscord(
                $"📜 Quest {questName} was completed!"
            );
        }

        public static void OnQuestObjectiveCompleted(PlayerManager playerManager, QuestObjective questObjective)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack())
                return;

            string questObjectiveName = questObjective.name;

            Track("questObjectiveCompleted", new Dictionary<string, object>
            {
                { "questObjective", questObjectiveName }
            });

            playerManager.discordNotifier.SendToDiscord(
                $"✅ Quest objective {questObjectiveName} was completed!"
            );
        }

        public static void OnItemFound(PlayerManager playerManager, Item item, string itemType)
        {
            if (AnalyticsConsentManager.Instance == null || !AnalyticsConsentManager.Instance.CanTrack() || item == null)
                return;

            string itemName = item.name.Replace("(Clone)", "");

            Track("itemFound", new Dictionary<string, object>
            {
                { "itemName", itemName },
                { "itemType", itemType },
                { "scene", SceneManager.GetActiveScene().name },
            });
        }
    }
}

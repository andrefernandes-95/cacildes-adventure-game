using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AYellowpaper.SerializedCollections;
using CI.QuickSave;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace AF.Companions
{
    [CreateAssetMenu(fileName = "Companions Database", menuName = "System/New Companions Database", order = 0)]
    public class CompanionsDatabase : ScriptableObject
    {
        [SerializedDictionary("Companion ID", "Companion State")]
        public SerializedDictionary<string, CompanionState> companionsInParty = new();

        [Header("Settings")]
        public float companionToPlayerStoppingDistance = 2f;
        public float maxDistanceToPlayerBeforeTeleportingNear = 15f;

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

        public void AddToParty(CharacterManager characterManager)
        {
            AddToParty(characterManager.GetCharacterID());
        }

        public void AddToParty(string companionId)
        {
            if (companionsInParty.ContainsKey(companionId))
            {
                Debug.Log($"Trying to add companion with id: {companionId} to party, but companion already exists.");
                return;
            }

            companionsInParty.Add(companionId, new()
            {
                isWaitingForPlayer = false,
                sceneNameWhereCompanionsIsWaitingForPlayer = "",
                waitingPosition = Vector3.zero
            });

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public void RemoveFromParty(CharacterManager characterManager)
        {
            RemoveFromParty(characterManager.GetCharacterID());
        }

        public void RemoveFromParty(string companionId)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                Debug.Log($"Trying to remove companion with id: {companionId} to party, but couldn't not find him in party.");
                return;
            }

            companionsInParty.Remove(companionId);

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public bool IsInParty(string companionId)
        {
            return companionsInParty.ContainsKey(companionId);
        }

        public bool IsCompanionWaiting(string companionId)
        {
            if (!IsInParty(companionId))
            {
                return false;
            }

            return companionsInParty[companionId].isWaitingForPlayer;
        }

        public CompanionState GetWaitState(string companionId)
        {
            return companionsInParty[companionId];
        }

        public bool IsCompanionAndIsActivelyInParty(string companionId)
        {
            if (companionId == "Minion")
            {
                return true;
            }

            return IsInParty(companionId) && !IsCompanionWaiting(companionId);
        }

        public void WaitForPlayer(string companionId, CompanionState newCompanionState)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                return;
            }

            companionsInParty[companionId] = newCompanionState;

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public void FollowPlayer(string companionId)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                return;
            }

            companionsInParty[companionId] = new()
            {
                isWaitingForPlayer = false,
                sceneNameWhereCompanionsIsWaitingForPlayer = "",
                waitingPosition = Vector3.zero
            };

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public void Clear()
        {
            companionsInParty.Clear();
        }

        public Dictionary<string, CompanionState> GetActiveCompanins()
        {
            return companionsInParty.Where(x => x.Value.isWaitingForPlayer == false)
                                    .ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<string, CompanionState> GetWaitingCompanions()
        {
            return companionsInParty.Where(x => x.Value.isWaitingForPlayer)
                                    .ToDictionary(x => x.Key, x => x.Value);
        }

        public bool TryGetCompanionCount(out int count)
        {
            count = companionsInParty.Where(x => !x.Value.isWaitingForPlayer).Count();
            return count > 0;
        }

        public void SaveCompanionStates(QuickSaveWriter quickSaveWriter)
        {
            List<SerializedCompanionState> serializedCompanionStates = new();

            foreach (KeyValuePair<string, CompanionState> companionInParty in companionsInParty)
            {
                SerializedCompanionState serializedCompanionState = new();
                serializedCompanionState.companionId = companionInParty.Key;

                CompanionState state = companionInParty.Value;
                serializedCompanionState.isWaitingForPlayer = state.isWaitingForPlayer;
                serializedCompanionState.sceneNameWhereCompanionsIsWaitingForPlayer = state.sceneNameWhereCompanionsIsWaitingForPlayer;
                serializedCompanionState.waitingPosition = state.waitingPosition;

                List<string> rightWeaponsToSave = new();
                foreach (Weapon wp in state.rightWeapons)
                {
                    string weaponPath = Utils.GetItemPath(wp).Replace("(Clone)", "");
                    rightWeaponsToSave.Add(weaponPath);
                }
                serializedCompanionState.rightWeapons = rightWeaponsToSave.ToArray();

                List<string> leftWeaponsToSave = new();
                foreach (Weapon wp in state.leftWeapons)
                {
                    string weaponPath = Utils.GetItemPath(wp).Replace("(Clone)", "");
                    leftWeaponsToSave.Add(weaponPath);
                }
                serializedCompanionState.leftWeapons = leftWeaponsToSave.ToArray();

                List<string> spellsToSave = new();
                foreach (Spell spell in state.spells)
                {
                    string spellPath = Utils.GetItemPath(spell).Replace("(Clone)", "");
                    spellsToSave.Add(spellPath);
                }
                serializedCompanionState.spells = spellsToSave.ToArray();

                List<string> accessoriesToSave = new();
                foreach (Accessory acc in state.accessories)
                {
                    string accessoryPath = Utils.GetItemPath(acc).Replace("(Clone)", "");
                    accessoriesToSave.Add(accessoryPath);
                }
                serializedCompanionState.accessories = accessoriesToSave.ToArray();

                string helmetPath = Utils.GetItemPath(state.helmet).Replace("(Clone)", "");
                serializedCompanionState.helmet = helmetPath;
                string armorPath = Utils.GetItemPath(state.armor).Replace("(Clone)", "");
                serializedCompanionState.armor = armorPath;
                string gauntletPath = Utils.GetItemPath(state.gauntlet).Replace("(Clone)", "");
                serializedCompanionState.gauntlet = gauntletPath;
                string legwearPath = Utils.GetItemPath(state.legwear).Replace("(Clone)", "");
                serializedCompanionState.legwear = legwearPath;

                serializedCompanionStates.Add(serializedCompanionState);
            }

            quickSaveWriter.Write("companionsInParty", serializedCompanionStates);
        }

        public void LoadCompanionStates(QuickSaveReader quickSaveReader)
        {
            companionsInParty.Clear();

            quickSaveReader.TryRead("companionsInParty", out List<SerializedCompanionState> savedCompanionsInParty);

            if (savedCompanionsInParty != null && savedCompanionsInParty.Count > 0)
            {
                for (int idx = 0; idx < savedCompanionsInParty.Count; idx++)
                {
                    var savedState = savedCompanionsInParty.ElementAt(idx);

                    CompanionState newState = new();
                    newState.isWaitingForPlayer = savedState.isWaitingForPlayer;
                    newState.sceneNameWhereCompanionsIsWaitingForPlayer = savedState.sceneNameWhereCompanionsIsWaitingForPlayer;
                    newState.waitingPosition = savedState.waitingPosition;

                    for (int i = 0; i < savedState.rightWeapons.Length - 1; i++)
                    {
                        Weapon weapon = Resources.Load<Weapon>(savedState.rightWeapons[i]);
                        if (weapon != null)
                        {
                            newState.rightWeapons[i] = Instantiate(weapon);
                        }
                    }

                    for (int i = 0; i < savedState.leftWeapons.Length - 1; i++)
                    {
                        Weapon weapon = Resources.Load<Weapon>(savedState.leftWeapons[i]);
                        if (weapon != null)
                        {
                            newState.leftWeapons[i] = Instantiate(weapon);
                        }
                    }

                    for (int i = 0; i < savedState.spells.Length - 1; i++)
                    {
                        Spell spell = Resources.Load<Spell>(savedState.spells[i]);
                        if (spell != null)
                        {
                            newState.spells[i] = Instantiate(spell);
                        }
                    }

                    for (int i = 0; i < savedState.accessories.Length - 1; i++)
                    {
                        Accessory accessory = Resources.Load<Accessory>(savedState.accessories[i]);
                        if (accessory != null)
                        {
                            newState.accessories[i] = Instantiate(accessory);
                        }
                    }

                    if (!string.IsNullOrEmpty(savedState.helmet))
                    {
                        Helmet helmet = Resources.Load<Helmet>(savedState.helmet);
                        if (helmet != null)
                        {
                            newState.helmet = Instantiate(helmet);
                        }
                    }

                    if (!string.IsNullOrEmpty(savedState.gauntlet))
                    {
                        Gauntlet gauntlet = Resources.Load<Gauntlet>(savedState.gauntlet);
                        if (gauntlet != null)
                        {
                            newState.gauntlet = Instantiate(gauntlet);
                        }
                    }

                    if (!string.IsNullOrEmpty(savedState.armor))
                    {
                        Armor armor = Resources.Load<Armor>(savedState.armor);
                        if (armor != null)
                        {
                            newState.armor = Instantiate(armor);
                        }
                    }

                    if (!string.IsNullOrEmpty(savedState.legwear))
                    {
                        Legwear legwear = Resources.Load<Legwear>(savedState.legwear);
                        if (legwear != null)
                        {
                            newState.legwear = Instantiate(legwear);
                        }
                    }

                    companionsInParty.Add(savedState.companionId, newState);
                }
            }
        }

    }

}

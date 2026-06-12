using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;

namespace AF.Companions
{

    public class CompanionsSceneManager : MonoBehaviour
    {
        [Header("Companion Prefabs")]
        public GameObject[] companionPrefabs;
        public CompanionsDatabase companionsDatabase;

        // Companion Instances
        [HideInInspector] public Dictionary<string, GameObject> companionInstancesInScene = new();

        [Header("Scene References")]
        public PlayerManager playerManager;

        public void SpawnCompanions()
        {
            Evaluate();

            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, Evaluate);
        }

        public void ClearInactiveCompanions()
        {
            Dictionary<string, GameObject> companionInstancesInSceneClone = companionInstancesInScene.ToDictionary(item => item.Key, item => item.Value);

            foreach (var companionInstance in companionInstancesInSceneClone)
            {
                if (!companionsDatabase.companionsInParty.ContainsKey(companionInstance.Key))
                {
                    Destroy(companionInstancesInScene[companionInstance.Key]);
                    companionInstancesInScene.Remove(companionInstance.Key);
                }
            }
        }

        public void HandleWaitingCompanions()
        {
            foreach (var waitingCompanion in companionsDatabase.GetWaitingCompanions())
            {
                if (!companionInstancesInScene.ContainsKey(waitingCompanion.Key))
                {
                    companionInstancesInScene.Add(
                        waitingCompanion.Key,
                        Instantiate(companionPrefabs.First(
                            companionPrefab => companionPrefab.GetComponent<CharacterManager>().GetCharacterID() == waitingCompanion.Key)));
                }

                TeleportCompanion(
                    companionInstancesInScene[waitingCompanion.Key].GetComponent<CharacterManager>(),
                    waitingCompanion.Value.waitingPosition);
            }
        }

        public void HandleActiveCompanions()
        {
            int companionIndex = 0;
            foreach (var activeCompanion in companionsDatabase.GetActiveCompanins())
            {
                companionIndex++;

                if (!companionInstancesInScene.ContainsKey(activeCompanion.Key))
                {
                    companionInstancesInScene.Add(
                        activeCompanion.Key,
                        Instantiate(companionPrefabs.First(
                            companionPrefab => companionPrefab.GetComponent<CharacterManager>().GetCharacterID() == activeCompanion.Key)));
                }

                Vector3 desiredPosition = playerManager.transform.position + (playerManager.transform.forward * companionIndex);
                NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

                TeleportCompanion(
                    companionInstancesInScene[activeCompanion.Key].GetComponent<CharacterManager>(),
                    hit.position != null ? hit.position : desiredPosition);
            }
        }


        public void TeleportCompanion(CharacterManager characterManager, Vector3 spawnPosition)
        {
            if (IsValidPosition(spawnPosition))
            {
                characterManager.characterController.enabled = false;
                characterManager.transform.position = spawnPosition;
                characterManager.characterController.enabled = true;
            }

        }

        private bool IsValidPosition(Vector3 position)
        {
            // Check for Infinity or NaN values
            return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z) &&
                   !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
        }

        public void TeleportCompanionsNearPlayer()
        {
            int companionIndex = 0;
            foreach (var activeCompanion in companionsDatabase.GetActiveCompanins())
            {
                companionIndex++;

                Vector3 desiredPosition = playerManager.transform.position + (playerManager.transform.forward * -1f * companionIndex);
                NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

                TeleportCompanion(
                    companionInstancesInScene[activeCompanion.Key].GetComponent<CharacterManager>(),
                    hit.position != null ? hit.position : desiredPosition);
            }
        }


        void Evaluate()
        {
            ClearInactiveCompanions();

            HandleWaitingCompanions();

            HandleActiveCompanions();
        }

        public CharacterManager GetCompanionInScene(string companionID)
        {
            if (companionInstancesInScene.ContainsKey(companionID))
            {
                return companionInstancesInScene[companionID].GetComponent<CharacterManager>();
            }
            return null;
        }


        public void GiveLootToCompanions(List<Item> items)
        {

            foreach (var companion in playerManager.companionsDatabase.companionsInParty)
            {
                CharacterManager companionInScene = playerManager.companionsSceneManager.GetCompanionInScene(companion.Key);
                if (companionInScene == null)
                {
                    continue;
                }

                foreach (Item item in items)
                {
                    bool isRightHand = Random.Range(0, 1f) > 0.5;
                    if (item is Weapon weapon)
                    {
                        // If bow, always equip on left hand weapon
                        if (weapon.damage.weaponAttackType == WeaponAttackType.Range)
                        {
                            if (companionInScene.characterBaseAttackManager.CompareWeapon(weapon, false) > 0)
                            {
                                companionInScene.characterBaseEquipment.EquipWeapon(Instantiate(weapon), 0, false);
                            }
                        }
                        else if (companionInScene.characterBaseAttackManager.CompareWeapon(weapon, isRightHand) > 0)
                        {
                            companionInScene.characterBaseEquipment.EquipWeapon(Instantiate(weapon), 0, isRightHand);
                        }
                    }
                    else if (item is Helmet helmet && companionInScene.characterBaseDefenseManager.CompareArmorPiece(helmet).comparison > 0)
                    {
                        companionInScene.characterBaseEquipment.EquipHelmet(Instantiate(helmet));
                    }
                    else if (item is Armor armor && companionInScene.characterBaseDefenseManager.CompareArmorPiece(armor).comparison > 0)
                    {
                        companionInScene.characterBaseEquipment.EquipArmor(Instantiate(armor));
                    }
                    else if (item is Gauntlet gauntlet && companionInScene.characterBaseDefenseManager.CompareArmorPiece(gauntlet).comparison > 0)
                    {
                        companionInScene.characterBaseEquipment.EquipGauntlets(Instantiate(gauntlet));
                    }
                    else if (item is Legwear legwear && companionInScene.characterBaseDefenseManager.CompareArmorPiece(legwear).comparison > 0)
                    {
                        companionInScene.characterBaseEquipment.EquipLegwear(Instantiate(legwear));
                    }
                }
            }
        }
    }
}

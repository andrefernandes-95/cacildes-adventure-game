namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class EnemyRenderManager : MonoBehaviour
    {
        List<CharacterManager> characterManagers = new();
        [SerializeField] GameObject enemyGroupRoot;

        private void Awake()
        {
            characterManagers = Utils.CollectComponentsFromGameObject<CharacterManager>(enemyGroupRoot).ToList();

            HandleActivation(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) HandleActivation(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) HandleActivation(false);
        }

        void HandleActivation(bool isActive)
        {

            if (characterManagers != null && characterManagers.Count > 0)
            {
                foreach (CharacterManager c in characterManagers)
                {
                    c.gameObject.SetActive(isActive);
                }
            }
        }
    }
}

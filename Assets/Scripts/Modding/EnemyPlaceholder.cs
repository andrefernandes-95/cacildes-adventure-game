namespace AF
{
    using AF.ModTools;
    using UnityEngine;

    public class EnemyPlaceholder : MonoBehaviour
    {
        ModManager modManager;
        GameObject instatiatedEnemy;

        public CharacterManager enemyToSpawn;

        void Awake()
        {
            modManager = FindAnyObjectByType<ModManager>(FindObjectsInactive.Include);

            modManager.onPlayModeEnter.AddListener(OnPlayModeEnter);
            modManager.onEditModeEnter.AddListener(OnEditModeEnter);
        }

        void OnPlayModeEnter()
        {
            if (instatiatedEnemy != null)
            {
                Destroy(instatiatedEnemy.gameObject);
            }

            instatiatedEnemy = Instantiate(enemyToSpawn.gameObject, transform.position, Quaternion.identity);

            gameObject.SetActive(false);
        }

        void OnEditModeEnter()
        {
            if (instatiatedEnemy != null)
            {
                Destroy(instatiatedEnemy.gameObject);
            }

            gameObject.SetActive(true);
        }
    }
}

namespace AF
{
    using AF.Flags;
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class EnemyDoor : MonoBehaviour
    {
        [SerializeField] MonoBehaviourID monoBehaviourID;
        [SerializeField] FlagsDatabase flagsDatabase;

        [SerializeField] CharacterManager[] enemies;

        AudioSource audioSource => GetComponent<AudioSource>();

        void Awake()
        {
            if (flagsDatabase.ContainsFlag(monoBehaviourID.ID))
            {
                Utils.UpdateTransformChildren(this.transform, false);
            }
            else
            {
                foreach (CharacterManager enemy in enemies)
                {
                    if (enemy != null)
                    {
                        enemy.health.onDeath.AddListener(CheckIfDoorShouldOpen);
                    }
                }
            }
        }

        void CheckIfDoorShouldOpen()
        {
            bool allEnemiesDead = false;
            foreach (CharacterManager enemy in enemies)
            {
                allEnemiesDead = enemy.health.GetCurrentHealth() <= 0;
                if (!allEnemiesDead) break;
            }

            if (allEnemiesDead)
            {
                Utils.UpdateTransformChildren(this.transform, false);
                flagsDatabase.AddFlag(monoBehaviourID);
                audioSource.Play();
            }
        }
    }
}
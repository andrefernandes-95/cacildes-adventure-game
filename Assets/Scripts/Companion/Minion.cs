using UnityEngine;

namespace AF
{
    public class Minion : MonoBehaviour
    {
        void Awake()
        {
            var player = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);

            if (player != null)
            {
                transform.position = player.transform.position + player.transform.forward;
            }
        }
    }
}

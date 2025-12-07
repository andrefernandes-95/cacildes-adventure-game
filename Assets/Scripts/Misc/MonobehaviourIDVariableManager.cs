using AF.Flags;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Misc
{
    /// <summary>
    /// A variation of VariableManager but it handles its counter through monobehaviour ids
    /// </summary>
    public class MonoBehaviourIDVariableManager : MonoBehaviour
    {
        public MonoBehaviourID[] monoBehaviourIDs;

        [SerializeField] FlagsDatabase flagsDatabase;

        [Header("Notifications")]
        public NotificationManager notificationManager;
        public Sprite notificationSprite;
        public string notificationSuffixTextEn;
        public string notificationSuffixTextPt;

        [Header("Sounds")]
        [SerializeField] AudioClip notificationSound;
        [SerializeField] Soundbank soundbank;

        [Header("Events")]
        [SerializeField] UnityEvent onAllBehaviourIDCompleted;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowCurrentCounter()
        {
            string notificationSuffixText = Utils.IsPortuguese() ? notificationSuffixTextPt : notificationSuffixTextEn;

            notificationManager.ShowNotification($"{GetCounter()}/{monoBehaviourIDs.Length}{notificationSuffixText}", notificationSprite);

            if (notificationSound != null)
            {
                soundbank.PlaySound(notificationSound);
            }

            if (GetCounter() >= monoBehaviourIDs.Length)
            {
                onAllBehaviourIDCompleted?.Invoke();
            }
        }

        int GetCounter()
        {
            int sum = 0;
            foreach (MonoBehaviourID monoBehaviourID in monoBehaviourIDs)
            {
                if (flagsDatabase.ContainsFlag(monoBehaviourID.ID))
                {
                    sum++;
                }
            }
            return sum;
        }
    }
}

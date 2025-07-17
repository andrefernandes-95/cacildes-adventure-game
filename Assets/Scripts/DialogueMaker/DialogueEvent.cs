using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class DialogueEvent : MonoBehaviour
    {
        public string eventId;
        public UnityEvent eventToInvoke;
        [TextArea] public string comment;

        public void Execute()
        {
            eventToInvoke.Invoke();
        }
    }

}

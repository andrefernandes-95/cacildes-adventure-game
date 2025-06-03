using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AF.Triggers
{
    [Preserve]
    public class OnMonobehaviourEvents : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onPlayerLeaveBonfire;

        private void Awake()
        {
            onAwake?.Invoke();

            Debug.Log("OnMonobehaviourEvents - " + gameObject.name + " has run Awake()");

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                onPlayerLeaveBonfire?.Invoke();
            });
        }
        private void Start()
        {
            onStart?.Invoke();
        }
        private void OnEnable()
        {
            onEnable?.Invoke();
        }

    }
}

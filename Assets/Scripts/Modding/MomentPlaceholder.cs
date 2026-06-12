namespace AF
{
    using System.Collections.Generic;
    using AF.Events;
    using AF.ModTools;
    using UnityEngine;

    [RequireComponent(typeof(Moment))]
    [RequireComponent(typeof(SphereCollider))]
    public class MomentPlaceholder : MonoBehaviour
    {
        ModManager modManager;
        public List<ModEvent> modEvents = new();

        Moment moment => GetComponent<Moment>();
        MeshRenderer meshRenderer => GetComponent<MeshRenderer>();

        bool isInPlayMode = false;

        [SerializeField] GameObject child;

        void Awake()
        {
            modManager = FindAnyObjectByType<ModManager>(FindObjectsInactive.Include);

            modManager.onPlayModeEnter.AddListener(OnPlayModeEnter);
            modManager.onEditModeEnter.AddListener(OnEditModeEnter);

            modEvents = new List<ModEvent>();
        }

        void OnPlayModeEnter()
        {
            isInPlayMode = true;
            meshRenderer.enabled = false;

            RebuildMoment();
        }

        void OnEditModeEnter()
        {
            isInPlayMode = false;
            meshRenderer.enabled = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && isInPlayMode)
            {
                moment.Trigger();
            }
        }

        public void RebuildMoment()
        {
            // Destroy
            foreach (EventBase c in child.GetComponents<EventBase>())
            {
                Destroy(c);
            }

            foreach (ModEvent m in modEvents)
            {
                m.BuildEvent(child);
            }
        }
    }
}

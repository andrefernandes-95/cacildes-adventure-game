namespace AF
{
    using UnityEngine;

    public class TeleportGenericTrigger : GenericTrigger
    {
        [SerializeField] SceneLocation teleportTo;
        [SerializeField] SpawnLocationData spawnAt;

        TeleportManager _teleportManager;

        private void Awake()
        {
            onActivate.AddListener(OnActivate);
        }

        public void OnActivate()
        {
            Teleport();

            // Disable prompt after beginning teleport to avoid user re-interacting with it
            DisableCapturable();
        }


        public override string GetAction()
        {
            if (Utils.IsPortuguese())
            {
                return $"Viajar para {teleportTo.GetName()}";
            }

            return $"Go to {teleportTo.GetName()}";
        }


        public void Teleport()
        {
            GetTeleportManager().Teleport(teleportTo, spawnAt);
        }

        TeleportManager GetTeleportManager()
        {
            if (_teleportManager == null)
            {
                _teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);
            }

            return _teleportManager;
        }
    }
}

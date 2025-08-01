using System;
using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        [Header("Teleport Settings")]
        [Obsolete] public string sceneName;
        [Obsolete] public string spawnGameObjectName;


        public SceneLocation sceneToTeleportTo;
        public SpawnLocationData spawnAt;

        // Scene Refs
        TeleportManager teleportManager;

        public override IEnumerator Dispatch()
        {
            yield return null;
            Teleport();
        }

        public void Teleport()
        {
            GetTeleportManager().Teleport(sceneToTeleportTo, spawnAt);
        }

        TeleportManager GetTeleportManager()
        {
            if (teleportManager == null)
            {
                teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);
            }

            return teleportManager;
        }
    }
}

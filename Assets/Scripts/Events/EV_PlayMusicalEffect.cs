using System.Collections;
using AF.Music;
using UnityEngine;

namespace AF
{

    public class EV_PlayMusicalEffect : EventBase
    {
        [Header("Components")]
        BGMManager _bgmManager;

        public AudioClip musicalEffect;

        public override IEnumerator Dispatch()
        {
            yield return null;
            GetBGMManager().PlayMusicalEffect(musicalEffect);
        }

        BGMManager GetBGMManager()
        {
            if (_bgmManager == null)
            {
                _bgmManager = FindAnyObjectByType<BGMManager>(FindObjectsInactive.Include);
            }

            return _bgmManager;
        }
    }
}

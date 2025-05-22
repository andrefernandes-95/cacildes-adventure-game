using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Journal))]
    public class EV_ReadBook : EventBase
    {
        Journal journal => GetComponent<Journal>();

        bool hasFinishedReading = false;

        private void Awake()
        {
            journal.onRead_End.AddListener(OnEndRead);
        }

        public override IEnumerator Dispatch()
        {
            hasFinishedReading = false;
            yield return null;

            journal.Read();

            yield return new WaitUntil(() => hasFinishedReading == true);
        }

        void OnEndRead()
        {
            hasFinishedReading = true;
        }
    }
}

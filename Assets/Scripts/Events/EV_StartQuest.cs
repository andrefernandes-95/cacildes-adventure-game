using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_StartQuest : EventBase
    {
        [SerializeField] QuestParent quest;
        [SerializeField] bool trackQuest = false;

        public override IEnumerator Dispatch()
        {
            if (!quest.hasStarted)
            {
                quest.StartQuest();

                if (trackQuest)
                {
                    quest.TrackQuest();
                }
            }

            yield return null;
        }
    }
}

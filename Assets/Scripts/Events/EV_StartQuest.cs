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
            if (!quest.HasStarted())
            {
                quest.SetProgress(0);

                if (trackQuest)
                {
                    quest.Track();
                }
            }

            yield return null;
        }
    }
}

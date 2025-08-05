using System;
using System.Collections;

namespace AF
{
    [Obsolete]
    public class EV_ProgressQuest : EventBase
    {
        public QuestParent questParent;
        public int questProgress = 0;

        public override IEnumerator Dispatch()
        {
            yield return null;
        }

    }

}

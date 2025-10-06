using System;
using System.Collections;

namespace AF
{

    public class EV_AutoSave : EventBase
    {
        public SaveManager saveManager;

        public override IEnumerator Dispatch()
        {
            saveManager.SaveGameData(null);
            yield return null;
        }
    }

}

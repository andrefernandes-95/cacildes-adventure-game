namespace AF
{
    using UnityEngine;

    public abstract class CharacterBaseActivityManager : MonoBehaviour
    {
        public NPCActivity currentActivity;

        public void ResetStates()
        {
            StopCurrentActivity();
        }

        public void StopCurrentActivity()
        {
            if (currentActivity != null)
            {
                currentActivity.OnActivityEnd(GetCharacter());
                currentActivity = null;
            }
        }

        public abstract CharacterBaseManager GetCharacter();

        public void SetActivity(NPCActivity activity)
        {
            StopCurrentActivity();

            this.currentActivity = activity;
            this.currentActivity.OnActivityStart(GetCharacter());
        }
    }
}

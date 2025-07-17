namespace AF
{
    using Ink.Runtime;
    using UnityEngine;

    public class DialogueFunctions : MonoBehaviour
    {
        [SerializeField] QuestParent bearQuest;

        public void BindStoryToFunctions(Story story)
        {
            story.BindExternalFunction("isDoingChickensQuest", () =>
            {
                return bearQuest.questProgress != -1 && bearQuest.IsCompleted() == false;
            });
        }
    }
}
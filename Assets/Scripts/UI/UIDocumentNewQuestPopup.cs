
namespace AF
{
    using AF.Events;
    using AF.Music;
    using DG.Tweening;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentNewQuestPopup : MonoBehaviour
    {
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;

        VisualElement icon;
        Label questTitle;
        Label questDescription;

        [Header("Settings")]
        [SerializeField] float alertDuration = 5f;

        [Header("Audio")]
        [SerializeField] AudioClip questAppearPopupSfx;
        [SerializeField] BGMManager bGMManager;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_QUEST_ADDED, ShowAddedQuestPopup);
        }

        private void Start()
        {
            icon = root.Q<VisualElement>("Icon");
            questTitle = root.Q<Label>("QuestTitle");
            questDescription = root.Q<Label>("QuestDescription");
            root.style.display = DisplayStyle.None;
        }

        void ShowAddedQuestPopup()
        {
            QuestParent questParent = EventManager.GetData(EventMessages.ON_QUEST_ADDED) as QuestParent;
            EventManager.Dispose(EventMessages.ON_QUEST_ADDED);

            questTitle.text = questParent.questName_LocalizedString.GetLocalizedString();

            if (questParent.questObjectiveInfos != null && questParent.questObjectiveInfos.Length > 0)
            {
                questDescription.text = questParent.questObjectiveInfos[0].objectiveDescription.GetLocalizedString();
            }

            root.style.display = DisplayStyle.Flex;
            root.style.opacity = 0f;
            root.transform.scale = new Vector3(0f, 1f, 1f); // Start with width = 0 (collapsed horizontally)

            // Kill any previous tweens
            DOTween.Kill(root);

            Sequence seq = DOTween.Sequence();

            // Fade in and expand horizontally
            seq.Append(DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 1f, 0.2f));
            seq.Join(DOTween.To(() => root.transform.scale.x, x =>
            {
                var s = root.transform.scale;
                s.x = x;
                root.transform.scale = s;
            }, 1.2f, 0.25f)); // Slight overshoot for pop effect

            // Subtle bounce back to full size
            seq.Append(DOTween.To(() => root.transform.scale.x, x =>
            {
                var s = root.transform.scale;
                s.x = x;
                root.transform.scale = s;
            }, 1f, 0.1f));

            PlaySound();

            // Wait before hiding
            seq.AppendInterval(alertDuration);

            // Fade out and collapse again
            seq.Append(DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 0f, 0.3f));
            seq.Join(DOTween.To(() => root.transform.scale.x, x =>
            {
                var s = root.transform.scale;
                s.x = x;
                root.transform.scale = s;
            }, 0f, 0.3f));

            seq.OnComplete(() => root.style.display = DisplayStyle.None);
        }

        void PlaySound()
        {
            if (questAppearPopupSfx == null)
            {
                return;
            }

            bGMManager.PlayMusicalEffect(questAppearPopupSfx);
        }
    }

}

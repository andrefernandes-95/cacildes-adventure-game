using System.Collections;
using System.Linq;
using AF.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewQuestsMenu : ViewMenu
    {
        [Header("Prefabs")]
        public VisualTreeAsset questPrefabButton;
        [SerializeField] VisualTreeAsset questInfoButton;

        [Header("Quests")]
        public QuestsDatabase questsDatabase;

        [Header("Components")]
        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        [Header("Sprites")]
        [SerializeField] Sprite unrevealedObjectiveSprite;

        VisualElement questPreview;

        QuestParent selectedQuest;

        [Header("Audio")]
        [SerializeField] AudioClip trackQuestSound;

        protected override void OnEnable()
        {
            selectedQuest = null;
            base.OnEnable();
            SetupRefs();
            RedrawUI();
            StartCoroutine(FocusOnFirstQuestListButton());
        }

        void SetupRefs()
        {
            questPreview = root?.Q<VisualElement>("QuestPreview");
            if (questPreview != null)
            {
                questPreview.style.opacity = 0;
            }

            UIUtils.SetupButton(root.Q<Button>("ShowObjectives"), () =>
            {
                HideAllTabs();
                DrawObjectivesTab();

            }, soundbank);

            UIUtils.SetupButton(root.Q<Button>("ShowCharacters"), () =>
            {
                HideAllTabs();
                DrawCharactersTab();
            }, soundbank);

            UIUtils.SetupButton(root.Q<Button>("ReturnToQuestList"), () =>
            {
                SetSelectedQuest(null);
                StartCoroutine(FocusOnFirstQuestListButton());
            }, soundbank);

            UIUtils.SetupButton(root.Q<Button>("TrackQuest"), () =>
            {
                OnTryToTrackQuest();
            }, soundbank);

            starterAssetsInputs.onMainMenuUnequipSlot.AddListener(OnTryToTrackQuest);
        }

        void OnDisable()
        {
            starterAssetsInputs.onMainMenuUnequipSlot.RemoveListener(OnTryToTrackQuest);
        }

        void OnTryToTrackQuest()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            if (selectedQuest == null)
            {
                return;
            }

            questsDatabase.SetQuestToTrack(selectedQuest);
            RedrawUI();
            soundbank.PlaySound(trackQuestSound);

            // Force refresh on quest details to show or hide the bookmark icon
            SetSelectedQuest(selectedQuest);
        }

        void RedrawUI()
        {
            DrawQuestsMenu();
        }

        void DrawQuestsMenu()
        {
            var questListScroll = root.Q<ScrollView>("QuestListScroll");
            questListScroll.Clear();

            var reversedQuests = questsDatabase.questsReceived.AsEnumerable().Reverse().ToList();

            for (int i = 0; i < reversedQuests.Count; i++)
            {
                var quest = reversedQuests[i];
                var clone = questPrefabButton.CloneTree();

                clone.Q<Label>("QuestName").text = quest.questName_LocalizedString.GetLocalizedString();
                clone.viewDataKey = quest.name;

                int questIndex = i;
                UIUtils.SetupButton(
                    clone.Q<Button>("QuestButton"),
                    () =>
                    {
                        SetSelectedQuest(quest);
                        questListScroll.ScrollTo(clone);
                    },
                    soundbank
                );

                bool isCompleted = quest.IsCompleted();
                if (isCompleted)
                {
                    clone.style.opacity = 0.5f;
                }

                clone.Q<VisualElement>("CompletedIcon").style.display = isCompleted ? DisplayStyle.Flex : DisplayStyle.None;
                clone.Q<VisualElement>("IncompleteIcon").style.display = !isCompleted ? DisplayStyle.Flex : DisplayStyle.None;
                clone.Q<VisualElement>("TrackIcon").style.display = quest.IsTracked() ? DisplayStyle.Flex : DisplayStyle.None;

                questListScroll.Add(clone);
            }
        }

        void SetSelectedQuest(QuestParent questParent)
        {
            this.selectedQuest = questParent;
            PreviewQuest();
            StartCoroutine(FocusOnFirstObjective());
        }

        IEnumerator FocusOnFirstQuestListButton()
        {
            yield return new WaitForEndOfFrame();
            var scrollView = root.Q<ScrollView>("QuestListScroll");
            if (scrollView.childCount > 0)
            {
                scrollView.ElementAt(0).Q<Button>().Focus();
            }
        }

        IEnumerator FocusOnFirstObjective()
        {
            yield return new WaitForEndOfFrame();
            var objectiveScrollView = root.Q<ScrollView>("QuestObjectivesScrollView");
            if (objectiveScrollView.childCount > 0)
            {
                objectiveScrollView.ElementAt(0).Q<Button>().Focus();
            }
        }

        public void TrackObjective(QuestParent quest)
        {
            questsDatabase.SetQuestToTrack(quest);
            RedrawUI();
        }

        void PreviewQuest()
        {
            if (selectedQuest == null)
            {
                questPreview.style.opacity = 0;

                return;
            }

            root.Q<Label>("QuestType").text = selectedQuest.questType.questType.GetLocalizedString();
            root.Q<Label>("QuestTitle").text = selectedQuest.questName_LocalizedString.GetLocalizedString();
            root.Q<VisualElement>("QuestIcon").style.backgroundImage = new StyleBackground(Utils.Texture2DToSprite(selectedQuest.questIcon as Texture2D));
            root.Q<Toggle>("QuestCompleted").value = selectedQuest.IsCompleted();
            root.Q<Label>("QuestDescription").text = selectedQuest.questDescription.IsEmpty ? "" : selectedQuest.questDescription.GetLocalizedString();
            root.Q<VisualElement>("TrackQuestImage").style.display = selectedQuest.IsTracked() ? DisplayStyle.Flex : DisplayStyle.None;

            var trackQuestLabel = root.Q<Button>("TrackQuest").Q<Label>();
            var trackIcon = root.Q<Button>("TrackQuest").Q<VisualElement>("Track");
            var untrackIcon = root.Q<Button>("TrackQuest").Q<VisualElement>("Untrack");

            trackIcon.style.display = DisplayStyle.None;
            untrackIcon.style.display = DisplayStyle.None;

            if (selectedQuest.IsTracked())
            {
                untrackIcon.style.display = DisplayStyle.Flex;
                trackQuestLabel.text = !Utils.IsPortuguese() ? "Untrack Quest" : "Desafixar Missão";
            }
            else
            {
                trackIcon.style.display = DisplayStyle.Flex;
                trackQuestLabel.text = !Utils.IsPortuguese() ? "Track Quest" : "Afixar Missão";
            }


            HandleTabs();

            questPreview.style.opacity = 1;
        }

        void HandleTabs()
        {
            DrawObjectivesTab();
        }

        void HideAllTabs()
        {
            root.Q<VisualElement>("QuestObjectivesPanel").style.display = DisplayStyle.None;
            root.Q<VisualElement>("Characters").style.display = DisplayStyle.None;

            root.Q<Button>("ShowObjectives").Q<Label>().style.borderBottomWidth = 0;
            root.Q<Button>("ShowCharacters").Q<Label>().style.borderBottomWidth = 0;
        }

        void DrawObjectivesTab()
        {
            HideAllTabs();

            root.Q<VisualElement>("QuestObjectivesPanel").style.display = DisplayStyle.Flex;
            root.Q<Button>("ShowObjectives").Q<Label>().style.borderBottomColor = Color.white;
            root.Q<Button>("ShowObjectives").Q<Label>().style.borderBottomWidth = 2;
            var objectiveScrollView = root.Q<ScrollView>("QuestObjectivesScrollView");
            objectiveScrollView.Clear();

            for (int i = 0; i < selectedQuest.questObjectives.Length; i++)
            {
                DrawObjective(selectedQuest, selectedQuest.questObjectives[i], i, objectiveScrollView);
            }
        }

        void DrawCharactersTab()
        {
            HideAllTabs();

            root.Q<VisualElement>("Characters").style.display = DisplayStyle.Flex;

            root.Q<Button>("ShowCharacters").Q<Label>().style.borderBottomColor = Color.white;
            root.Q<Button>("ShowCharacters").Q<Label>().style.borderBottomWidth = 2;

            ScrollView relatedCharactersScrollView = root.Q<ScrollView>("MissionRelatedCharacters");
            relatedCharactersScrollView.Clear();
            if (selectedQuest.questGiver != null)
            {
                DrawCharacter(selectedQuest.questGiver, relatedCharactersScrollView);
            }

            foreach (Character character in selectedQuest.relatedCharacters)
            {
                DrawCharacter(character, relatedCharactersScrollView);
            }
        }

        public void DrawObjective(QuestParent questParent, string questObjective, int index, ScrollView scrollView)
        {
            bool isCompleted = questParent.IsObjectiveCompleted(questObjective);
            bool isLocked = !isCompleted && index > questParent.questProgress;
            bool isCurrent = index == questParent.questProgress;

            var entry = questInfoButton.CloneTree();
            var descLabel = entry.Q<Label>("Description");

            entry.Q<Label>("Type").text = Utils.IsPortuguese() ? "Objetivo" : "Objective";
            entry.Q<Label>("Label").style.display = DisplayStyle.None;

            descLabel.text = isLocked ? (Utils.IsPortuguese() ? "Objetivo ainda não revelado" : "Unrevelead objective") : questParent.questObjectives_LocalizedString[index].GetLocalizedString();
            descLabel.style.fontSize = isLocked ? 16 : 24;
            descLabel.style.color = Color.white;
            descLabel.style.marginRight = 10;
            descLabel.style.unityFontStyleAndWeight = isCurrent ? FontStyle.Bold : FontStyle.Normal;

            var checkbox = entry.Q<Toggle>("ObjectiveCompletedCheckbox");
            checkbox.value = isCompleted;
            checkbox.style.display = isLocked ? DisplayStyle.None : DisplayStyle.Flex;

            var image = entry.Q<VisualElement>("Image");
            if (isLocked)
            {
                image.style.backgroundImage = new StyleBackground(unrevealedObjectiveSprite);
                image.style.display = DisplayStyle.Flex;
                image.style.width = 120;
                image.style.height = 120;
                entry.style.opacity = 0.5f;
            }
            else
            {
                var info = index < questParent.questObjectiveInfos.Length ? questParent.questObjectiveInfos[index] : null;
                if (info?.objectiveImage != null)
                {
                    image.style.backgroundImage = new StyleBackground(info.objectiveImage);
                    image.style.display = DisplayStyle.Flex;
                }
                else
                {
                    image.style.display = DisplayStyle.None;
                }
            }

            entry.Q<Button>().RegisterCallback<FocusInEvent>(ev => scrollView.ScrollTo(entry));
            scrollView.Add(entry);

            var objectiveInfo = index < questParent.questObjectiveInfos.Length ? questParent.questObjectiveInfos[index] : null;

            if (objectiveInfo != null && !isLocked && !objectiveInfo.location.IsEmpty)
            {
                DrawLocation(objectiveInfo, entry.Q("CardDetails"));
            }
        }

        public void DrawLocation(QuestObjectiveInfo info, VisualElement parent)
        {
            var entry = CreateInfoEntry(
                Utils.IsPortuguese() ? "Localização" : "Location",
                info.location.GetLocalizedString(),
                (Utils.IsPortuguese() ? "Viagem Rápida: " : "Quick Travel: ") + info.closestBonfire.GetLocalizedString(),
                info.locationImage,
                false
            );
            parent.Add(entry);
        }

        public void DrawCharacter(Character character, VisualElement parent)
        {
            var entry = CreateInfoEntry(
                Utils.IsPortuguese() ? "Relacionado com:" : "Related to:",
                character.name_Localized.IsEmpty ? character.name : character.name_Localized.GetLocalizedString(),
                character.biography.IsEmpty ? "" : character.biography.GetLocalizedString(),
                character.avatar,
                false
            );

            entry.style.fontSize = 16;
            VisualElement image = entry.Q("Image");
            image.style.width = 50;
            image.style.height = 50;

            parent.Add(entry);
        }

        VisualElement CreateInfoEntry(string type, string label, string description, Sprite image = null, bool showCheckbox = true)
        {
            var entry = questInfoButton.CloneTree();
            entry.Q<Label>("Type").text = type;
            entry.Q<Label>("Label").text = label;
            entry.Q<Label>("Description").text = description;

            var checkbox = entry.Q<Toggle>("ObjectiveCompletedCheckbox");
            checkbox.style.display = showCheckbox ? DisplayStyle.Flex : DisplayStyle.None;

            var imageElement = entry.Q<VisualElement>("Image");
            if (image != null)
            {
                imageElement.style.backgroundImage = new StyleBackground(image);
                imageElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                imageElement.style.display = DisplayStyle.None;
            }

            entry.Q("QuestInfoButton").style.unityBackgroundImageTintColor = new Color(0, 0, 0, 0.05f);

            return entry;
        }
    }
}

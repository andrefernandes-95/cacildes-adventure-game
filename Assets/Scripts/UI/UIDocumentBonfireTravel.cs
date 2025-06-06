using System.Collections.Generic;
using AF.Bonfires;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireTravel : MonoBehaviour
    {
        public List<BonfireSite> bonfireLocations = new();

        [Header("Components")]
        public Soundbank soundbank;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public TeleportManager teleportManager;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualTreeAsset travelOptionAsset;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;


        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;

        // Internal
        VisualElement root;

        // Last scroll position
        int lastScrollElementIndex = -1;


        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }


        void Close()
        {
            uIDocumentBonfireMenu.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<ScrollView>().Clear();
            root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;

            // The exit button
            var exitOption = travelOptionAsset.CloneTree();
            exitOption.Q<Button>().text = Utils.IsPortuguese() ? "Regressar" : "Return";

            UIUtils.SetupButton(exitOption.Q<Button>(), () =>
            {
                Close();
            },
            () =>
            {
                {
                    root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
                }
            },
            () =>
            {
                root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
            },
            true,
            soundbank);

            root.Q<ScrollView>().Add(exitOption);

            // Add callbacks
            foreach (var location in bonfireLocations)
            {
                if (bonfiresDatabase.unlockedBonfires.Contains(location.name))
                {
                    var clonedBonfireOption = travelOptionAsset.CloneTree();
                    clonedBonfireOption.Q<Button>().text = Utils.IsPortuguese() ? location.portugueseName : location.englishName;

                    UIUtils.SetupButton(clonedBonfireOption.Q<Button>(), () =>
                    {
                        teleportManager.Teleport(location.sceneName, location.spawnGameObjectNameRef);
                    },
                    () =>
                    {
                        {
                            root.Q<IMGUIContainer>("BonfireIcon").style.backgroundImage = new StyleBackground(location.image);
                            root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 1;
                            root.Q<ScrollView>().ScrollTo(clonedBonfireOption);
                        }
                    },
                    () =>
                    {
                        root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
                    },
                    true,
                    soundbank);


                    root.Q<ScrollView>().Add(clonedBonfireOption);
                }

            }

            cursorManager.ShowCursor();

            if (lastScrollElementIndex == -1)
            {
                root.Q<ScrollView>().ScrollTo(exitOption);
            }
            else
            {
                Invoke(nameof(GiveFocus), 0f);
            }
        }
        void GiveFocus()
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>(),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        public string GetBonfireLocalizedNameByBonfireId(string bonfireId)
        {
            var target = bonfireLocations.Find(x => x.name == bonfireId);
            if (target == null)
            {
                return "Unknown Bonfire";
            }

            return Utils.IsPortuguese() ? target.portugueseName : target.englishName;
        }
    }
}

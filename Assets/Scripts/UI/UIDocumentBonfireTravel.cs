using System.Collections.Generic;
using System.Linq;
using AF.Bonfires;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireTravel : MonoBehaviour
    {
        List<BonfireSite> bonfireLocations = new();

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

        bool hasLoadedBonfires = false;

        private void Awake()
        {
            gameObject.SetActive(false);

            TryLoadBonfires();
        }

        void TryLoadBonfires()
        {
            if (!hasLoadedBonfires)
            {
                hasLoadedBonfires = true;
                // Slight overhead, but ensures we don't have to manually include all bonfires
                bonfireLocations = (List<BonfireSite>)Resources.LoadAll<BonfireSite>("Bonfire Sites").ToList().Where(bonfire => bonfire != null && bonfire.canFastTravel);
            }
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
            foreach (var location in GetBonfireLocations())
            {
                if (bonfiresDatabase.unlockedBonfires.Contains(location.name))
                {
                    var clonedBonfireOption = travelOptionAsset.CloneTree();
                    clonedBonfireOption.Q<Button>().text = Utils.IsPortuguese() ? location.portugueseName : location.englishName;

                    UIUtils.SetupButton(clonedBonfireOption.Q<Button>(), () =>
                    {
                        if (location.sceneLocation != null)
                        {
                            teleportManager.Teleport(location.sceneLocation.id, location.spawnLocationData);
                        }
                        else
                        {
                            Debug.LogWarning($"Could not find scene location associated with bonfire {location.name}");
                        }
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

        List<BonfireSite> GetBonfireLocations()
        {
            TryLoadBonfires();
            return bonfireLocations;
        }
    }
}

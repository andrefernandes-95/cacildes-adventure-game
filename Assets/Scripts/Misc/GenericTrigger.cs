using AF.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class GenericTrigger : MonoBehaviour
    {

        [Header("Events")]
        public UnityEvent onActivate;

        [Header("Prompt")]
        public string key = "E";
        public string action = "Pickup";

        // Scene Refs
        UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Alchemy Pickable Info")]
        public Item item;

        [Header("Required Item to Open")]
        public Item requiredItemToOpen;
        public InventoryDatabase inventoryDatabase;

        bool canInteract = true;

        MomentManager _momentManager;
        UIManager _uiManager;
        UIDocumentKeyPrompt _uIDocumentKeyPrompt;
        StarterAssetsInputs _starterAssetsInputs;

        const float MAX_INTERACT_ANGLE = 30f;

        StarterAssetsInputs GetStarterAssetsInputs()
        {
            if (_starterAssetsInputs == null)
            {
                _starterAssetsInputs = FindAnyObjectByType<StarterAssetsInputs>(FindObjectsInactive.Include);
            }

            return _starterAssetsInputs;
        }

        MomentManager GetMomentManager()
        {
            if (_momentManager == null)
            {
                _momentManager = FindAnyObjectByType<MomentManager>(FindObjectsInactive.Include);
            }

            return _momentManager;
        }

        UIManager GetUIManager()
        {
            if (_uiManager == null)
            {
                _uiManager = FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
            }

            return _uiManager;
        }

        UIDocumentKeyPrompt GetUIDocumentKeyPrompt()
        {
            if (_uIDocumentKeyPrompt == null)
            {
                _uIDocumentKeyPrompt = FindAnyObjectByType<UIDocumentKeyPrompt>(FindObjectsInactive.Include);
            }

            return _uIDocumentKeyPrompt;
        }

        bool IsPlayer(Collider other) => other.gameObject.CompareTag("Player");

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            if (CanInteract())
            {
                if (!GetUIDocumentKeyPrompt().isActiveAndEnabled)
                {
                    GetUIDocumentKeyPrompt().DisplayPrompt(key, GetAction(), item);
                }

                if (GetStarterAssetsInputs().interact)
                {
                    GetStarterAssetsInputs().interact = false;

                    HandleActivation();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            DisableKeyPrompt();
        }

        bool CanInteract()
        {
            if (GetUIManager().IsShowingFullScreenGUI() || GetMomentManager().HasMomentOnGoing)
            {
                return false;
            }

            return canInteract;
        }

        public void OnCaptured()
        {
            if (!canInteract)
            {
                return;
            }

        }

        public virtual string GetAction()
        {
            return action;
        }


        public void DisableKeyPrompt()
        {
            GetUIDocumentKeyPrompt().gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void TurnCapturable()
        {
            canInteract = true;
            //this.gameObject.layer = LayerMask.NameToLayer("IEventNavigatorCapturable");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisableCapturable()
        {
            canInteract = false;
            //this.gameObject.layer = 0;
        }

        public void HandleActivation()
        {
            if (!canInteract)
            {
                return;
            }

            DisableKeyPrompt();

            bool canActivate = true;

            if (requiredItemToOpen != null && inventoryDatabase != null)
            {
                if (inventoryDatabase.HasItem(requiredItemToOpen))
                {
                    inventoryDatabase.RemoveItem(requiredItemToOpen);
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.GetName()} " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "was lost with its use."));
                }
                else
                {
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.GetName()} " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "is required to activate."));
                    canActivate = false;
                }
            }

            if (canActivate)
            {
                onActivate?.Invoke();
            }
        }

        NotificationManager GetNotificationManager()
        {
            return FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
        }
    }
}

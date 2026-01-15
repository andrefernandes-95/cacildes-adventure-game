namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EV_AddSpell : EventBase
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] Spell spell;
        [SerializeField] bool autoEquip = false;

        UIDocumentReceivedItemPrompt _uIDocumentReceivedItemPrompt;
        Soundbank _soundbank;

        public override IEnumerator Dispatch()
        {
            // Add items
            Spell addedSpell = playerManager.playerInventory.AddSpell(spell);

            if (autoEquip)
            {
                playerManager.characterBaseEquipment.EquipSpell(addedSpell, 0);
            }

            ShowNotification();

            yield return null;
        }

        void ShowNotification()
        {
            GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
            var combinedList = new List<UIDocumentReceivedItemPrompt.ItemsReceived>();
            UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new()
            {
                itemName = spell.GetName(),
                quantity = 1,
                sprite = spell.sprite
            };
            combinedList.Add(itemReceived);
            GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(combinedList);

            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (_uIDocumentReceivedItemPrompt == null)
            {
                _uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return _uIDocumentReceivedItemPrompt;
        }

        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }

}

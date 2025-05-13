using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class ViewEquipmentMenu : ViewMenu
    {

        [Header("UI Components")]
        public EquipmentSlots equipmentSlots;
        public ItemTooltip itemTooltip;
        public PlayerStatsAndAttributesUI playerStatsAndAttributesUI;
        public ItemList itemList;

        void Awake()
        {
            itemList.onEnabled.AddListener(ShowItemListFooter);
            itemList.onDisabled.AddListener(ShowEquipmentListFooter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            equipmentSlots.gameObject.SetActive(false);
            itemList.gameObject.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            playerStatsAndAttributesUI.gameObject.SetActive(true);

            InitUI();

            ShowEquipmentListFooter();
        }

        void ShowItemListFooter()
        {
            root.Q("EquipmentListFooter").style.display = DisplayStyle.None;
            root.Q("ItemListFooter").style.display = DisplayStyle.Flex;
        }

        void ShowEquipmentListFooter()
        {
            root.Q("EquipmentListFooter").style.display = DisplayStyle.Flex;
            root.Q("ItemListFooter").style.display = DisplayStyle.None;
        }

        private void OnDisable()
        {
            equipmentSlots.gameObject.SetActive(false);
            itemList.gameObject.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            playerStatsAndAttributesUI.gameObject.SetActive(false);

            equipmentSlots.shouldRerender = true;
            itemList.shouldRerender = true;
            itemTooltip.shouldRerender = true;
            playerStatsAndAttributesUI.shouldRerender = true;
        }

        void InitUI()
        {
            equipmentSlots.gameObject.SetActive(true);
            playerStatsAndAttributesUI.DrawStats(null);
        }
    }
}

using System;
using Interface.Inventory.Tooltips;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Interface.Inventory
{
    public class InventoryItem : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Variables

        [SerializeField] private Image image;
        [SerializeField] private Text countText;
        
        public ItemData itemData;
        
        public InventorySlot slot;
        public InventorySlot slotToExchange;
        public InventoryItem itemToExchange;
        public Transform parentAfterDrag;

        public int numberItem = 1;
        public bool isDropped;
        
        private Tooltip _tooltip;

        #endregion

        private void Start()
        {
            if (PlayerManager.LocalInstance != null)
            {
                _tooltip = PlayerManager.LocalInstance.tooltip;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance != null)
            {
                _tooltip = PlayerManager.LocalInstance.tooltip;
            }
        }

        public void Initialise(InventorySlot newSlot, ItemData newItemData)
        {
            slot = newSlot;
            itemData = newItemData;

            slot.itemData = itemData;
            slot.itemInSlot = this;
            
            RefreshCount();
        }

        public void RefreshCount()
        {
            countText.text = numberItem.ToString();
            bool textActive = numberItem > 1;
            countText.gameObject.SetActive(textActive);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (PlayerManager.LocalInstance == null) return;
            
            _tooltip.canShow = false;
            _tooltip.Hide();
            image.raycastTarget = false;
            
            Transform parentTransform = transform.parent;
            parentAfterDrag = parentTransform;
            transform.SetParent(parentTransform.parent.parent.parent);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (PlayerManager.LocalInstance == null) return;
            
            if (isDropped)
            {
                slot.SetItemInHandState(false);
                slot.itemData = null;
                Destroy(gameObject);
            }
            else
            {
                transform.SetParent(parentAfterDrag);

                if (itemToExchange != null)
                {
                    itemToExchange.slot = slotToExchange;
                    (slot, itemToExchange.slot) = (itemToExchange.slot, slot);
                    itemToExchange.transform.SetParent(itemToExchange.parentAfterDrag);
                    itemToExchange = null;
                }
                else
                {
                    slot = slotToExchange;
                }
            }
        
            image.raycastTarget = true;
            _tooltip.canShow = true;
        }
    }
}

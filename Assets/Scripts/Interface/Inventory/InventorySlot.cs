using Interface.Inventory.Tooltips;
using Items;
using Items.BatFeatures;
using Items.GlassesFeatures;
using Items.PhoneFeatures;
using Items.SubjectFeatures;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Key = Items.KeyFeatures.Key;

namespace Interface.Inventory
{
    public class InventorySlot : NetworkBehaviour, IDropHandler
    {
        #region Variables
        
        [SerializeField] private Image image;
        [SerializeField] public Color selectedColor, notSelectedColor;
        [SerializeField] private GameObject[] itemsInHandParent;
        
        public InventoryItem itemInSlot;

        private ItemData _itemData;
        public ItemData itemData
        {
            get => _itemData;
            set
            {
                _itemData = value;
                GetComponent<TooltipSlot>().itemData = value;
            }
        }

        #endregion

        public void Select()
        {
            image.color = selectedColor;
            SetItemInHandState(true);
        }

        public void Deselect()
        {
            image.color = notSelectedColor;
            SetItemInHandState(false);
        }
        
        public void SetItemInHandState(bool active)
        {
            if (itemData != null)
            {
                Debug.Log(itemData.itemName);
                UpdateItemInHandServerRpc(itemData.id, active);
            }
        }

        [ServerRpc]
        private void UpdateItemInHandServerRpc(int index, bool active)
        {
            GameObject itemInHand = itemsInHandParent[index];
            ItemFeatures itemsFeatures = itemInHand.GetComponentInChildren<ItemFeatures>();
            switch (itemsFeatures)
            {
                case Bat bat:
                    bat.IsActive.Value = active;
                    break;
                case Phone phone:
                    phone.IsActive.Value = active;
                    break;
                case Key key:
                    key.IsActive.Value = active;
                    break;
                case Subject subject:
                    subject.IsActive.Value = active;
                    break;
                case Headphone headphone:
                    headphone.IsActive.Value = active;
                    break;
                case Glasses glasses:
                    glasses.IsActive.Value = active;
                    break;
            }
            
            UpdateItemInHandClientRpc(index, active);
        }
        
        [ClientRpc]
        private void UpdateItemInHandClientRpc(int index, bool active)
        {
            GameObject itemInHand = itemsInHandParent[index];
            itemInHand.SetActive(active);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (PlayerManager.LocalInstance == null) return;
            
            InventoryItem firstItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        
            if (transform.childCount > 0)
            {
                firstItem.itemToExchange = itemInSlot;
                firstItem.itemToExchange.parentAfterDrag = firstItem.parentAfterDrag;
            }

            firstItem.slotToExchange = this;
            firstItem.parentAfterDrag = transform;

            InventorySlot firstSlot = firstItem.slot;
            InventorySlot secondSlot = firstItem.slotToExchange;

            (firstSlot.itemData, secondSlot.itemData) = (secondSlot.itemData, firstSlot.itemData);
            
        }
    }
}

using System;
using Game;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Interface.Inventory
{
    public class InventoryManager : NetworkBehaviour
    {
        #region Variables

        [SerializeField] public InventorySlot[] inventorySlots;
    
        [SerializeField] private Transform toolBar;
        [SerializeField] private Transform mainInventory;

        private int _selectedSlotKey = -1;

        private int selectedSlotKey
        {
            get => _selectedSlotKey;
            set => _selectedSlotKey = value - 1;
        }

        public ItemData selectedItem => GetSelectedItem();
        public InventorySlot selectedSlot => inventorySlots[selectedSlotKey];

        private const string SubjectName = "Subject";

        public event EventHandler<OnAddNewItemEventArgs> OnAddNewItem;
        public class OnAddNewItemEventArgs : EventArgs
        {
            public readonly InventorySlot InventorySlot;
            public readonly ItemData ItemData;
            public readonly int Index;

            public OnAddNewItemEventArgs(InventorySlot inventorySlot, ItemData itemData, int index)
            {
                InventorySlot = inventorySlot;
                ItemData = itemData;
                Index = index;
            }
        }
        
        public event EventHandler<OnSelectedSlotChangedEventArgs> OnSelectedSlotChanged;
        public class OnSelectedSlotChangedEventArgs : EventArgs
        {
            public readonly int SelectedSlotKey;

            public OnSelectedSlotChangedEventArgs(int selectedSlotKey = 1)
            {
                SelectedSlotKey = selectedSlotKey;
            }
        }
        
        public bool hasSubject;
        public bool hasHeadphone;
        #endregion

        private void Start()
        {
            if (!IsOwner) return;
            
            inventorySlots = new InventorySlot[toolBar.childCount + mainInventory.childCount];
            for (int i = 0; i < toolBar.childCount; i++)
            {
                inventorySlots[i] = toolBar.GetChild(i).GetComponent<InventorySlot>();
            }

            for (int i = 0; i < mainInventory.childCount; i++)
            {
                inventorySlots[i + toolBar.childCount] = mainInventory.GetChild(i).GetComponent<InventorySlot>();
            }

            if (PlayerManager.LocalInstance != null)
            {
                OnAddNewItem += InventoryManager_OnAddNewItem;
            
                OnSelectedSlotChanged += InventoryManager_OnChangeSelectedSlot;
                OnSelectedSlotChanged?.Invoke(this, new OnSelectedSlotChangedEventArgs());
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
                OnAddNewItem -= InventoryManager_OnAddNewItem;
                OnAddNewItem += InventoryManager_OnAddNewItem;
            
                OnSelectedSlotChanged -= InventoryManager_OnChangeSelectedSlot;
                OnSelectedSlotChanged += InventoryManager_OnChangeSelectedSlot;
                OnSelectedSlotChanged?.Invoke(this, new OnSelectedSlotChangedEventArgs());
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            int number = GameInputs.Instance.SelectSlot;
            if (number is >= 1 and <= 4 && number - 1 != selectedSlotKey)
            {
                OnSelectedSlotChanged?.Invoke(this, new OnSelectedSlotChangedEventArgs(number));
            }
        }

        private void InventoryManager_OnAddNewItem(object sender, OnAddNewItemEventArgs e)
        {
            SpawnInventoryItem(e.ItemData, e.InventorySlot);
                    
            if (e.ItemData.itemName == SubjectName)
            {
                hasSubject = true;
            }
            if (e.ItemData.itemName == "Headphone")
            {
                hasHeadphone = true;
            }

            if (e.Index == selectedSlotKey)
            {
                e.InventorySlot.SetItemInHandState(true);
            }
        }

        private void InventoryManager_OnChangeSelectedSlot(object sender, OnSelectedSlotChangedEventArgs e)
        {
            if (selectedSlotKey >= 0)
            {
                inventorySlots[selectedSlotKey].Deselect();
            }
            
            selectedSlotKey = e.SelectedSlotKey;
            inventorySlots[selectedSlotKey].Select();
        }
    
        public bool AddItem(ItemData itemData)
        {
            if (itemData.stackable)
            {
                foreach (InventorySlot slot in inventorySlots)
                {
                    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot != null && itemInSlot.itemData == itemData && itemInSlot.numberItem < itemInSlot.itemData.numberItemByStack)
                    {
                        itemInSlot.numberItem++;
                        itemInSlot.RefreshCount();
                        return true;
                    }
                }
            }

            if (itemData.multiple)
            {
                for (int index = 0; index < inventorySlots.Length; index++)
                {
                    InventorySlot inventorySlot = inventorySlots[index];
                    InventoryItem itemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot == null)
                    {
                        OnAddNewItem?.Invoke(this, new OnAddNewItemEventArgs(inventorySlot, itemData, index));
                        return true;
                    }
                }
            }
            else
            {
                for (int index = 0; index < inventorySlots.Length; index++)
                {
                    InventorySlot inventorySlot = inventorySlots[index];
                    if (inventorySlot.itemData == itemData) return false;

                    InventoryItem itemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
                    if (itemInSlot == null)
                    {
                        OnAddNewItem?.Invoke(this, new OnAddNewItemEventArgs(inventorySlot, itemData, index));
                        return true;
                    }
                }
            }

            return false;
        }

        private void SpawnInventoryItem(ItemData itemData, InventorySlot slot)
        {
            GameObject newInventoryItemGo = Instantiate(itemData.inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newInventoryItemGo.GetComponent<InventoryItem>();
            inventoryItem.Initialise(slot, itemData);
        }

        private ItemData GetSelectedItem()
        {
            InventorySlot slot = inventorySlots[selectedSlotKey];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                ItemData itemData = itemInSlot.itemData;
                return itemData;
            }

            return null;
        }

        public void UseSelectedItem()
        {
            InventorySlot inventorySlot = inventorySlots[selectedSlotKey];
            InventoryItem itemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        
            if (itemInSlot != null)
            {
                itemInSlot.numberItem--;
                if (itemInSlot.numberItem <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                    inventorySlot.itemData = null;
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
        }
        public void UseSubject(InventorySlot slot)
        {
            InventorySlot inventorySlot = slot;
            InventoryItem itemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        
            if (itemInSlot != null)
            {
                itemInSlot.numberItem--;
                if (itemInSlot.numberItem <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                    inventorySlot.itemData = null;
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
        }
    }
}

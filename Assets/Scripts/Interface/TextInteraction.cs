using System;
using Door;
using Game;
using Interface.Inventory;
using Items;
using Items.QuickOutline.Scripts;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    [RequireComponent(typeof(LayoutElement))]
    public class TextInteraction : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private Text headerField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private int maxCharacter = 80;
        
        public bool isTextActive => gameObject.activeSelf;

        private DoorsDetection _doorsDetection;
        private ItemsDetection _itemsDetection;
        private InventoryManager _inventoryManager;

        private Camera _mainCamera;
        private Vector3 _textPosition;

        private readonly Vector3 _textPositionRelativeToItem = new(0, 0.5f, 0);
        private const int KeyId = 1;
        
        private const string LockedDoorText = "Locked";
        private const string UnlockedDoorText = "Unlocked";
        private string _doorTextState;

        #endregion

        private void Start()
        {
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance)
            {
                _mainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
                _itemsDetection = PlayerManager.LocalInstance.itemsDetection;
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
                
                _itemsDetection.OnDetectItem += ItemsDetection_OnDetectItem;

                _itemsDetection.OnUndetectItem += ItemsDetection_OnUndetectItem;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
            
            gameObject.SetActive(false);
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance)
            {
                _mainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
                _itemsDetection = PlayerManager.LocalInstance.itemsDetection;
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
                
                _itemsDetection.OnDetectItem -= ItemsDetection_OnDetectItem;
                _itemsDetection.OnDetectItem += ItemsDetection_OnDetectItem;

                _itemsDetection.OnUndetectItem -= ItemsDetection_OnUndetectItem;
                _itemsDetection.OnUndetectItem += ItemsDetection_OnUndetectItem;
            }
        }
        
        private void ItemsDetection_OnDetectItem(object sender, EventArgs e)
        {
            if (!_itemsDetection.item) return;
            
            string header = _itemsDetection.item.itemData.itemName;
            string content = $"PICK UP  [{GameInputs.Instance.InteractKey}]";
            Show(Color.white, _itemsDetection.currentItem.transform.position, content, header);
            _doorTextState = null;
        }

        public void ShowDoorInfo(bool isLocked = false)
        {
            if (isLocked)
            {
                ShowLockedDoorInfo();
            }
            else
            {
                ShowUnlockedDoorInfo();
            }
        }

        private void ShowLockedDoorInfo()
        {
            _doorsDetection.currentItemData = _inventoryManager.selectedItem;

            if (isTextActive && (_doorTextState == LockedDoorText)) return;

            Color color = (bool)_doorsDetection.currentItemData && _doorsDetection.currentItemData.id == KeyId
                ? Color.white
                : Color.red;
            Show(color, _doorsDetection.currentDoorController.transform.position, _doorsDetection.currentDoorController.TextContent());
            _doorTextState = LockedDoorText;
        }

        private void ShowUnlockedDoorInfo()
        {
            if ((isTextActive && _doorTextState == UnlockedDoorText) || _doorsDetection.currentDoorController.AnimatorIsPlaying) return;

            Show(Color.white, _doorsDetection.currentDoorController.transform.position, _doorsDetection.currentDoorController.TextContent());
            _doorTextState = UnlockedDoorText;
        }

        private void Show(Color color, Vector3 position, string content, string header = "")
        {
            _textPosition = position + _textPositionRelativeToItem;
            if (header == "")
            {
                headerField.gameObject.SetActive(false);
            }
            else
            {
                headerField.text = header;
                headerField.color = color;
                headerField.gameObject.SetActive(true);
            }

            descriptionField.text = content;
            descriptionField.color = color;
        
            int headerLength = headerField.text.Length;
            int contentLength = descriptionField.text.Length;

            layoutElement.enabled = headerLength > maxCharacter || contentLength > maxCharacter;

            UpdatePosition();
            
            gameObject.SetActive(true);
        }
        
        private void ItemsDetection_OnUndetectItem(object sender, EventArgs e)
        {
            if ((bool)_itemsDetection.previousItem)
            {
                _itemsDetection.previousItem.GetComponent<OutlineManager>().enabled = false;   
            }
            
            Hide();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _doorTextState = null;
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            
            if (!isTextActive) return;
            
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            transform.position = _mainCamera.WorldToScreenPoint(_textPosition);
        }
    }
}

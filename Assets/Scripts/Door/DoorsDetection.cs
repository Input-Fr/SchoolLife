using System;
using Cam;
using Interface;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Door
{
    public sealed class DoorsDetection : Detection
    {
        #region Variables

        private ItemsDetection _itemsDetection;
        private TextInteraction _textInteraction;

        public MyDoorController previousDoorController;
        private MyDoorController _currentDoorController;
        public MyDoorController currentDoorController
        {
            get => _currentDoorController;
            private set
            {
                previousDoorController = currentDoorController;
                _currentDoorController = value;
            }
        }

        private bool sameDoors => currentDoorController == previousDoorController;
        public bool isLocked => currentDoorController != null && currentDoorController.isLocked.Value;

        public ItemData currentItemData;

        #endregion

        
        private void Start()
        {
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance)
            {
                MainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _itemsDetection = PlayerManager.LocalInstance.itemsDetection;
                _textInteraction = PlayerManager.LocalInstance.textInteraction;

                _itemsDetection.OnDetectItem += DoorsDetection_OnDetectItem;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPLayerSpawn;
            }
        }
        
        private void PlayerManager_OnAnyPLayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance)
            {
                MainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _itemsDetection = PlayerManager.LocalInstance.itemsDetection;
                _textInteraction = PlayerManager.LocalInstance.textInteraction;
                
                _itemsDetection.OnDetectItem -= DoorsDetection_OnDetectItem;
                _itemsDetection.OnDetectItem += DoorsDetection_OnDetectItem;
            }
        }
        
        private void DoorsDetection_OnDetectItem(object sender, EventArgs e)
        {
            currentDoorController = null;
            isDetected = false;
        }

        protected override void Update()
        {
            if (!PlayerManager.LocalInstance) return;
            
            if (!_itemsDetection.isDetected && DetectTarget())
            {
                _textInteraction.ShowDoorInfo(isLocked);
                CheckUserInteraction();
            }
            else if (!sameDoors)
            {
                _textInteraction.Hide();
            }
        }

        protected override bool DetectTarget()
        {
            GameObject newObject = null;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistanceInteraction, mask))
            {
                newObject = hit.collider.gameObject;
            }

            currentDoorController = (bool)newObject ? newObject.GetComponent<MyDoorController>() : null;
            
            isDetected = (bool)currentDoorController;
            return isDetected;
        }

        private void CheckUserInteraction()
        {
            if (isLocked) return;
            
            if (DoorsInteraction.ChangeDoorState(currentDoorController))
            {
                UndetectDoorServerRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void UndetectDoorServerRpc()
        {
            UndetectDoorClientRpc();
        }

        [ClientRpc]
        private void UndetectDoorClientRpc()
        {
            if (PlayerManager.LocalInstance)
            {
                PlayerManager.LocalInstance.textInteraction.Hide();
            }
        }
    }
}

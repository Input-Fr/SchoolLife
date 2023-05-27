using System;
using Game;
using Interface.Inventory;
using PlayerScripts;
using Professors;
using Unity.Netcode;
using UnityEngine;

namespace Items.PhoneFeatures
{
    public class Phone : ItemFeatures
    {
        #region Variables

        [SerializeField] private AudioSource audioSource;

        private InventoryManager _inventoryManager;

        public readonly NetworkVariable<bool> IsActive = new();

        #endregion

        private void Start()
        {
            transform.parent.gameObject.SetActive(IsActive.Value);

            if (PlayerManager.LocalInstance != null)
            {
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
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
                Debug.Log($"Local Instance : {PlayerManager.LocalInstance}");                
                Debug.Log($"Inventory Manager : {(PlayerManager.LocalInstance? PlayerManager.LocalInstance.inventoryManager : null)}");
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
            }
        }

        private void Update()
        {
            if (PlayerManager.LocalInstance == null) return;

            if (!GameInputs.Instance.Use) return;

            GameInputs.Instance.ResetUseInput();
            MakeSound();
        }

        private void MakeSound()
        {
            InventorySlot slot = _inventoryManager.selectedSlot;
            WaitingTime waitingTime = slot.GetComponentInChildren<WaitingTime>();

            if (waitingTime.isFinish)
            {
                audioSource.Play();
                waitingTime.UpdateTimer(30);
                Sound sound = new Sound(transform.position, audioSource.maxDistance);
                Sounds.MakeSound(sound);
            }
        }
    }
}
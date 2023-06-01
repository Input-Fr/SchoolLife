using System;
using Game;
using Interface;
using Interface.Inventory;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopSystem : NetworkBehaviour
    {
    
        [SerializeField] private Button phoneBtn;
        [SerializeField] private Button batBtn;
        [SerializeField] private Button headphoneBtn;
        [SerializeField] private Button glasseBtn;
        [SerializeField] private Button exitBtn;
        
        [SerializeField] private ItemData phone;
        [SerializeField] private ItemData bat;
        [SerializeField] private ItemData headphone;
        [SerializeField] private ItemData glasses;

        [SerializeField] private Pause pause;
        
        private InventoryManager _inventory;
        private GameInputs _gameInputs;

        void OnEnable()
        {
            if (_gameInputs)
            {
                pause.canLockCursor = false;
                _gameInputs.inInterface = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnDisable()
        {
            if (_gameInputs)
            {
                Cursor.lockState = CursorLockMode.Locked;
                _gameInputs.inInterface = false;
                pause.canLockCursor = true;
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance != null)
            {
                HUDSystem hudSystem = PlayerManager.LocalInstance.hudSystem;
                
                _inventory = PlayerManager.LocalInstance.inventoryManager;
                _gameInputs = PlayerManager.LocalInstance.gameInput;
                
                phoneBtn.onClick.AddListener(() =>
                {
                    if (hudSystem.wealth >= 100 && _inventory.AddItem(phone))
                    {
                        hudSystem.wealth -= 100;
                    }
                });
        
                batBtn.onClick.AddListener(() => 
                {
                    if (hudSystem.wealth >= 75 && _inventory.AddItem(bat))
                    {
                        hudSystem.wealth -= 75;
                    }
                });
                
                headphoneBtn.onClick.AddListener(() => 
                {
                    if (hudSystem.wealth >= 90 && _inventory.AddItem(headphone))
                    {
                        hudSystem.wealth -= 90;
                    }
                });
        
                glasseBtn.onClick.AddListener(() => 
                {
                    if (hudSystem.wealth >= 130 && _inventory.AddItem(glasses))
                    {
                        hudSystem.wealth -= 130;
                    }
                });
                
                exitBtn.onClick.AddListener(() => 
                {
                    gameObject.SetActive(false);
                });        
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
                HUDSystem hudSystem = PlayerManager.LocalInstance.hudSystem;
                
                _inventory = PlayerManager.LocalInstance.inventoryManager;
                _gameInputs = PlayerManager.LocalInstance.gameInput;
                
                phoneBtn.onClick.AddListener(() =>
                {
                    if (hudSystem.wealth >= 100 && _inventory.AddItem(phone))
                    {
                        hudSystem.wealth -= 100;
                    }
                });
        
                batBtn.onClick.AddListener(() => 
                {
                    if (hudSystem.wealth >= 75 && _inventory.AddItem(bat))
                    {
                        hudSystem.wealth -= 75;
                    }
                });
                
                headphoneBtn.onClick.AddListener(() => 
                {
                    if (hudSystem.wealth >= 90 && _inventory.AddItem(headphone))
                    {
                        hudSystem.wealth -= 90;
                    }
                });
        
                exitBtn.onClick.AddListener(() => 
                {
                    gameObject.SetActive(false);
                });        
            }
        }
    }
}

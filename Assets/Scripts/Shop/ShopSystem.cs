using System;
using Game;
using Interface;
using Interface.Inventory;
using Items;
using PlayerScripts;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopSystem : NetworkBehaviour
    {
    
        [SerializeField] private Button phoneBtn;
        [SerializeField] private Button batBtn;
        [SerializeField] private Button headphoneBtn;
        [SerializeField] private Button exitBtn;
        
        [SerializeField] private ItemData phone;
        [SerializeField] private ItemData bat;
        [SerializeField] private ItemData headphone;
        
        private InventoryManager _inventory;

        void OnEnable()
        {
            GameObject.FindWithTag("InputsManager").GetComponent<GameInputs>().inInterface = true;

        }

        private void OnDisable()
        {
            GameObject.FindWithTag("InputsManager").GetComponent<GameInputs>().inInterface = false;
        }

        private void Start()
        { 
            
            gameObject.SetActive(false);
        
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance != null)
            {
                HUDSystem hudSystem = PlayerManager.LocalInstance.hudSystem;
                
                _inventory = PlayerManager.LocalInstance.inventoryManager;
                
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

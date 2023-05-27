using System;
using Interface.Inventory;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    public class Item : NetworkBehaviour
    {
        #region Variables
        
        [SerializeField] public ItemData itemData;

        private InventoryManager _inventoryManager;

        #endregion

        private void Start()
        {
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
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
            }
        }

        public bool Pickup()
        {
            if (_inventoryManager.AddItem(itemData))
            {
                DestroyItemServerRpc();
                return true;
            }
            
            Debug.Log("Can't ADD the Item");
            return false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyItemServerRpc()
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}

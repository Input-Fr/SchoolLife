using Game;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface.Inventory
{
    public class Background : NetworkBehaviour, IDropHandler
    {
        #region Variables

        [SerializeField] private Transform dropPoint;

        #endregion

        public void OnDrop(PointerEventData eventData)
        {
            if (!IsOwner) return;
            
            InventoryItem item = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (item.itemData.id == 4)
            {
                PlayerManager.LocalInstance.inventoryManager.hasHeadphone = false;
            }
                
            if (item.itemData.itemName == "Subject")
            {
                PlayerManager.LocalInstance.inventoryManager.hasSubject = false;
            }
            for (int i = 0; i < item.numberItem; i++)
            {
                GameMultiplayer.Instance.InstantiateItem(item.itemData, dropPoint.position, Quaternion.identity);
            }
        
            item.isDropped = true;
        }
    }
}

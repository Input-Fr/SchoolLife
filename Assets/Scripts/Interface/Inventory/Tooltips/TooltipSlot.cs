using System;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine.EventSystems;

namespace Interface.Inventory.Tooltips
{
    public class TooltipSlot : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Variables

        private Tooltip _tooltip;

        public ItemData itemData;

        #endregion

        public void Start()
        {
            if (!IsOwner) return;
            
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PlayerManager.LocalInstance == null) return;
            
            if (itemData == null || _tooltip == null) return;
            
            _tooltip.Show(itemData.description, itemData.itemName);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (PlayerManager.LocalInstance == null) return;
            
            _tooltip.Hide();
        }
    }
}

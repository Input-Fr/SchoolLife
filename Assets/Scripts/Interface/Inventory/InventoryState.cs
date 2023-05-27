using Game;
using Unity.Netcode;
using UnityEngine;

namespace Interface.Inventory
{
    public class InventoryState : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private GameObject mainInventoryGroup;
        [SerializeField] private GameObject button;

        public bool isOpen;

        #endregion

        private void Start()
        {
            if (!IsOwner) return;
            
            mainInventoryGroup.SetActive(false);
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            if (GameInputs.Instance.ChangeInventoryState)
            {
                SetInventoryState(!isOpen);
            }
        }

        public void SetInventoryState(bool open)
        {
            isOpen = open;
            mainInventoryGroup.SetActive(open);
            button.SetActive(!open);
            GameInputs.Instance.ResetChangeInventoryStateInput();
        }
    }
}

using Game;
using Unity.Netcode;
using UnityEngine;

namespace Interface.Inventory
{
    public class InventoryState : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private GameObject mainInventoryGroup;

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
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            isOpen = open;
            mainInventoryGroup.SetActive(open);
            GameInputs.Instance.ResetChangeInventoryStateInput();
        }
    }
}

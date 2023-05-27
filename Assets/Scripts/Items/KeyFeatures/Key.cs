using System;
using Door;
using Game;
using PlayerScripts;
using Unity.Netcode;

namespace Items.KeyFeatures
{
    public class Key : ItemFeatures
    {
        #region Variables

        private DoorsDetection _doorsDetection;

        public readonly NetworkVariable<bool> IsActive = new();

        #endregion

        private void Start()
        {
            transform.parent.gameObject.SetActive(IsActive.Value);

            if (!IsOwner) return;

            if (PlayerManager.LocalInstance != null)
            {
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
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
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
            }
        }

        private void Update()
        {
            if (PlayerManager.LocalInstance == null) return; 

            if (!GameInputs.Instance.Use || !_doorsDetection.isDetected || !_doorsDetection.isLocked) return;
            
            GameInputs.Instance.ResetInteractInput();
            UnlockDoor();
        }

        private void UnlockDoor()
        {
            DisableParentServerRpc();
            _doorsDetection.currentDoorController.Unlock();
            _doorsDetection.UndetectDoorServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DisableParentServerRpc()
        {
            IsActive.Value = false;
            DisableParentClientRpc();
        }

        [ClientRpc]
        private void DisableParentClientRpc()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
using System;
using Game;
using Interface.Inventory;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Door
{
    public class MyDoorController : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private Animator doorAnimator;

        public NetworkVariable<bool> isOpen = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLocked = new(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public bool AnimatorIsPlaying => doorAnimator.GetCurrentAnimatorStateInfo(0).length > doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        private const string LockedTag = "Locked";
        private const string UnlockedTag = "Unlocked";

        private const string OpenDoorAnimationName = "DoorOpen";
        private const string CloseDoorAnimationName = "DoorClose";
        
        private InventoryManager _inventoryManager;

        #endregion

        private void Awake()
        {
            gameObject.tag = isLocked.Value ? LockedTag : UnlockedTag;
        }

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

        public void Unlock()
        {
            _inventoryManager.UseSelectedItem();
            SetDoorVariablesServerRpc(false, UnlockedTag);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetDoorVariablesServerRpc(bool newValue, string newTag)
        {
            isLocked.Value = newValue;
            SetDoorTagClientRpc(newTag);
        }

        [ClientRpc]
        private void SetDoorTagClientRpc(string newTag)
        {
            gameObject.tag = newTag;
        }
        
        public void SetDoorState(bool open)
        {
            if (open)
            {
                PlayAnimationServerRpc(true, OpenDoorAnimationName);
            }
            else
            {
                PlayAnimationServerRpc(false, CloseDoorAnimationName);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void PlayAnimationServerRpc(bool value, string animationName)
        {
            isOpen.Value = value;
            PlayerAnimationClientRpc(animationName);
        }

        [ClientRpc]
        private void PlayerAnimationClientRpc(string animationName)
        {
            doorAnimator.Play(animationName, 0, 0f);
            StartCoroutine(GameMultiplayer.BakeSurface());
        }

        public string TextContent()
        {
            return isLocked.Value ? 
                $"UNLOCK  [{GameInputs.Instance.UseKey}]" :
                isOpen.Value ? 
                    $"CLOSE  [{GameInputs.Instance.InteractKey}]" : 
                    $"OPEN  [{GameInputs.Instance.InteractKey}]";
        }
    }
}

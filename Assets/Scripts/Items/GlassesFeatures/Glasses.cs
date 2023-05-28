using System;
using System.Collections;
using Game;
using Interface.Inventory;
using Items.PhoneFeatures;
using Items.QuickOutline.Scripts;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Items.GlassesFeatures
{
    public class Glasses : ItemFeatures
    {
        [SerializeField] private GameObject glassesOnHead;
        public readonly NetworkVariable<bool> IsActive = new();

        private InventoryManager _inventoryManager;
        
        private GameObject[] _professors;
        private const string ProfessorsTag = "Professor";

        private bool _isInUse;

        private void Start()
        {
            transform.parent.gameObject.SetActive(IsActive.Value);
            
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance != null)
            {
                _professors = GameObject.FindGameObjectsWithTag(ProfessorsTag);
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance)
            {
                _professors = GameObject.FindGameObjectsWithTag(ProfessorsTag);
                _inventoryManager = PlayerManager.LocalInstance.inventoryManager;
            }
        }

        private void Update()
        {
            if (!PlayerManager.LocalInstance) return; 

            if (!GameInputs.Instance.Use || _isInUse) return;
            
            GameInputs.Instance.ResetInteractInput();
            
            InventorySlot slot = _inventoryManager.selectedSlot;
            WaitingTime waitingTime = slot.GetComponentInChildren<WaitingTime>();

            if (waitingTime.isFinish)
            {
                waitingTime.UpdateTimer(5);
                StartCoroutine(EnableProfessorsOutline());
            }
        }

        private IEnumerator EnableProfessorsOutline()
        {
            _isInUse = true;
            gameObject.SetActive(false);
            glassesOnHead.SetActive(true);

            ChangeProfessorsOutlineState(true);

            yield return new WaitForSeconds(5f);

            Debug.Log($"AFTER WAIT");
            ChangeProfessorsOutlineState(false);
            glassesOnHead.SetActive(false);
            _inventoryManager.UseSelectedItem();
        }

        private void ChangeProfessorsOutlineState(bool state)
        {
            foreach (GameObject professor in _professors)
            {
                Debug.Log($"outline state : {state}");
                professor.GetComponent<OutlineManager>().enabled = state;
            }
        }
    }
}

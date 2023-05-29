using System;
using System.Collections;
using Interface.Inventory;
using Items.PhoneFeatures;
using Items.QuickOutline.Scripts;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

public class GlassesManager : NetworkBehaviour
{
    [SerializeField] private GameObject glassesInHand;
    [SerializeField] private GameObject glassesOnHead;

    private InventoryManager _inventoryManager;

    private GameObject[] _professors;
    private const string ProfessorsTag = "Professor";

    private void Start()
    {
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

    public void UseGlasses()
    {
        InventorySlot slot = _inventoryManager.selectedSlot;
        WaitingTime waitingTime = slot.GetComponentInChildren<WaitingTime>();
        waitingTime.UpdateTimer(5);
        StartCoroutine(EnableProfessorsOutline());
    }

    private IEnumerator EnableProfessorsOutline()
    {
        glassesInHand.gameObject.SetActive(false);
        glassesOnHead.SetActive(true);

        ChangeProfessorsOutlineState(true);

        yield return new WaitForSeconds(5f);

        ChangeProfessorsOutlineState(false);
        glassesOnHead.SetActive(false);
        _inventoryManager.UseSelectedItem();
    }

    private void ChangeProfessorsOutlineState(bool state)
    {
        foreach (GameObject professor in _professors)
        {
            professor.GetComponent<OutlineManager>().enabled = state;
        }
    }
}

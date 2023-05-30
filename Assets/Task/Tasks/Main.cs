using UnityEngine;
using Unity.Netcode;
using PlayerScripts;
using Interface.Inventory;
using System;
using Game;
using Interface;
using Items;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Main : NetworkBehaviour
    {
    public int switchCount;
    public bool hasSpawned;
    public int onCount;
    public GameObject canvas;
    [SerializeField] private ItemData key;
    [SerializeField] private AudioSource done;
    
    [SerializeField] private Pause pause;

    public Switch switch1;
    public Switch switch2;
    public Switch switch3;
    public Switch switch4;
    public Switch switch5;
    public Switch switch6;
    public Switch switch7;
    public Switch switch8;
    public Switch switch9;
    public Switch switch10;
    public Switch switch11;
    public Switch switch12;

    InventoryManager _inventory;
    private GameInputs _gameInputs;
    
    public GameObject currentButton; // talvin il faut pas les supprimer ces variables elle sont en public mais prennent pas leurs valeurs en SerializeField
    //Ok Ivan bien reçu mdrrrr
    
    private void OnEnable()
    {
        if (_gameInputs)
        {
            pause.canLockCursor = false;
            _gameInputs.inInterface = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnDisable()
    {
        if (_gameInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _gameInputs.inInterface = false;
            pause.canLockCursor = true;
            if (hasSpawned) done.Play();
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
        if (!IsOwner) return;
        
        if (PlayerManager.LocalInstance != null)
        {
            _inventory = PlayerManager.LocalInstance.inventoryManager;
            _gameInputs = PlayerManager.LocalInstance.gameInput;
        }
        else
        {
            PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
        }
        hasSpawned = true;
    }

    void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
    {
        if (PlayerManager.LocalInstance != null)
        {
            _inventory = PlayerManager.LocalInstance.inventoryManager;
            _gameInputs = PlayerManager.LocalInstance.gameInput;
        }
    }

    
    public void SwitchChange(int points)
    {
        if (!IsOwner) return;
        if (PlayerManager.LocalInstance == null) return;

        onCount = onCount + points;
        if (onCount == switchCount)
        {
            canvas.SetActive(false);
            

            onCount = 0;
            switchCount = 12;
            if (currentButton.layer == LayerMask.NameToLayer("button"))
            {
                _inventory.AddItem(key);
            }
            else
            {
                int pointToAdd = 2;
                if (pointToAdd + PlayerManager.LocalInstance.hudSystem.points > 20)
                {
                    Debug.Log("Sorry but you can get more than 20 points");
                }
                else
                {
                    PlayerManager.LocalInstance.hudSystem.points += pointToAdd;
                }
                
            }
            

            switch1.ResetTask();
            switch2.ResetTask();
            switch3.ResetTask();
            switch4.ResetTask();
            switch5.ResetTask();
            switch6.ResetTask();
            switch7.ResetTask();
            switch8.ResetTask();
            switch9.ResetTask();
            switch10.ResetTask();
            switch11.ResetTask();
            switch12.ResetTask();
        }
    }
    
    
}
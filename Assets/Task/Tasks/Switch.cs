using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using PlayerScripts;
using System;
public class Switch : NetworkBehaviour
{
    public GameObject up;
    public GameObject on;
    public bool isOn;
    public bool isUp;
    Main main;

    // Start is called before the first frame update



   
    void Start()
    {
        
        if (!IsOwner) return;
        
        if (PlayerManager.LocalInstance != null)
        {
            main = PlayerManager.LocalInstance.main;
            int temp = UnityEngine.Random.Range(0,10);
            int temp2 = UnityEngine.Random.Range(0,2);
            isOn = (temp>8);
            isUp = (temp2 == 1);
            on.SetActive(isOn);
            up.SetActive(isUp);
            if (isOn)
            {
                main.SwitchChange(1);
            }
        }
        else
        {
            PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
        }
        
    }

     void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
    {
        if (PlayerManager.LocalInstance != null)
        {
            main = PlayerManager.LocalInstance.main;
            int temp = UnityEngine.Random.Range(0,10);
            int temp2 = UnityEngine.Random.Range(0,2);
            isOn = (temp>8);
            isUp = (temp2 == 1);
            on.SetActive(isOn);
            up.SetActive(isUp);
            if (isOn)
            {
                main.SwitchChange(1);
            }
        }
    }

    public void SwitchActionButton()
    {
        if (!IsOwner) return;
        isUp = !isUp;
        isOn = !isOn;
        on.SetActive(isOn);
        up.SetActive(isUp);
        if (isOn)
        {
            main.SwitchChange(1);
        }
        else
        {
            main.SwitchChange(-1);
        }
    }

    public void ResetTask(){
        if (!IsOwner) return;
        int temp = UnityEngine.Random.Range(0,10);
        int temp2 = UnityEngine.Random.Range(0,2);
        isOn = (temp>8);
        isUp = (temp2 == 1);
        on.SetActive(isOn);
        up.SetActive(isUp);
        if (isOn)
        {
            main.SwitchChange(1);
        }
    }

    // Update is called once per frame
}

using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

public class activationMonitor : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PlayerManager.LocalInstance.inventoryManager.hasHeadphone);
        if (!PlayerManager.LocalInstance.inventoryManager.hasHeadphone)
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

public class dontMove : MonoBehaviour
{
    void OnEnable()
    {
        PlayerManager.LocalInstance.gameInput.inInterface = true;
    }

    private void OnDisable()
    {
        PlayerManager.LocalInstance.gameInput.inInterface = false;
    }
}

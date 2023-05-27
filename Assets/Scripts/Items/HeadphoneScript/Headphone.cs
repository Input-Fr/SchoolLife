using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using PlayerScripts;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Headphone : ItemFeatures
{
    public readonly NetworkVariable<bool> IsActive = new();
    public GameObject headphoneOnHead;
    private bool wannaListen;
    private void Start()
    {
        transform.parent.gameObject.SetActive(IsActive.Value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            wannaListen = !wannaListen;
            headphoneOnHead.SetActive(wannaListen);
            GetComponent<MeshRenderer>().enabled = !wannaListen;
            foreach (MeshRenderer componentsInChild in GetComponentsInChildren<MeshRenderer>())
            {
                componentsInChild.enabled = !wannaListen;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class timerGlobal : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI textGeneric;
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private TextMeshProUGUI textEvent;
    [SerializeField] private GameObject antiCheat;
    [SerializeField] private GameObject txt;
    [SerializeField] private GameObject back;
    bool isNotInPause = false;
    bool isDone = false;

    
    public static float timeInSecMCQ = 480;
    public static float timeInSecPause = 60;

    public override void OnNetworkSpawn()
    {
        back.SetActive(true);
    }

    [ClientRpc]
    void DesactivateDoorClientRpc(bool active)
    {
        antiCheat.SetActive(active);
        txt.GetComponent<MeshRenderer>().enabled = active;
        
    }

    [ClientRpc]
    void updateClClientRpc(int min,int sec, bool isInPauseorNot)
    {
        textTime.text = $"{min}' {sec}''";
        if (isInPauseorNot)
        {
            textEvent.text = "Next exam : ";
        }
        else
        {
            textEvent.text = "Next train : ";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (timeInSecMCQ > 0)
        {
            if (isNotInPause)
            {
                timeInSecMCQ -= Time.deltaTime;
                updateClClientRpc(((int)timeInSecMCQ)/60,((int)timeInSecMCQ)%60, isNotInPause);
                if ((((int)timeInSecMCQ)/60) < 5)
                {
                    DesactivateDoorClientRpc(true);
                }
                else{
                    DesactivateDoorClientRpc(false);
                }
            }
            else
            {
                isNotInPause = true;
                updateClClientRpc(((int)timeInSecMCQ)/60,((int)timeInSecMCQ)%60, isNotInPause);
                DesactivateDoorClientRpc(false);
            }
        }
        else
        {
            isNotInPause = false;

        }
        if (!isNotInPause)
        {
            if (timeInSecPause > 0)
            {
                timeInSecPause -= Time.deltaTime;
                updateClClientRpc((int)timeInSecPause/60,((int)timeInSecPause)%60, isNotInPause);

                if ((int)timeInSecMCQ == 0 && (int)timeInSecPause == 59)
                {
                    if (!isDone)
                    {
                        Debug.Log("UPDATE HUD");
                        UpdatePlayerHudClientRpc();
                        isDone = true;
                    }
                }
                else
                {
                    isDone = false;
                }
                
            }
            else
            {
                timeInSecMCQ = 480;
                timeInSecPause = 60;
            }
        }
    }

    [ClientRpc]
    private void UpdatePlayerHudClientRpc()
    {
        if (PlayerManager.LocalInstance != null)
        {
            PlayerManager.LocalInstance.hudSystem.UpdateHUD();
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using PlayerScripts;
using Tasks;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Random = UnityEngine.Random;

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
    private bool hasSpawnedSubject;
    public static List<GameObject> playerTransforms = new List<GameObject>();
    public List<TaskManager> tasks = new List<TaskManager>();


    public static float timeInSecMCQ = 60;
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

    void tpAllPlayer()
    {
        foreach (var player in playerTransforms.ToList())
        {
            Debug.Log($"Player \"{player.name}\" is in list");
            player.GetComponent<ThirdPersonController>().enabled = false;
            player.transform.position = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
            player.GetComponent<ThirdPersonController>().enabled = true;
            playerTransforms.Remove(player);
            Debug.Log($"List size : {playerTransforms.Count}");
        }
    }
    
    void DespawnSubject()
    {
        var subs = GameObject.FindGameObjectsWithTag("Item");
        foreach (var sub in subs)
        {
            if (sub.name.Contains("Subject"))
            {
                sub.GetComponent<NetworkObject>()?.Despawn();
                Destroy(sub);
            }
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
                if ((((int)timeInSecMCQ)/60) < 2)
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
                if (!hasSpawnedSubject)
                {
                    tpAllPlayer();
                    DespawnSubject();
                    PlayerNetwork.SpawnSubject();
                    hasSpawnedSubject = true;
                    foreach (TaskManager task in tasks)
                    {
                        task.isTaskDone = false;
                    }
                }
                timeInSecPause -= Time.deltaTime;
                updateClClientRpc((int)timeInSecPause/60,((int)timeInSecPause)%60, isNotInPause);

                if ((int)timeInSecMCQ == 0 && (int)timeInSecPause == 59)
                {
                    if (!isDone)
                    {
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
                timeInSecMCQ = 4*60;
                timeInSecPause = 60;
                hasSpawnedSubject = false;
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

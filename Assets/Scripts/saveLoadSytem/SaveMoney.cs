using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerScripts;
using UnityEngine;
using Unity.Netcode;

public class SaveMoney : NetworkBehaviour
{
    public async override void OnNetworkSpawn()
    {
        await Task.Delay(300);
        PlayerManager.LocalInstance.hudSystem.points = PlayerPrefs.GetInt("Points", 0);
        PlayerManager.LocalInstance.hudSystem.wealth = PlayerPrefs.GetInt("Money", 0);
    }

    public override void OnNetworkDespawn()
    {
        PlayerPrefs.SetInt("Points", PlayerManager.LocalInstance.hudSystem.points);
        PlayerPrefs.SetInt("Money", PlayerManager.LocalInstance.hudSystem.wealth);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInside : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log($"Player \"{other.gameObject.name}\" entered");
            timerGlobal.playerTransforms.Add(other.gameObject);
            Debug.Log($"List size : {timerGlobal.playerTransforms.Count}");
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log($"Player \"{other.gameObject.name}\" exited");
            timerGlobal.playerTransforms.Remove(other.gameObject);
            Debug.Log($"List size : {timerGlobal.playerTransforms.Count}");
        }
    }
}

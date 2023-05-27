using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public int numTask = 0;
    public GameObject Door1;

    public GameObject Tp;

    public GameObject Player;
    // Start is called before the first frame update

    // Update is called once per frame
    public void FinishTask()
    {
        numTask += 1;
        if (numTask == 1)
        {
            Door1.SetActive(false);
        }
    }

    public void PlayerCatched()
    {
        Player.transform.position = Tp.transform.position;
    }

}

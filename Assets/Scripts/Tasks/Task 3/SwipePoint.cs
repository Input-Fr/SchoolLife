using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipePoint : MonoBehaviour
{
    // Start is called before the first frame update
    private SwipeTask _swipeTask;

    private void Awake(){
        _swipeTask = GetComponentInParent<SwipeTask>();
    }

    private void OnTriggerEnter2D(Collider2D other){
        _swipeTask.SwipePointTrigger(this);
    }
}
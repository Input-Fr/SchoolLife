using System.Collections;
using System.Collections.Generic;
using Tasks.Task_3;
using UnityEngine;

public class SwipeTask : MonoBehaviour
{
    public CardTask cardTask;
    
    public List<SwipePoint> _swipePoints = new List<SwipePoint>();
    public float _countdownMax = 0.5f;
    public GameObject _greenOn;
    public GameObject _redOn;
    private int _currentSwipePointIndex;
    private float _countdown;

    private void Update(){
        _countdown -= Time.deltaTime;
        if (_currentSwipePointIndex != 0 && _countdown<= 0){
            _currentSwipePointIndex = 0;
            StartCoroutine(FinishTask(false));
        }
    }

    private IEnumerator FinishTask(bool wasSuccessful) {
        if (wasSuccessful){
            _greenOn.SetActive(true);
        }
        else {
            _redOn.SetActive(true);
        }
        yield return new WaitForSeconds(1.5f);
        if (_greenOn.activeSelf)
        {
            _greenOn.SetActive(false);
            cardTask.ValidAccess();
        }
        else
        {
            _redOn.SetActive(false);
        }
    }

    public void SwipePointTrigger(SwipePoint swipePoint) {
        if (swipePoint == _swipePoints[_currentSwipePointIndex]){
            _currentSwipePointIndex++;
            _countdown = _countdownMax;
        }
        
        if (_currentSwipePointIndex >= _swipePoints.Count){
            _currentSwipePointIndex = 0;
            StartCoroutine(FinishTask(true));
        }
    }
}

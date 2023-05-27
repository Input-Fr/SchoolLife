using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWire : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject wireEnd;
    Vector3 startPoint;

    void Start(){
        startPoint = transform.parent.position;
    }
    // Start is called before the first frame update
    public void OnDrag (PointerEventData eventData)
    {
        Vector3 newPosition = Input.mousePosition;
        newPosition.z = 0;
        transform.position = newPosition;
        float dist = Vector2.Distance(startPoint, newPosition);
        wireEnd.transform.localScale = new Vector3(dist, wireEnd.transform.localScale.y, 1);
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.localPosition = Vector3.zero;
    }
}

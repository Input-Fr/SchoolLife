using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ButtonD
{
public class ButtonD : MonoBehaviour
{
    private Camera _cam;
    private GameObject _currentButton;

    private GameObject canvas;
    private LayerMask buttonMask;
    private LayerMask environment;

    private float maxDistanceInteraction;
    private float viewAngle;

    GameObject FindInActiveObjectByTag(string tag)
        {
            GameObject res = null;
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].hideFlags == HideFlags.None)
                {
                    if (objs[i].CompareTag(tag))
                    {
                        res = objs[i].gameObject;
                    }
                }
            }
            return res;
        }

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        buttonMask = 5;
        //environment = 7;
        maxDistanceInteraction = 5;
        viewAngle = 30;
    }

    private void Update()
    {
        _currentButton = DetectButton();
        if ((bool)_currentButton && Input.GetKeyDown(KeyCode.E))
        {
            canvas = FindInActiveObjectByTag("TaskS");
            canvas.SetActive(true);
        }
    }

    private GameObject DetectButton()
    {
        // ReSharper disable once Unity.PreferNonAllocApi
        RaycastHit[] buttons = Physics.SphereCastAll(transform.position, maxDistanceInteraction, transform.forward,
            maxDistanceInteraction, buttonMask);

        if (buttons.Length > 0)
        {
            Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(_cam);
            foreach (RaycastHit buttonHit in buttons)
            {
                Bounds hitBounds = buttonHit.collider.bounds;
                if (!GeometryUtility.TestPlanesAABB(cameraFrustum, hitBounds)) continue;

                Vector3 directionToTarget = (buttonHit.transform.position - transform.position).normalized;

                float distanceToTarget = Vector3.Distance(transform.position, buttonHit.transform.position);
                if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, environment)) continue;

                float angle = Vector3.Angle(transform.forward, directionToTarget);
                if (!(angle < viewAngle / 2)) continue;
                return buttonHit.transform.gameObject;
            }
        }

        return null;
    }
}
}
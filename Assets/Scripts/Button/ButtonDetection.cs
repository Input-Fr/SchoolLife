using System;
using Game;
using PlayerScripts;
using Tasks;
using Tasks.Task_1;
using Tasks.Task_2;
using Unity.Netcode;
using UnityEngine;

namespace Cam
{
    public class ButtonDetection : NetworkBehaviour
    {
        [SerializeField] private float maxDistanceInteraction = 5;
        [SerializeField] private float viewAngle = 30;
        
        [SerializeField] private LayerMask buttonMask;
        [SerializeField] private LayerMask environment;

        private Vector3 origin => transform.position;
        private Vector3 direction => transform.TransformDirection(Vector3.forward);
        
        private GameObject _currentButton;
        private Camera _cam;
    
        private void Start() {
            if (!IsOwner) return;

            if (PlayerManager.LocalInstance != null)
            {
                _cam = GetComponent<Camera>();
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
        }

        void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance != null)
            {
                _cam = GetComponent<Camera>();
            }
        }

        private void Update()
        {
            _currentButton = DetectButton();
            if (_currentButton != null && GameInputs.Instance.Interact)
            {
                
                GameInputs.Instance.ResetInteractInput();
                TaskManager taskManager = _currentButton.GetComponent<TaskManager>();
                GameObject taskUi = PlayerManager.LocalInstance.allTasksUI[taskManager.taskIndex];
                taskUi.GetComponent<TaskState>().taskManager = taskManager;


                switch (taskManager)
                {
                    case SwitchTask task:
                        foreach (Transform t in taskUi.transform)
                        {
                            t.GetComponent<Switch>().switchTask = task;
                        }
                        break;
                    case CodeTask task:
                        taskUi.transform.GetChild(0).GetComponent<KeypadTask>().codeTask = task;
                        break;
                }

                taskUi.SetActive(true);
            }
        }

        private GameObject DetectButton()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] buttons = Physics.SphereCastAll(origin, maxDistanceInteraction, direction,
                maxDistanceInteraction, buttonMask);

            if (buttons.Length > 0)
            {
                Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(_cam);
                foreach (RaycastHit buttonHit in buttons)
                {
                    Bounds hitBounds = buttonHit.collider.bounds;
                    if (GeometryUtility.TestPlanesAABB(cameraFrustum, hitBounds))
                    {
                        Vector3 position = buttonHit.transform.position;
                        Vector3 directionToTarget = (position - transform.position).normalized;

                        float distanceToTarget = Vector3.Distance(origin, position);
                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, environment))
                        {
                            float angle = Vector3.Angle(direction, directionToTarget);
                            if (angle < viewAngle / 2)
                            {
                                return buttonHit.transform.gameObject;
                            }
                        }
                    }
                }
            }

            return null;
        }
    
    }
}

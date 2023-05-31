using System;
using Game;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Shop
{
    public class ShopDetection : NetworkBehaviour
    {
        [SerializeField] private float maxDistanceInteraction = 5;
        [SerializeField] private float viewAngle = 30;
        
        [SerializeField] private LayerMask shopMask;
        [SerializeField] private LayerMask environment;

        private Vector3 Origin => transform.position;
        private Vector3 direction => transform.TransformDirection(Vector3.forward);
        
        private GameObject _currentButton;
        private Camera _cam;
        private GameObject _canvas;
    
        private void Start() {
            
            if (!IsOwner) return;
            
            if (PlayerManager.LocalInstance != null)
            {
                _canvas = PlayerManager.LocalInstance.shop;
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
                _canvas = PlayerManager.LocalInstance.shop;
                _cam = GetComponent<Camera>();
            }
        }

        private void Update()
        {
            if (PlayerManager.LocalInstance == null) return;

            _currentButton = DetectButton();
            if (_currentButton != null && GameInputs.Instance.Interact)
            {
                _canvas.SetActive(true);
            }
        }

        private GameObject DetectButton()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] buttons = Physics.SphereCastAll(Origin, maxDistanceInteraction, direction,
                maxDistanceInteraction, shopMask);

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

                        float angle = Vector3.Angle(direction, directionToTarget);
                        if (angle < viewAngle / 2)
                        {
                            return buttonHit.transform.gameObject;
                        }
                    }
                }
            }

            return null;
        }
    }
}

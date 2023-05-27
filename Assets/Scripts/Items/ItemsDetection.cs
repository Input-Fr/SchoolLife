using System;
using Cam;
using Door;
using Interface;
using Items.QuickOutline.Scripts;
using PlayerScripts;
using UnityEngine;

namespace Items
{
    public sealed class ItemsDetection : Detection
    {
        #region Variables

        private DoorsDetection _doorsDetection;
        private TextInteraction _textInteraction;

        public Item item => (bool)currentItem? currentItem.GetComponent<Item>() : null;

        [SerializeField] private float viewAngle;
        [SerializeField] private LayerMask environment;
        
        public GameObject previousItem;
        private GameObject _currentItem;
        public GameObject currentItem
        {
            get => _currentItem;
            private set
            {
                previousItem = currentItem;
                _currentItem = value;
            }
        }
        
        private bool sameItems => currentItem == previousItem;
        
        public event EventHandler OnDetectItem;
        public event EventHandler OnUndetectItem;

        #endregion

        private void Start()
        {
            if (!IsOwner) return;
            
            if (PlayerManager.LocalInstance != null)
            {
                MainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
                _textInteraction = PlayerManager.LocalInstance.textInteraction;
                
                OnDetectItem += ItemsInteraction_OnDetectItem;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance != null)
            {
                MainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
                _doorsDetection = PlayerManager.LocalInstance.doorsDetection;
                _textInteraction = PlayerManager.LocalInstance.textInteraction;
                
                OnDetectItem -= ItemsInteraction_OnDetectItem;
                OnDetectItem += ItemsInteraction_OnDetectItem;
            }
        }

        private void ItemsInteraction_OnDetectItem(object sender, EventArgs e)
        {
            if (!sameItems)
            {
                UpdateOutline();
            }
        }
        

        protected override void Update()
        {
            if (PlayerManager.LocalInstance == null) return;
            
            if (DetectTarget())
            {
                OnDetectItem?.Invoke(this, EventArgs.Empty);
                CheckUserInteraction();
            }
            else if (!sameItems)
            {
                OnUndetectItem?.Invoke(this, EventArgs.Empty);
            }
            else if (_textInteraction.isTextActive && !_doorsDetection.isDetected)
            {
                _textInteraction.Hide();
            }
        }

        protected override bool DetectTarget()
        {
            GameObject newObject = null;
            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] allHits = Physics.SphereCastAll(origin, maxDistanceInteraction, direction, maxDistanceInteraction, mask);

            if (allHits.Length > 0)
            {
                float minAngle = 180;
                Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(MainCamera);
                foreach (RaycastHit hit in allHits)
                {
                    Bounds hitBounds = hit.collider.bounds;
                    if (!GeometryUtility.TestPlanesAABB(cameraFrustum, hitBounds)) continue;

                    Vector3 hitPosition = hit.transform.position;
                    Vector3 directionToTarget = (hitPosition - origin).normalized;

                    float distanceToTarget = Vector3.Distance(transform.position, hitPosition);
                    if (Physics.Raycast(origin, directionToTarget, distanceToTarget, environment)) continue;
                
                    float angle = Vector3.Angle(transform.forward, directionToTarget);
                    if (!(angle < viewAngle / 2)) continue;
                
                    if (minAngle < 180)
                    {
                        if (!(angle < minAngle)) continue;
                
                        newObject = hit.transform.gameObject;
                        minAngle = angle;
                    }
                    else
                    {
                        newObject = hit.transform.gameObject;
                        minAngle = angle;
                    }
                }
            }

            if (newObject == currentItem)
            {
                if (previousItem != currentItem) previousItem = currentItem;
                else if (!currentItem) previousItem = null;
            }
            currentItem = newObject;
            isDetected = (bool)currentItem;
            return isDetected;
        }

        private void UpdateOutline()
        {
            if ((bool)previousItem)
            {
                previousItem.GetComponent<OutlineManager>().enabled = false;
            }

            currentItem.GetComponent<OutlineManager>().enabled = true;
        }

        private void CheckUserInteraction()
        {
            if (ItemsInteraction.PickUp(item))
            {
                OnUndetectItem?.Invoke(this, EventArgs.Empty);
                currentItem = null;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(origin + direction * maxDistanceInteraction, maxDistanceInteraction);
        }
    }
}

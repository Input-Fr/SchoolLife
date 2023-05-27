using System;
using Game;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Items.BatFeatures
{
	public class Bat : ItemFeatures
	{
		private Camera _mainCamera;

		[SerializeField] private GameObject controller;
		[SerializeField] private float attackMaxDistance;
		[SerializeField] private float viewAngle;
		
		[SerializeField] private LayerMask controllerMask;
		[SerializeField] private LayerMask environment;

		private Vector3 origin => controller.transform.position;
		private Vector3 direction => controller.transform.TransformDirection(Vector3.forward);

		private GameObject _currentGameObjectDetected;
		
		public readonly NetworkVariable<bool> IsActive = new();
		
		private void Start()
		{
			transform.parent.gameObject.SetActive(IsActive.Value);
			if (!IsOwner) return;

			if (PlayerManager.LocalInstance != null)
			{
				_mainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
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
				_mainCamera = PlayerManager.LocalInstance.mainCameraGameObject.GetComponent<Camera>();
			}
		}

		private void Update()
		{
			if (!IsOwner || PlayerManager.LocalInstance == null) return;

			if (GameInputs.Instance.Use && DetectController())
			{
				ThirdPersonController currentController = _currentGameObjectDetected.GetComponent<ThirdPersonController>();
				int id = (int)currentController.NetworkObjectId;
				currentController.UpdateMovementsServerRpc(id);
			}
		}

		private bool DetectController()
		{
			GameObject newController = null;
			// ReSharper disable once Unity.PreferNonAllocApi
			RaycastHit[] allHits = Physics.SphereCastAll(origin, attackMaxDistance, direction, attackMaxDistance, controllerMask);

			Debug.Log(allHits.Length);
			if (allHits.Length > 0)
			{
				float minAngle = 180;
				Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
				foreach (RaycastHit hit in allHits)
				{
					Bounds hitBounds = hit.collider.bounds;
					if (GeometryUtility.TestPlanesAABB(cameraFrustum, hitBounds))
					{
						Vector3 hitPosition = hit.transform.position;
						Vector3 directionToTarget = (hitPosition - origin).normalized;

						float distanceToTarget = Vector3.Distance(origin, hitPosition);
						if (!Physics.Raycast(origin, directionToTarget, distanceToTarget, environment))
						{
							float angle = Vector3.Angle(direction, directionToTarget);
							Debug.Log(angle);
							if (angle < viewAngle / 2)
							{
								if (minAngle < 180)
								{
									if (angle < minAngle)
									{
										newController = hit.transform.gameObject;
										minAngle = angle;
									}
								}
								else
								{
									newController = hit.transform.gameObject;
									minAngle = angle;
								}
							}
						}
					}
				}
			}

			_currentGameObjectDetected = newController;

			return _currentGameObjectDetected != null;
		}
	}
}

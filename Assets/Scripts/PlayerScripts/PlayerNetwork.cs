using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerNetwork : NetworkBehaviour
    {
        #region Variables

        [Header("Camera")]
        [SerializeField] private GameObject playerFollowCamera;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject interfaceCanvas;
        [SerializeField] private GameObject subject;
        [SerializeField] private List<Transform> subjectPos;
        private int numberOfSubject;
        
        private const string MenuCameraTag = "MenuCamera";

        #endregion
        
        

        private void Start()
        {
            UpdateComponentsState();
            numberOfSubject = subjectPos.Count;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer || !IsOwner) return;
            for (int i = 0; i < numberOfSubject; i++)
            {
                GameObject instantiated = Instantiate(subject,subjectPos[i].position,Quaternion.identity);
                instantiated.GetComponent<NetworkObject>().Spawn(true);
            }
        }

        private void UpdateComponentsState()
        {
            if (!IsLocalPlayer)
            {
                playerFollowCamera.SetActive(false);
                mainCamera.SetActive(false);
                interfaceCanvas.SetActive(false);
            }
            else
            {
                GameObject menuCamera = GameObject.FindGameObjectWithTag(MenuCameraTag);
                if (menuCamera != null)
                {
                    menuCamera.SetActive(false);
                }
            }
        }
    }
}

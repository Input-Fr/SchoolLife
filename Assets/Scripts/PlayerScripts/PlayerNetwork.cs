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
        [SerializeField] private GameObject subjectPrefab;
        private static GameObject subject;
        private static List<Vector3> subjectPos = new List<Vector3>();
        private static int numberOfSubject;
        
        private const string MenuCameraTag = "MenuCamera";

        #endregion
        
        

        private void Start()
        {
            UpdateComponentsState();
            
        }

        public static void SpawnSubject()
        {
            Debug.Log("Spawned !");
            Debug.Log(numberOfSubject);
            for (int i = 0; i < numberOfSubject; i++)
            {
                GameObject instantiated = Instantiate(subject,subjectPos[i],Quaternion.identity);
                instantiated.GetComponent<NetworkObject>().Spawn(true);
            }
        }
        public override void OnNetworkSpawn()
        {
            if (!IsServer || !IsOwner) return;
            subject = subjectPrefab;
            subjectPos.Add(new Vector3(-47.62702f,23.78214f,22.15781403f));
            subjectPos.Add(new Vector3(-47.62702f,23.78214f,28.95f));
            subjectPos.Add(new Vector3(-47.62702f,23.78214f,34.32f));
            numberOfSubject = subjectPos.Count;
            SpawnSubject();
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

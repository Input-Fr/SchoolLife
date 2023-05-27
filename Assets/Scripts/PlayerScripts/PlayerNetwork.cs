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
        
        private const string MenuCameraTag = "MenuCamera";

        #endregion
        
        

        private void Start()
        {
            UpdateComponentsState();
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

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Timer : NetworkBehaviour
    {
        [SerializeField] private GameObject timerBackGround;
        [SerializeField] private Text timerText;
        private void Start()
        {
            timerBackGround.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            int minutes = 10;
            int seconds = minutes * 60;
            StartTimer(seconds);
        }

        private void StartTimer(int duration)
        {
            SetActiveTextClientRpc(true);
            StartCoroutine(Pause(duration));
        }

        private IEnumerator Pause(int duration)
        {
            foreach ((int remainingSeconds, WaitForSeconds waitForSeconds) T in Count.CountDown(duration))
            {
                int minutes = T.remainingSeconds / 60;
                string strMinutes = minutes < 10 ? $"0{minutes}" : $"{minutes}";

                int seconds = T.remainingSeconds % 60;
                string strSeconds = seconds < 10 ? $"0{seconds}" : $"{seconds}";
                
                string newText = $"{strMinutes}:{strSeconds}";
                SetTextClientRpc(newText);
                
                yield return T.waitForSeconds;
            }
            
            SetActiveTextClientRpc(false);
        }

        [ClientRpc]
        private void SetActiveTextClientRpc(bool active)
        {
            timerBackGround.SetActive(active);
        }

        [ClientRpc]
        private void SetTextClientRpc(string newText)
        {
            if (!timerBackGround.activeSelf)
            {
                timerBackGround.SetActive(true);
            }
            
            timerText.text = newText;
        }
    }
}

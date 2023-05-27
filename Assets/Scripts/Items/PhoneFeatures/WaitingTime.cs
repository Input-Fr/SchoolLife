using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Items.PhoneFeatures
{
    public class WaitingTime : MonoBehaviour
    {

        #region Variable

        public bool isFinish { get; private set; } = true;

        private Image _phoneWaitingImage;

        #endregion

        private void Awake()
        {
            _phoneWaitingImage = GetComponent<Image>();
        }

        public void UpdateTimer(int duration)
        {
            StartCoroutine(Waiting(duration));
        }

        private IEnumerator Waiting(int duration)
        {
            isFinish = false;
            foreach ((int remainingSeconds, WaitForSeconds waitForSeconds) T in Count.CountDown(duration))
            {
                _phoneWaitingImage.fillAmount = Mathf.InverseLerp(0, duration, T.remainingSeconds);
                yield return T.waitForSeconds;
            }

            isFinish = true;
        } 
    }
}

using Unity.Netcode;
using UnityEngine;

namespace Tasks.Task_1
{
    public class Switch : NetworkBehaviour
    {
        [SerializeField] private GameObject up;
        [SerializeField] private GameObject on;

        private bool _isOn;
        private bool _isUp;

        public SwitchTask switchTask;

        private void Start()
        {
            int temp = Random.Range(0, 10);
            int temp2 = Random.Range(0, 2);
            
            _isOn = temp > 8;
            _isUp = temp2 == 1;
            
            on.SetActive(_isOn);
            up.SetActive(_isUp);
            
            if (_isOn)
            {
                switchTask.SwitchChange(1);
            }
        }

        public void SwitchActionButton()
        {
            _isUp = !_isUp;
            _isOn = !_isOn;
            
            on.SetActive(_isOn);
            up.SetActive(_isUp);
            
            if (_isOn)
            {
                switchTask.SwitchChange(1);
            }
            else
            {
                switchTask.SwitchChange(-1);
            }
        }

        public void ResetTask()
        {
            int temp = Random.Range(0,10);
            int temp2 = Random.Range(0,2);

            _isOn = temp > 8;
            _isUp = temp2 == 1;
            
            on.SetActive(_isOn);
            up.SetActive(_isUp);
            
            if (_isOn)
            {
                Debug.Log($"SWITCH TASK BUG : {switchTask != null}");
                switchTask.SwitchChange(1);
            }
        }
    }
}

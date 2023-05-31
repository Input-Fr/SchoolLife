using Interface;
using PlayerScripts;
using UnityEngine;

namespace Tasks.Task_1
{
    public class SwitchTask : TaskManager
    {
        private const int SwitchCount = 12;
        private static int _numberSwitchesOn;

        public void SwitchChange(int points)
        {
            _numberSwitchesOn += points;
            if (_numberSwitchesOn == SwitchCount)
            {
                isTaskDone = true;
                task.SetActive(false);

                _numberSwitchesOn = 0;
                
                Rewards();

                foreach (Transform t in task.transform)
                {
                    t.GetComponent<Switch>().ResetTask();
                }
            }
        }
    }
}
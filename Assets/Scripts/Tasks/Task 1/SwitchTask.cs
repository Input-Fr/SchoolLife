using Interface;
using PlayerScripts;
using UnityEngine;

namespace Tasks.Task_1
{
    public class SwitchTask : TaskManager
    {
        private const int PointToAdd = 2;
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
                if (buttonType is ButtonType.StealTask)
                {
                    AddItemToInventory();
                }
                else
                {
                    HUDSystem hudSystem = PlayerManager.LocalInstance.hudSystem;
                    hudSystem.points += PointToAdd;
                    if (hudSystem.points >= 20)
                    {
                        hudSystem.points = 20;
                    }
                }

                foreach (Transform t in task.transform)
                {
                    t.GetComponent<Switch>().ResetTask();
                }
            }
        }
    }
}
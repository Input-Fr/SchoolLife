using System;
using Game;
using Interface;
using PlayerScripts;
using UnityEngine;

namespace Tasks
{
    public class TaskState : MonoBehaviour
    {
        public TaskManager taskManager;

        private GameInputs _gameInputs;
        private Pause _pause;
    
        private bool _isTaskDone;
    
        private void Start()
        {
            gameObject.SetActive(false);
            
            if (PlayerManager.LocalInstance != null)
            {
                _gameInputs = PlayerManager.LocalInstance.gameInput;
                _pause = PlayerManager.LocalInstance.pause;
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;   
            }

            _isTaskDone = true;
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance != null)
            {
                _gameInputs = PlayerManager.LocalInstance.gameInput;
                _pause = PlayerManager.LocalInstance.pause;
            }
        }
    
        private void OnEnable()
        {
            if (_gameInputs)
            {
                _pause.canLockCursor = false;
                _gameInputs.inInterface = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnDisable()
        {
            if (_gameInputs)
            {
                Cursor.lockState = CursorLockMode.Locked;
                _gameInputs.inInterface = false;
                _pause.canLockCursor = true;
                if (_isTaskDone)
                {
                    taskManager.GetComponent<AudioSource>().Play();
                }
            }
        }
    }
}

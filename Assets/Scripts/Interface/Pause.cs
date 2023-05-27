using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class Pause : MonoBehaviour
    {
        [SerializeField]private Button pause;
        [SerializeField]private Button resume;
        [SerializeField]private Button disconnect;
        [SerializeField]private Button quit;

        public GameObject pausePanel;

        private void Awake()
        {
            pause.onClick.AddListener(() => { 
                pausePanel.SetActive(true);
            
            });
            resume.onClick.AddListener(() => { 
                pausePanel.SetActive(false);
            
            });
            disconnect.onClick.AddListener(() =>
            {
                
            });
            quit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}

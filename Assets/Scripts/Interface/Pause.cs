using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
	public class Pause : MonoBehaviour
	{
		[SerializeField]private Button resume;
		[SerializeField]private Button quit;

		public bool canLockCursor = true;

		public GameObject pausePanel;

		private void Awake()
		{
			resume.onClick.AddListener(() =>
			{
				UpdatePauseState(false);
			});
            
			quit.onClick.AddListener(Application.Quit);
		}

		private void Update()
		{
			if (!Input.GetKeyDown(KeyCode.Escape)) return;

			UpdatePauseState(true);
		}

		private void UpdatePauseState(bool newState)
		{
			Cursor.lockState = newState || !canLockCursor ? CursorLockMode.None : CursorLockMode.Locked;
			pausePanel.SetActive(newState);
		}
	}
}
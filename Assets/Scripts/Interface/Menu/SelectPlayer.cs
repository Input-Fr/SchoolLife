using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menu
{
	public class SelectPlayer : MonoBehaviour
	{
		public static int Id;
		
		[SerializeField] private Button selectPlayerButton;
		[SerializeField] private int id;

		private void Awake()
		{
			selectPlayerButton.onClick.AddListener(() =>
			{
				Id = id;
			});
		}
	}
}

using System;
using PlayerScripts;
using TMPro;
using UnityEngine;

namespace Interface
{
	public class HUDSystem : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI quantityMoney;
		[SerializeField] private TextMeshProUGUI quantityPoint;
		
		private int _points = 10;
		public int points
		{
			get => _points;
			set
			{
				_points = value;
				quantityPoint.text = _points.ToString();
			}
		}

		private int _wealth = 10;
		public int wealth
		{
			get => _wealth;
			set
			{
				_wealth = value;
				quantityMoney.text = _wealth.ToString();
			}
		}
		
		private bool _isDone;
		private const int SommeMax = 1000;

		private void Start()
		{
			if (PlayerManager.LocalInstance != null)
			{
				wealth = 10;
				points = 10;
			}
			else
			{
				PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
			}
		}

		private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
		{
			if (PlayerManager.LocalInstance != null)
			{
				wealth = 10;
				points = 10;
			}
		}

		public void UpdateHUD()
		{
			foreach (var slot in PlayerManager.LocalInstance.inventoryManager.inventorySlots)
			{
				if (slot.itemInSlot != null && slot.itemInSlot.itemData.name == "Subject" && points != 20)
				{
					points = 20;
					PlayerManager.LocalInstance.inventoryManager.UseSubject(slot);
				}
			}
			wealth += (int)(points / 20f * SommeMax);
			points = 0;
			_isDone = true;
		}
	}
}

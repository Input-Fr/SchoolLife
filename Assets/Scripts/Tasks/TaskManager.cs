using System;
using Interface;
using Interface.Inventory;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
	public class TaskManager : NetworkBehaviour
	{
		private const int PointToAdd = 2;
		
		[SerializeField] protected ButtonType buttonType;
		[SerializeField] private ItemData item;
		
		[SerializeField] public int taskIndex;
		[SerializeField] protected GameObject task;

		public bool isTaskDone;

		private InventoryManager _inventoryManager;

		private void Start()
		{
			PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
		}

		private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
		{
			if (PlayerManager.LocalInstance != null)
			{
				_inventoryManager = PlayerManager.LocalInstance.inventoryManager;
				task = PlayerManager.LocalInstance.allTasksUI[taskIndex];
			}
		}

		protected void Rewards()
		{
			if (buttonType is ButtonType.StealTask)
			{
				_inventoryManager.AddItem(item);
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
		}

		protected enum ButtonType
		{
			StealTask,
			ClassTask
		}
	}
}

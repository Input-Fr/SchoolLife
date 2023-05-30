using System;
using Interface.Inventory;
using Items;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
	public class TaskManager : NetworkBehaviour
	{
		[SerializeField] protected ButtonType buttonType;
		[SerializeField] private ItemData item;
		
		[SerializeField] public int taskIndex;
		[SerializeField] protected GameObject task;

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

		protected void AddItemToInventory()
		{
			_inventoryManager.AddItem(item);
		}

		protected enum ButtonType
		{
			StealTask,
			ClassTask
		}
	}
}

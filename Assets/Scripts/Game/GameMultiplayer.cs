using System.Collections;
using Items;
using PlayerScripts;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
	public class GameMultiplayer : NetworkBehaviour
	{
		public static GameMultiplayer Instance { get; private set; }

		private const string BuildingCTag = "BuildingC";

		private void Awake()
		{
			Instance = this;
		}

		public void InstantiateItem(ItemData itemData, Vector3 position, Quaternion rotation)
		{
			InstantiateItemServerRpc(itemData.id, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);
		}

		[ServerRpc(RequireOwnership = false)]
		private void InstantiateItemServerRpc(int index, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float rotationW)
		{
			GameObject itemPrefab = PlayerManager.LocalInstance.itemDataArray[index].prefabInScene;
			Vector3 position = new Vector3(positionX, positionY, positionZ);
			Quaternion rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
			
			GameObject instantiatedItem = Instantiate(itemPrefab, position, rotation);
			instantiatedItem.GetComponent<NetworkObject>().Spawn(true);
		}
	}
}

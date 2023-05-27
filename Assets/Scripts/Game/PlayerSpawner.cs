using Interface.Menu;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
	public class PlayerSpawner : NetworkBehaviour
	{
		[SerializeField] private GameObject[] playerPrefabs;

		public override void OnNetworkSpawn()
		{
			SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, SelectPlayer.Id);
		}

		[ServerRpc(RequireOwnership = false)]
		private void SpawnPlayerServerRpc(ulong clientId, int index)
		{
			GameObject newPlayer = Instantiate(playerPrefabs[index]);
			newPlayer.SetActive(true);
			newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);  
		}
	}
}

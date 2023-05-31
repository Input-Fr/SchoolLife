using System;
using System.Collections;
using Door;
using Game;
using Interface;
using Interface.Inventory;
using Interface.Inventory.Tooltips;
using Items;
using Tasks.Task_1;
using TMPro;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
	public class PlayerManager : NetworkBehaviour
	{
		#region Variables

		public static PlayerManager LocalInstance;
		public static string Code;
		
		[Header("Items")] 
		[SerializeField] public ItemData[] itemDataArray;
		
		[Header("Player")]
		[SerializeField] private Transform transformTp;
		
		[Header("Static Access")]
		[SerializeField] public ThirdPersonController thirdPersonController;
		[SerializeField] public GameObject mainCameraGameObject;
		[SerializeField] public DoorsDetection doorsDetection;
		[SerializeField] public ItemsDetection itemsDetection;
		[SerializeField] public InventoryManager inventoryManager;
		[SerializeField] public InventoryState inventoryState;
		[SerializeField] public TextInteraction textInteraction;
		[SerializeField] public Tooltip tooltip;
		[SerializeField] public GameObject caught;
		[SerializeField] public GameObject shop;
		[SerializeField] public GameObject pauseMenu;
		[SerializeField] public Pause pause;
		[SerializeField] public TextMeshProUGUI textCode;
		[SerializeField] public HUDSystem hudSystem;
		[SerializeField] public PuzzleManager puzzleManager;
		[SerializeField] private bool cursorLocked = true;
		[SerializeField] public GameObject[] allTasksUI;

		private const string BuildingCTag = "BuildingC";

		public GameObject SoundReplication;
		private GameObject instantiation;
		public GameInputs gameInput;
		public static event EventHandler OnAnyPlayerSpawn;

		#endregion

		public override void OnNetworkSpawn()
		{
			textCode.text = "GAME CODE : " + Code;
			
			if (IsOwner)
			{
				gameInput = GameObject.FindWithTag("InputsManager").GetComponent<GameInputs>();
				LocalInstance = this;
				thirdPersonController.gameObject.layer = LayerMask.NameToLayer("Default");
			}
			
			OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);
		}

		private void Start()
		{
			if (!IsOwner) return;

			Cursor.lockState = cursorLocked?CursorLockMode.Locked:CursorLockMode.None;
			
			NavMeshSurface surface = GameObject.FindGameObjectWithTag(BuildingCTag).GetComponent<NavMeshSurface>();
			surface.BuildNavMesh();
		}
		public async void playSoundNetwork()
		{
			var temp = transform.position;
			InstantiateSoundServerRpc(temp.x,temp.y,temp.z);
		}
		[ServerRpc(RequireOwnership = false)]
		private void InstantiateSoundServerRpc(float x,float y,float z)
		{
			GameObject itemPrefab = SoundReplication;
			Vector3 position = new Vector3(x,y,z);
			Quaternion rotation = new Quaternion(0,0,0,0);
            
			GameObject instantiatedItem = Instantiate(itemPrefab, position, rotation);
			instantiatedItem.GetComponent<NetworkObject>().Spawn(true);
			instantiation = instantiatedItem;
			StartCoroutine(waitAndDestroy());

		}
		IEnumerator waitAndDestroy()
		{
			yield return new WaitForSeconds(instantiation.GetComponent<AudioSource>().clip.length);
			instantiation.GetComponent<NetworkObject>().Despawn();
		}
		
		[ClientRpc]
		public void CatchPlayerClientRpc(int id)
		{
			if (id != (int)NetworkObjectId) return;

			thirdPersonController.enabled = false;
			thirdPersonController.transform.position = transformTp.position;
			thirdPersonController.enabled = true;
			
			caught.SetActive(true);
		}
	}
}

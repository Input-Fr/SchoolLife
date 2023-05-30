using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game
{
	public class GameInputs : MonoBehaviour
	{
		#region Variables

		public static GameInputs Instance { get; private set; }

		private PlayerInputActions _playerInputActions;
		private PlayerInputActions.PlayerActions _playerActions;

		private Vector2 _move;
		public Vector2 Move
		{
			get => inInterface ? Vector2.zero : _move;
			private set => _move = value;
		}

		private Vector2 _look;
		public Vector2 Look
		{
			get => _look;
			private set => _look = inInterface ? Vector2.zero : value;
		}

		private bool _jump;
		public bool Jump
		{
			get => !inInterface && _jump;
			private set => _jump = value;
		}

		private bool _sprint;

		public bool Sprint
		{
			get => inInterface ? false : _sprint;
			private set => _sprint = value;
		}
		
		public bool Use { get; private set; }
		public bool Interact { get; private set; }
		public int SelectSlot { get; private set; }
		public bool LockCamera { get; private set; }
		public bool ChangeInventoryState { get; private set; }

		private const int KeyboardIndex = 0;
		public string UseKey { get; private set; }
		public string InteractKey { get; private set; }

		public bool inInterface;

		#endregion

		private void Awake()
		{
			Instance = this;

			_playerInputActions = new PlayerInputActions();
			_playerActions = _playerInputActions.Player;

			InteractKey = _playerActions.Interact.bindings[KeyboardIndex].path.Last().ToString().ToUpper();
			UseKey = _playerActions.Use.bindings[KeyboardIndex].path.Last().ToString().ToUpper();
			//SelectSlotKey = _playerActions.SelectSlot.bindings[KeyboardIndex].path.Last().ToString().ToUpper();
			//ChangeInventoryStateKey = _playerActions.ChangeInventoryState.bindings[KeyboardIndex].path.Last().ToString().ToUpper();
			//LockCameraKey = _playerActions.LockCamera.bindings[KeyboardIndex].path.Last().ToString().ToUpper();
		}

		private void OnEnable()
		{
			_playerActions.Jump.started += OnJumpStarted;
			_playerActions.Jump.canceled += OnJumpCanceled;
			
			_playerActions.Use.started += OnUseItemStarted;
			_playerActions.Use.canceled += OnUseItemCanceled;
			
			_playerActions.Interact.started += OnInteractStarted;
			_playerActions.Interact.canceled += OnInteractCanceled;
			
			_playerActions.LockCamera.started += OnLockCameraStarted;
			_playerActions.LockCamera.canceled += OnLockCameraCanceled;

			_playerActions.ChangeInventoryState.started += OnInventoryChangeStateStarted;
			_playerActions.ChangeInventoryState.canceled += OnInventoryChangeStateCanceled;

			_playerActions.Enable();
		}

		private void OnDisable()
		{
			_playerActions.Disable();

			_playerActions.Jump.started -= OnJumpStarted;
			_playerActions.Jump.canceled -= OnJumpCanceled;
			
			_playerActions.Use.started -= OnUseItemStarted;
			_playerActions.Use.canceled -= OnUseItemCanceled;
			
			_playerActions.Interact.started -= OnInteractStarted;
			_playerActions.Interact.canceled -= OnInteractCanceled;

			_playerActions.LockCamera.started -= OnLockCameraStarted;
			_playerActions.LockCamera.canceled -= OnLockCameraCanceled;
			
			_playerActions.ChangeInventoryState.started -= OnInventoryChangeStateStarted;
			_playerActions.ChangeInventoryState.canceled -= OnInventoryChangeStateCanceled;
		}

		public void OnMove(InputValue value) { Move = value.Get<Vector2>(); }
		
		public void OnLook(InputValue value) { Look = value.Get<Vector2>(); }
		
		public void OnSprint(InputValue value) { Sprint = value.isPressed; }

		public void OnSelectSlot(InputValue value) { SelectSlot = (int)value.Get<float>(); }
		 
		public void OnLockCamera(InputValue value) { LockCamera = value.isPressed; }
		
		private void OnJumpStarted(InputAction.CallbackContext context) { Jump = true; }
		private void OnJumpCanceled(InputAction.CallbackContext context) { Jump = false; }
		
		private void OnUseItemStarted(InputAction.CallbackContext context) { Use = true; }
		private void OnUseItemCanceled(InputAction.CallbackContext context) { Use = false; }

		private void OnInteractStarted(InputAction.CallbackContext context) { Interact = true; }
		private void OnInteractCanceled(InputAction.CallbackContext context) { Interact = false; }
		
		private void OnLockCameraStarted(InputAction.CallbackContext context) { LockCamera = true; }
		private void OnLockCameraCanceled(InputAction.CallbackContext context) { LockCamera = false; }

		private void OnInventoryChangeStateStarted(InputAction.CallbackContext context) { ChangeInventoryState = true; }
		private void OnInventoryChangeStateCanceled(InputAction.CallbackContext context) { ChangeInventoryState = false; }

		public void ResetJumpInput() { Jump = false; }
		public void ResetChangeInventoryStateInput() { ChangeInventoryState = false; }
		public void ResetInteractInput() { Interact = false; }
		public void ResetUseInput() { Use = false; }
	}
	
}
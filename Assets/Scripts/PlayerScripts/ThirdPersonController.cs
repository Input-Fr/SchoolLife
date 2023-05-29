using System;
using System.Collections;
using Game;
using Interface.Inventory;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerScripts
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : NetworkBehaviour
    {
        [SerializeField] private GameObject filter;
        
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController controller;
        [SerializeField] private GameObject mainCamera;
        
        [Header("Player")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)] [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;
        
        [Space(10)]
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [Range(0, 1)] [SerializeField] private float footstepAudioVolume = 0.5f;
        [SerializeField] private AudioClip songToPlay;

        [Space(10)]
        [SerializeField] private float jumpHeight = 1.2f; 
        [SerializeField] private float gravity = -15.0f;

        [Space(10)]
        [SerializeField] private float jumpTimeout = 0.50f; 
        [SerializeField] private float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [SerializeField] private float groundedOffset = -0.14f; 
        [SerializeField] private float groundedRadius = 0.28f; 
        [SerializeField] private LayerMask groundLayers;
        private bool grounded {
            get
            {
                Vector3 position = transform.position;
                Vector3 spherePosition = new Vector3(position.x, position.y - groundedOffset, position.z);
                return Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
            }
        }

        [Header("Cinemachine")]
        [SerializeField] private GameObject cinemachineCameraTarget; 
        [SerializeField] private float lookSensitivity = 1f; 
        [SerializeField] private float topClamp = 70.0f; 
        [SerializeField] private float bottomClamp = -30.0f;
        [SerializeField] private float cameraAngleOverride;

        private bool lockCameraPosition => _inventoryState.isOpen || GameInputs.Instance.LockCamera ||  PlayerManager.LocalInstance.shop.activeInHierarchy || PlayerManager.LocalInstance.caught.activeInHierarchy || PlayerManager.LocalInstance.pauseMenu.activeInHierarchy;

        public bool canBeAttacked = true;
        private bool _isKnockedOut;
        private bool isKnockedOut
        { 
            get => _isKnockedOut; 
            set 
            {
                _isKnockedOut = value;
                filter.SetActive(value); 
            }
        }

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private const float TerminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        
        private GameInputs _gameInputs;
        private InventoryState _inventoryState;

        private const float Threshold = 0.01f;
        
        private void Start()
        {
            if (!IsOwner) return;
            
            if (PlayerManager.LocalInstance != null)
            {
                _inventoryState = PlayerManager.LocalInstance.inventoryState;
                _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
                AssignAnimationIDs();
                ResetTimeouts();
            }
            else
            {
                PlayerManager.OnAnyPlayerSpawn += PlayerManager_OnAnyPlayerSpawn;
            }
            
            _gameInputs = GameInputs.Instance;
        }

        private void PlayerManager_OnAnyPlayerSpawn(object sender, EventArgs e)
        {
            if (PlayerManager.LocalInstance != null)
            {
                _inventoryState = PlayerManager.LocalInstance.inventoryState;
                _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
                AssignAnimationIDs();
                ResetTimeouts();
            }
        }

        private void Update()
        {
            if (!IsOwner || PlayerManager.LocalInstance == null) return;
            
            JumpAndGravity();
            AnimatorCheck();
            Move();
        }

        private void LateUpdate()
        {
            if (!IsOwner || PlayerManager.LocalInstance == null) return;
            
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void ResetTimeouts()
        {
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        public void AnimatorCheck()
        { 
            animator.SetBool(_animIDGrounded, grounded);
        }

        private void CameraRotation()
        {
            if (_gameInputs.Look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _cinemachineTargetYaw += _gameInputs.Look.x * lookSensitivity;
                _cinemachineTargetPitch += _gameInputs.Look.y * lookSensitivity;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
        
            float targetSpeed = _gameInputs.Sprint && !isKnockedOut ? sprintSpeed : moveSpeed;

            if (_gameInputs.Move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            Vector3 velocity = controller.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            Vector3 inputDirection = new Vector3(_gameInputs.Move.x, 0.0f, _gameInputs.Move.y).normalized; 
            if (_gameInputs.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            animator.SetFloat(_animIDSpeed, _animationBlend);
            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        
            
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout; 
            
                animator.SetBool(_animIDJump, false);
                animator.SetBool(_animIDFreeFall, false);

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_gameInputs.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); 
                    animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            { 
                _jumpTimeoutDelta = jumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    animator.SetBool(_animIDFreeFall, true);
                }
            
                _gameInputs.ResetJumpInput();
            }

            if (_verticalVelocity < TerminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
            
            
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.color = grounded ? transparentGreen : transparentRed;

            Vector3 position = transform.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - groundedOffset, position.z), groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f) || footstepAudioClips.Length <= 0) return;
            
            int index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), footstepAudioVolume);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(controller.center), footstepAudioVolume);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateMovementsServerRpc(int id)
        {
            UpdateMovementsClientRpc(id);
        }

        [ClientRpc]
        private void UpdateMovementsClientRpc(int id)
        {
            if (id != (int)NetworkObjectId || !canBeAttacked) return;
            
            int isTouched = Random.Range(0, 3);

            if (isTouched == 0)
            {
                StartCoroutine(UpdateMovementsIEnumerator());
            }
        }

        private IEnumerator UpdateMovementsIEnumerator()
        {
            canBeAttacked = false;
            isKnockedOut = true;
            PlayerManager.LocalInstance.playSoundNetwork();

            yield return new WaitForSeconds(3f);

            isKnockedOut = false;
            canBeAttacked = true;
        }
    }
}
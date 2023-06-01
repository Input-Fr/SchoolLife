using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using PlayerScripts;
using Unity.Netcode;

namespace Professors
{
    public class AIProfessor : NetworkBehaviour, IHear
    {
        #region Variables

        [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask obstacleMask;
        
        [Space(10)]
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [Range(0, 1)] [SerializeField] private float footstepAudioVolume = 0.5f;
        [SerializeField] private AudioClip songToPlay;

        [Header("Statistics")]
        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float chaseSpeed = 3f;
        [SerializeField] private float walkViewRadius = 6f;
        [SerializeField] private float chaseViewRadius = 6f;
        [SerializeField] private float walkViewAngle = 200f;
        [SerializeField] private float chaseViewAngle = 360f;
        [SerializeField] private float distancePlayerCatch = 1.5f;
        [SerializeField] private float speedChangeRate = 10.0f;

        [Header("Wandering parameters")]
        [SerializeField] private float wanderingWaitTimeMin = 0.5f;
        [SerializeField] private float wanderingWaitTimeMax = 3f;
        [SerializeField] private float wanderingDistanceMin = 5f;
        [SerializeField] private float wanderingDistanceMax = 8f;

        [SerializeField] private Transform destinations;

        [Header("Currents")]
        private const string PlayerTag = "Player";

        private GameObject _player;
        private float _viewRadius;
        private float _viewAngle;

        private bool _isPurchasing;
        private bool _isFollowingSong;
        private bool _isInView;
        private bool _wasInView;
        private bool _isCatch;
        private bool _wasCatch;
        private bool _hasDestination;

        private float _animationBlend;
        
        private int _animIDSpeed;
        private int _animIDGrounded;
        //private int _animIDJump;
        //private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        #endregion

        private void Awake()
        {
            AssignAnimationIDs();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            //_animIDJump = Animator.StringToHash("Jump");
            //_animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _viewRadius = walkViewRadius;
                _viewAngle = walkViewAngle;
                SetAgentSpeedClientRpc(walkSpeed);
            }
            else
            {
                StartPositionServerRpc((int)NetworkObjectId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartPositionServerRpc(int id)
        {
            Vector3 professorServerPosition = transform.position;
            Vector3 destination = agent.destination;
            StartPositionClientRpc(id, agent.speed, agent.isStopped, professorServerPosition.x, professorServerPosition.y, professorServerPosition.z, destination.x, destination.y, destination.z);
        }

        [ClientRpc]
        private void StartPositionClientRpc(int id, float speed, bool isStopped, float pX, float pY, float pZ, float dX, float dY, float dZ)
        {
            if (id != (int)NetworkObjectId) return;

            agent.speed = speed;
            agent.isStopped = isStopped;
            transform.position = new Vector3(pX, pY, pZ);
            Vector3 destination = new Vector3(dX, dY, dZ);
            agent.SetDestination(destination);
        }

        private void Update()
        {
            if (!IsServer) return;
            
            UpdateMovements();
            UpdateAnimations();
        }

        private void UpdateMovements()
        {
            if (GetPlayer())
            {
                _isPurchasing = true;
                if (_isCatch)
                {
                    CatchPlayer();
                }
                else
                {
                    MoveTo(_player.transform.position, chaseViewRadius, chaseViewAngle, chaseSpeed);
                }
            }
            else if (_isFollowingSong)
            {
                if (!agent.hasPath)
                {
                    _isFollowingSong = false;
                }
            }
            else
            {
                _isPurchasing = false;
                Npc();
            }
        }

        private void UpdateAnimations()
        {
            float targetSpeed = agent.hasPath ? agent.speed : 0f;
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            float inputMagnitude = 1f;

            animator.SetBool(_animIDGrounded, true);
            animator.SetFloat(_animIDSpeed, _animationBlend);
            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        private bool GetPlayer()
        {
            GameObject[] playersGameObject = GameObject.FindGameObjectsWithTag(PlayerTag);

            GameObject newPlayer = null;
            bool isCatch = false;
            bool isInView = false;

            _wasInView = _isInView;
            _wasCatch = _isCatch;

            foreach (GameObject playerGameObject in playersGameObject)
            {
                Vector3 position = transform.position + new Vector3(0, 1f, 0);

                Vector3 playerBodyPosition = playerGameObject.transform.position + new Vector3(0, 1f, 0);
                Vector3 playerHeadPosition = playerBodyPosition + new Vector3(0, 0.6f, 0);

                if (!(Vector3.Distance(position, playerBodyPosition) < _viewRadius)) continue;

                Vector3 directionToPlayer = (playerBodyPosition - position).normalized;
                if (!(Vector3.Angle(transform.forward, directionToPlayer) < _viewAngle / 2)) continue;

                float distanceToPlayer = Vector3.Distance(position, playerHeadPosition);
                
                Debug.Log("Before");
                if (Physics.Raycast(position, directionToPlayer, distanceToPlayer, obstacleMask)) continue;
                Debug.Log("AFter");

                if (newPlayer != null)
                {
                    if (Vector3.Distance(position, playerBodyPosition) < Vector3.Distance(position,
                            newPlayer.transform.position + new Vector3(0, 1f, 0)))
                    {
                        newPlayer = playerGameObject;
                    }
                }
                else
                {
                    newPlayer = playerGameObject;
                }

                if (Vector3.Distance(position, newPlayer.transform.position + new Vector3(0, 1f, 0)) <
                    distancePlayerCatch)
                {
                    isCatch = true;
                }

                isInView = true;
            }

            _player = newPlayer;
            _isInView = isInView;
            _isCatch = isCatch;

            return _isInView;
        }

        private void CatchPlayer()
        {
            if (_isFollowingSong) _isFollowingSong = false;

            if (_wasCatch) return;
            
            _viewRadius = chaseViewRadius;
            _viewAngle = chaseViewAngle;
            
            PlayerManager playerManager = _player.GetComponentInParent<PlayerManager>();
            playerManager.CatchPlayerClientRpc((int)playerManager.NetworkObjectId);
            
            SetAgentSpeedClientRpc(chaseSpeed);
            SetAgentIsStoppedClientRpc(true);
        }

        private void MoveTo(Vector3 position, float viewRadius, float viewAngle, float speed)
        {
            if (!_wasInView || (_wasInView && _wasCatch))
            {
                _viewRadius = viewRadius;
                _viewAngle = viewAngle;
                SetAgentSpeedClientRpc(speed);
                SetAgentIsStoppedClientRpc(false);
            }

            SetAgentDestinationClientRpc(position.x, position.y, position.z);
        }

        private void Npc()
        {
            if (!agent.hasPath && !_hasDestination)
            {
                _viewRadius = walkViewRadius;
                _viewAngle = walkViewAngle;
                SetAgentSpeedClientRpc(walkSpeed);

                float delay = Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax);
                StartCoroutine(GoToDestination(delay));
            }
        }
    
        private IEnumerator GoToDestination(float delay)
        {
            _hasDestination = true;
            yield return new WaitForSeconds(delay);

            Vector3 destination = destinations.GetChild(Random.Range(0, destinations.childCount)).position;
            SetAgentDestinationClientRpc(destination.x, destination.y, destination.z);
            _hasDestination = false;
        }


        [ClientRpc]
        private void SetAgentSpeedClientRpc(float newSpeed)
        {
            agent.speed = newSpeed;
        }

        [ClientRpc]
        private void SetAgentIsStoppedClientRpc(bool isStopped)
        {
            agent.isStopped = isStopped;
            if (isStopped)
            {
                agent.ResetPath();
            }
        }

        [ClientRpc]
        private void SetAgentDestinationClientRpc(float x, float y, float z)
        { 
            agent.SetDestination(new Vector3(x, y, z));
        }
        
    
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position + new Vector3(0, 1f, 0);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(origin, _viewRadius);

            Gizmos.color = _isCatch ? Color.red : Color.black;
            Gizmos.DrawWireSphere(origin, distancePlayerCatch);
        
            Vector3 viewAngleA = DirFromAngle(-_viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(_viewAngle / 2, false);
        
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + viewAngleA * _viewRadius);
            Gizmos.DrawLine(origin, origin + viewAngleB * _viewRadius);

            if (_isInView)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, _player.transform.position + new Vector3(0, 1f, 0));
            }

            if (agent.hasPath)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(origin, agent.destination);
            }
        }

        public void RespondToSound(Sound sound)
        {
            Debug.Log("I Hear");
            if (_isPurchasing) return;

            switch (sound.GetSoundType())
            {
                case SoundType.Interesting:
                    MoveTo(sound.Position, walkViewRadius, walkViewAngle, walkSpeed);
                    break;
                case SoundType.Alerting:
                    MoveTo(sound.Position, chaseViewRadius, chaseViewAngle, chaseSpeed);
                    break;
                case SoundType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _isFollowingSong = true;
        }
        
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f) || footstepAudioClips.Length <= 0) return;
            
            int index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.position, footstepAudioVolume);
        }
    }
}

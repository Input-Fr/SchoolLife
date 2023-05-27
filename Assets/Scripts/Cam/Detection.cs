using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Cam
{
    public abstract class Detection : NetworkBehaviour
    {
        #region Variables

        protected Camera MainCamera;
    
        [SerializeField] protected LayerMask mask;
        [SerializeField] protected float maxDistanceInteraction;

        protected Vector3 origin => transform.position;
        protected Vector3 direction => transform.TransformDirection(Vector3.forward);

        public bool isDetected { get; protected set; }

        #endregion

        protected abstract void Update();
        
        protected abstract bool DetectTarget();
    }
}

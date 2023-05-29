using Game;
using PlayerScripts;
using Unity.Netcode;
using UnityEngine;

namespace Items.GlassesFeatures
{
    public class Glasses : ItemFeatures
    {
        [SerializeField] private GlassesManager glassesManager;

        public readonly NetworkVariable<bool> IsActive = new();

        private bool _isInUse;

        private void Start()
        {
            transform.parent.gameObject.SetActive(IsActive.Value);
        }

        private void Update()
        {
            if (!PlayerManager.LocalInstance) return; 

            if (!GameInputs.Instance.Use || _isInUse) return;

            GameInputs.Instance.ResetInteractInput();

            if (!_isInUse)
            {
                _isInUse = true;
                glassesManager.UseGlasses();
            }
        }
    }
}
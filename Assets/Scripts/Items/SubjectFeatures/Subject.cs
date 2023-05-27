using Items;
using Unity.Netcode;

namespace Items.SubjectFeatures
{
    public class Subject : ItemFeatures
    {
        public readonly NetworkVariable<bool> IsActive = new();

        private void Start()
        {
            transform.parent.gameObject.SetActive(IsActive.Value);
        }
    }
}
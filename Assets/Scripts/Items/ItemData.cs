using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Scriptable object/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Information")]
        public int id;        
        public string itemName;
        public string description;
        public Sprite image;
        
        [Header("References")]
        public GameObject prefabInScene;
        public GameObject prefabInHand;
        public GameObject inventoryItemPrefab;

        [Header("Inventory")]
        public bool stackable = true;
        public bool multiple = true;
        public uint numberItemByStack;

        private void Awake()
        {
            if (multiple || stackable) return;

            numberItemByStack = 1;
            stackable = false;
        }
    }
}

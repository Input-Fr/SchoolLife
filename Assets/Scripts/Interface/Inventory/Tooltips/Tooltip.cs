using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Inventory.Tooltips
{
    public class Tooltip : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LayoutElement layoutElement;
        
        [Space(10)]
        [SerializeField] private GameObject tooltip;
        [SerializeField] private Text headerField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private int maxCharacter = 80;

        public bool canShow = true;

        #endregion

        private void Start()
        {
            if (!IsOwner) return;
            
            tooltip.SetActive(false);
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            SetTooltipPosition();
        }

        public void Show(string content, string header = "")
        {
            if (!canShow) return;
        
            SetText(content, header);
            tooltip.SetActive(true);
        }

        public void Hide()
        {
            tooltip.SetActive(false);
        }

        private void SetText(string content, string header = "")
        {
            if (header == "")
            {
                headerField.gameObject.SetActive(false);
            }
            else
            {
                headerField.text = header;
                headerField.gameObject.SetActive(true);
            }

            descriptionField.text = content;
        
            int headerLength = headerField.text.Length;
            int contentLength = descriptionField.text.Length;

            layoutElement.enabled = headerLength > maxCharacter || contentLength > maxCharacter;
        }

        private void SetTooltipPosition()
        {
            Vector2 mousePosition = Input.mousePosition;

            float pivotX = mousePosition.x / Screen.width;
            float pivotY = mousePosition.y / Screen.height;

            rectTransform.pivot = new Vector2(pivotX, pivotY);

            tooltip.transform.position = mousePosition;
        }
    }
}

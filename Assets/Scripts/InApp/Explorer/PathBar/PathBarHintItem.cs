using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InApp.UI
{
    public class PathBarHintItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI displayText, typeText;
        [SerializeField] private Image background, icon;
        [SerializeField] private Color defaultColor, highlightedColor;

        private bool isHovered, isSelected;

        private IPathBarHint hint;
        private PathBarHintMaker maker;

        void Update()
        {
            background.color = isHovered || isSelected ? highlightedColor : defaultColor;
        }

        public void Refresh(IPathBarHint hint, string input, PathBarHintMaker maker, bool isSelected)
        {
            this.hint = hint;
            this.maker = maker;
            this.isSelected = isSelected;

            displayText.text = hint.GetDisplayText(input);
            typeText.text = hint.GetTypeText();
            icon.sprite = hint.GetIcon();
        }
        public void OnClick()
        {
            maker.OnClick(hint);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
    }
}
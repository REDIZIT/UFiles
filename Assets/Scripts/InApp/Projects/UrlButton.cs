using InApp.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class UrlButton : UILot<ProjectLink>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        [Inject] private IconsSO icons;

        public override void Refresh(ProjectLink model)
        {
            base.Refresh(model);
            text.text = model.displayText;

            if (string.IsNullOrWhiteSpace(model.iconID) == false && icons.customIcons.TryGetValue(model.iconID, out Sprite icon))
            {
                image.sprite = icon;
            }
            else
            {
                image.sprite = icons.browser;
            }
        }

        public void OnClick()
        {
            Application.OpenURL(Model.url);
        }
    }
}
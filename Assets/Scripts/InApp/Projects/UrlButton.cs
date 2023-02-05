using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InApp
{
    public class UrlButton : UILot<ProjectLink>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        public override void Refresh(ProjectLink model)
        {
            base.Refresh(model);
            text.text = model.displayText;
        }

        public void OnClick()
        {
            Application.OpenURL(Model.url);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class ContextItem
    {
        public string text;
        public List<ContextItem> children = new List<ContextItem>();

        [Inject] protected IconsSO icons;

        public virtual Texture2D GetIcon() { return null; }
        public virtual void Update() { }
        public virtual void OnClick(ContextItemEnvironment env) { }
    }
}
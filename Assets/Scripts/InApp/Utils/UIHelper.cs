using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class UIHelper
    {
        [Inject] private DiContainer container;

        public void Refresh<T>(IEnumerable<T> models, UILot<T> prefab, Transform parent)
        {
            int diff = models.Count() - parent.childCount;

            for (int i = 0; i < diff; i++)
            {
                container.InstantiatePrefabForComponent<UILot<T>>(prefab.gameObject, parent);
            }
            for (int i = diff; i < 0; i++)
            {
                Object.Destroy(parent.GetChild(0).gameObject);
            }

            int index = -1;
            foreach (T model in models)
            {
                index++;
                parent.GetChild(index).GetComponent<UILot<T>>().Refresh(model);
            }
        }
    }
}
using UnityEngine;

namespace InApp
{
    public abstract class UILot<T> : MonoBehaviour
    {
        protected T Model { get; private set; }

        public virtual void Refresh(T model)
        {
            Model = model;
        }
    }
}
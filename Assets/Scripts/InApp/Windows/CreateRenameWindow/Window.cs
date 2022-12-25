using UnityEngine;

namespace InApp.UI
{
    public class Window<T> : MonoBehaviour
    {
        protected T model;

        private void Awake()
        {
            OnAwake();
        }
        private void Update()
        {
            OnUpdate();
        }
        public void Show(T model)
        {
            this.model = model;

            gameObject.SetActive(true);
            OnShowed();
        }
        public void Close()
        {
            gameObject.SetActive(false);
            OnClosed();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnShowed() { }
        protected virtual void OnClosed() { }
        protected virtual void OnUpdate() { }
    }
}
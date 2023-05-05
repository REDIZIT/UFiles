using UnityEngine;
using Zenject;

namespace InApp.UI
{
    public class Window<T> : MonoBehaviour
    {
        [Inject] private BlockChecker shortcuts;

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

            shortcuts.Add(this);

            gameObject.SetActive(true);
            OnShowed();
        }
        public void Close()
        {
            shortcuts.Remove(this);

            gameObject.SetActive(false);
            OnClosed();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnShowed() { }
        protected virtual void OnClosed() { }
        protected virtual void OnUpdate() { }
    }
}
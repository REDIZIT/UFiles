using UnityEngine;
using Zenject;

namespace InApp.Sidebar
{
    public class SidebarGroup : MonoBehaviour
    {
        [SerializeField] private Transform content;

        [Inject] SidebarFolder.Factory factory;

        private void Start()
        {
            factory.Create("C:/", content);
            factory.Create("C:\\Users\\redizit\\Downloads", content);
        }
    }
}
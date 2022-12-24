using InApp.UI;
using UnityEngine;
using Zenject;

namespace InApp
{
    public class DragAndDropMono : MonoBehaviour
    {
        [Inject]
        private void Construct(FileOperator fileOperator, FilesView files)
        {
            Debug.Log("Register window");
            DragAndDrop.RegisterWindowTarget(fileOperator, files);
        }
        private void OnDisable()
        {
            Debug.Log("Disable window");
            DragAndDrop.UnregisterWindowTarget();
        }
    }
}
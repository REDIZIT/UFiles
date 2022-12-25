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
            DragAndDrop.RegisterWindowTarget(fileOperator, files);
        }
        private void OnDisable()
        {
            DragAndDrop.UnregisterWindowTarget();
        }
    }
}
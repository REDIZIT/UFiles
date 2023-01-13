using UnityEngine;
using Zenject;

namespace InApp
{
    public class BridgeLauncher : MonoBehaviour
    {
        private Bridge bridge;

        [Inject]
        private void Construct(Bridge bridge)
        {
            this.bridge = bridge;
        }

        private void Start()
        {
            bridge.Start();
        }
        private void Update()
        {
            bridge.UnityUpdate();
        }
    }
}
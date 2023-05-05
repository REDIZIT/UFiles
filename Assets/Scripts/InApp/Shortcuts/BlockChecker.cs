using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InApp
{
    public class BlockChecker : MonoBehaviour
    {
        private List<MonoBehaviour> stack = new List<MonoBehaviour>();

        public bool IsPointerOverInput()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.TryGetComponent(out TMP_InputField field))
            {
                return true;
            }
            return false;
        }

        public void Add(MonoBehaviour behaviour)
        {
            stack.Add(behaviour);
        }
        public void Remove(MonoBehaviour behaviour)
        {
            stack.Remove(behaviour);
        }
        public bool IsActive(MonoBehaviour behaviour)
        {
            return stack.Last() == behaviour;
        }
        public bool IsBlocked(MonoBehaviour behaviour)
        {
            return IsActive(behaviour) == false;
        }
    }
}
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;
using UnityEngine;

namespace Assets._Scripts.Etc.FeelManager
{
    public class Mono_FeelManager : MonoBehaviour
    {
        private Mono_Core_Data _coreData;

        private void Start()
        {
            _coreData = GetComponent<Mono_Core_Data>();

            if (_coreData == null)
            {
                Debug.LogError($"Mono_Core_Data component not found on {gameObject.name}!");
            }

        }
        public void SetFeelState(ScriptableObject_Core_State state)
        {
            Manager_Core.SetState(_coreData, state);
        }
    }
}

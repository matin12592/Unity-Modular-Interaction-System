using UnityEngine;
using System;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;

namespace Assets._Scripts.Core.Managers
{
    public class Manager_Core
    {
        #region Public Methods - GameObject versions
        public static void SetState(GameObject gameObject, ScriptableObject_Core_State state)
        {
            var monoData = gameObject?.GetComponent<Mono_Core_Data>();
            if (monoData == null || monoData.CurrentState == state) return;

            monoData.CurrentState = state;

            RaiseEventOnStateChanged(gameObject);
            RaiseEventOnStateChangedTo(gameObject, state);
        }

        public static bool CompareState(Mono_Core_Data coreData, ScriptableObject_Core_State state)
        {
            return coreData != null && coreData.CurrentState == state;
        }
        #endregion

        #region Events
        #region EventOnStateChanged
        public static event Action<GameObject> EventOnStateChanged;
        public static void RaiseEventOnStateChanged(GameObject gameObject)
        {
            if (Application.isPlaying == false) return;
            EventOnStateChanged?.Invoke(gameObject);
        }
        #endregion

        #region EventOnStateChangedTo
        public static event Action<GameObject, ScriptableObject_Core_State> EventOnStateChangedTo;
        public static void RaiseEventOnStateChangedTo(GameObject gameObject, ScriptableObject_Core_State state)
        {
            if (Application.isPlaying == false) return;
            EventOnStateChangedTo?.Invoke(gameObject, state);
        }
        #endregion
        #endregion
    }
}
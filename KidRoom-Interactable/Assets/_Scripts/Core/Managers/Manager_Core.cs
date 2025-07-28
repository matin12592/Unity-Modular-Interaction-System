using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;

namespace Assets._Scripts.Core.Managers
{
    public class Manager_Core
    {
        #region Public Methods
        public static void SetState(Mono_Core_Data coreData, ScriptableObject_Core_State state)
        {
            SetState(coreData, new List<ScriptableObject_Core_State> { state });
        }

        public static void SetState(Mono_Core_Data coreData, List<ScriptableObject_Core_State> states)
        {
            if(CompareState(coreData, states)) return;

            coreData.CurrentStates = states;

            RaiseEventOnStateChanged(coreData);
            RaiseEventOnStateChangedTo(coreData, states);
        }

        public static bool CompareState(Mono_Core_Data coreData, ScriptableObject_Core_State state, bool exactMatch = true)
        {
            return CompareState(coreData, new List<ScriptableObject_Core_State> { state }, exactMatch);
        }


        public static bool CompareState(Mono_Core_Data coreData, List<ScriptableObject_Core_State> states, bool exactMatch = true)
        {
            if (coreData == null || coreData.CurrentStates == null || states == null)
                return false;

            if (exactMatch)
            {
                if (coreData.CurrentStates.Count != states.Count)
                    return false;

                for (int i = 0; i < states.Count; i++)
                {
                    if (coreData.CurrentStates[i] != states[i])
                        return false;
                }

                return true;
            }
            else
            {
                // Subset match: All target states must exist in current states
                return states.All(state => coreData.CurrentStates.Contains(state));
            }
        }



        #endregion

        #region Events

        #region EventOnStateChanged

        public static event Action<Mono_Core_Data> EventOnStateChanged;
        public static void RaiseEventOnStateChanged(Mono_Core_Data coreData)
        {
            if (Application.isPlaying == false) return;
            EventOnStateChanged?.Invoke(coreData);
        }
        #endregion

        #region EventOnStateChangedTo
        public static event Action<Mono_Core_Data, ScriptableObject_Core_State> EventOnStateChangedTo;
        public static event Action<Mono_Core_Data, List<ScriptableObject_Core_State>> EventOnStateChangedToList;

        // Raise event - single state overload
        public static void RaiseEventOnStateChangedTo(Mono_Core_Data coreData, ScriptableObject_Core_State state)
        {
            if (Application.isPlaying == false) return;
            EventOnStateChangedTo?.Invoke(coreData, state);

            // Also raise the list version
            if (state != null)
            {
                EventOnStateChangedToList?.Invoke(coreData, new List<ScriptableObject_Core_State> { state });
            }
        }

        // Raise event - list of states overload
        public static void RaiseEventOnStateChangedTo(Mono_Core_Data coreData, List<ScriptableObject_Core_State> states)
        {
            if (Application.isPlaying == false) return;

            // If it's a single state, also raise the single state event for backward compatibility
            if (states is { Count: 1 })
            {
                EventOnStateChangedTo?.Invoke(coreData, states[0]);
            }

            EventOnStateChangedToList?.Invoke(coreData, states);
        }
        #endregion
        #endregion
    }
}
using Assets._Scripts.Primitive.InteractSystem.Monobehaviours;
using System;
using System.Collections.Generic;
using Assets._Scripts.Primitive.InteractSystem.DataObjects;
using UnityEngine;
using System.Linq;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;

namespace Assets._Scripts.Primitive.InteractSystem.Managers
{
    public static class Manager_InteractSystem
    {
        #region Public Variables
        public static EventArgs_InteractSystem InteractArgs;
        public static bool Enabled { get; private set; }

        #endregion

        #region Public Methods
        public static void Initialize(LayerMask layer, LayerMask ignoreLayer, float maxDistance)
        {
            Mono_InteractSystem_MouseController.Init(layer, ignoreLayer, maxDistance);
            var controllerGo = new GameObject("Interact System Mouse Controller");
            controllerGo.AddComponent<Mono_InteractSystem_MouseController>();
            Enabled = true;
        }

        public static void Enable(bool enable)
        {
            if (Enabled == enable)
                return;
            Enabled = enable;
            RaiseEventOnEnableChange(enable);
        }

        //public static void Interact()
        //{
        //    if (!Enabled) return;
        //    if (InteractArgs == null) return;
        //    RaiseEventOnInteract(InteractArgs);
        //}

        public static GameObject LastHoverGameObject()
        {
            return !Enabled ? null : InteractArgs?.LastHitGameObject;
        }

        public static string LastHoverText()
        {
            if (!Enabled) return null;
            var interactData = InteractArgs?.LastHitGameObject?.GetComponent<Mono_Core_Data>();
            if (interactData == null)
            {
                return null;
            }

            var interactAttributes = interactData.InteractSystemAttributes;
            if (interactAttributes == null)
            {
                Debug.LogError("Interactable Objects (Mono_Core_Data) Should Have This Scriptable Object: Attributes_InteractSystem");
                return null;
            }

            var entry = InteractSystem_Utils.GetMatchingStateEntry(
                interactData,
                interactAttributes.GameObjectStates,
                exactMatch: false
            );


            return entry?.DisplayText ?? interactAttributes.DefaultText;
        }
        #endregion

        #region Events

        #region EventOnMouseEnterGameObject
        public static event Action<EventArgs_InteractSystem> EventOnMouseEnterGameObject;
        public static void RaiseEventOnMouseEnterGameObject(EventArgs_InteractSystem arg)
        {
            if (!Enabled) return;
            if (Application.isPlaying == false) return;
            InteractArgs = arg;
            EventOnMouseEnterGameObject?.Invoke(arg);
        }
        #endregion

        #region EventOnMouseExitGameObject
        public static event Action<EventArgs_InteractSystem> EventOnMouseExitGameObject;
        public static void RaiseEventOnMouseExitGameObject(EventArgs_InteractSystem arg)
        {
            if (!Enabled) return;
            if (Application.isPlaying == false) return;
            InteractArgs = null;
            EventOnMouseExitGameObject?.Invoke(arg);
        }
        #endregion

        //#region EventOnInteract
        //public static event Action<EventArgs_InteractSystem> EventOnInteract;
        //public static void RaiseEventOnInteract(EventArgs_InteractSystem arg)
        //{
        //    if (!Enabled) return;
        //    if (Application.isPlaying == false) return;
        //    EventOnInteract?.Invoke(arg);
        //}
        //#endregion

        #region EventOnEnableChange
        public static event Action<bool> EventOnEnableChange;
        public static void RaiseEventOnEnableChange(bool arg)
        {
            if (Application.isPlaying == false) return;
            EventOnEnableChange?.Invoke(arg);
        }
        #endregion

        #endregion
    }
}

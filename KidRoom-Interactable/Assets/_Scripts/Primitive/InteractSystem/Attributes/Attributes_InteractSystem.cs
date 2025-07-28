using Assets._Scripts.Core.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Attributes
{
    [CreateAssetMenu(fileName = "InteractSystem_GameObject_Attributes", menuName = "EQiDo/InteractSystem/Attributes")]
    public class Attributes_InteractSystem : ScriptableObject
    {
        [SerializeField] private string _title;
        public string DefaultText = "Default";
        [Serializable]
        public class GameObjectStatesEntry
        {
            public ScriptableObject_Core_State State;
            public string DisplayText;
        }
        public List<GameObjectStatesEntry> GameObjectStates;

    }
}

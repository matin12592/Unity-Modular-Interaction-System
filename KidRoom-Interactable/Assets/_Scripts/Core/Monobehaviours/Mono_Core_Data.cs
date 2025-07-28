using Assets._Scripts.Core.ScriptableObjects;
using Assets._Scripts.Primitive.InteractSystem.Attributes;
using UnityEngine;

namespace Assets._Scripts.Core.Monobehaviours
{
    public class Mono_Core_Data : MonoBehaviour
    {
        [Header("Core Data")]
        public ScriptableObject_Core_State CurrentState;

        [Header("Interact System")]
        public Attributes_InteractSystem InteractSystemAttributes;
    }
}
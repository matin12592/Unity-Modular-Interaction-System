using System.Collections;
using System.Collections.Generic;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;
using Assets._Scripts.Etc.FeelManager;
using Assets._Scripts.Primitive.InteractSystem.Monobehaviours;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.InteractableObjects
{
    public class InteractableObject_Door : Mono_InteractSystem_InteractableObject
    {
        [Header("Door Settings")]
        [SerializeField] private GameObject _pivot;
        [SerializeField] private ScriptableObject_Core_State _openState;
        [SerializeField] private ScriptableObject_Core_State _closedState;
        [SerializeField] private float _openAngle = 90f;

        [Header("State Mappings")]
        [SerializeField] private List<DoorStateMapping> _doorStateMappings = new();

        private Mono_Core_Data _coreData;
        private float _animationDuration = 1f;
        private bool _isAnimating;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;

        [System.Serializable]
        public class DoorStateMapping
        {
            public ScriptableObject_Core_State feelState;
            public float animationDuration;
        }

        private void Awake()
        {
            _coreData = GetComponent<Mono_Core_Data>();
        }
        protected override void Start()
        {
            base.Start();
            Manager_Core.EventOnStateChangedTo += OnStateChangedTo;
            _closedRotation = _pivot.transform.rotation;
            _openRotation = _closedRotation * Quaternion.Euler(0, _openAngle, 0);
        }

        private void OnDestroy()
        {
            Manager_Core.EventOnStateChangedTo -= OnStateChangedTo;
        }

        private void OnStateChangedTo(Mono_Core_Data coreData, ScriptableObject_Core_State state)
        {
            if (coreData.GetComponent<Mono_FeelManager>() == null) return;
            HandleFeelStateChange(state);
        }

        private void HandleFeelStateChange(ScriptableObject_Core_State feelState)
        {
            if (feelState == null) return;
            var mapping = FindDoorMapping(feelState);
            if (mapping == null) return;
            TransitionToDoorState(mapping);
        }

        private void TransitionToDoorState(DoorStateMapping mapping)
        {
            _animationDuration = mapping.animationDuration;
        }

        private DoorStateMapping FindDoorMapping(ScriptableObject_Core_State feelState)
        {
            return _doorStateMappings.Find(mapping => mapping.feelState == feelState);
        }

        protected override void DoCustomInteraction()
        {
            if (_isAnimating) return;

            if (_coreData == null) return;

            FindAnyObjectByType<Mono_InteractSystem_MouseController>().UpdateInteractText(_animationDuration);

            if (Manager_Core.CompareState(_coreData, _closedState))
            {
                StartCoroutine(AnimateDoor(_openRotation, _openState));
            }
            else if(Manager_Core.CompareState(_coreData, _openState))
            {
                StartCoroutine(AnimateDoor(_closedRotation, _closedState));
            }
        }

        private IEnumerator AnimateDoor(Quaternion targetRotation, ScriptableObject_Core_State targetState)
        {
            _isAnimating = true;

            var startRotation = _pivot.transform.rotation;
            var elapsedTime = 0f;

            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = elapsedTime / _animationDuration;
                _pivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);
                yield return null;
            }

            _pivot.transform.rotation = targetRotation;

            Manager_Core.SetState(_coreData, targetState);

            _isAnimating = false;
        }

        public override bool CanInteract()
        {
            return base.CanInteract() && !_isAnimating;
        }
    }
}
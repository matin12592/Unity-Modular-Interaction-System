using System.Collections;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;
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
        [SerializeField] private float _animationDuration = 2f;

        private Mono_Core_Data _coreData;
        private bool _isAnimating;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;


        private void Awake()
        {
            _coreData = GetComponent<Mono_Core_Data>();
        }
        protected override void Start()
        {
            base.Start();

            _closedRotation = _pivot.transform.rotation;
            _openRotation = _closedRotation * Quaternion.Euler(0, _openAngle, 0);
        }

        protected override void DoCustomInteraction()
        {
            if (_isAnimating) return;

            if (_coreData == null) return;

            // Toggle between open and closed states
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

            // Update state after animation
            Manager_Core.SetState(_coreData, targetState);

            _isAnimating = false;
        }

        public override bool CanInteract()
        {
            return base.CanInteract() && !_isAnimating;
        }
    }
}
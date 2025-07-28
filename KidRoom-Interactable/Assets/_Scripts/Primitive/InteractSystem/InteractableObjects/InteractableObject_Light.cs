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
    public class InteractableObject_Light : Mono_InteractSystem_InteractableObject
    {
        #region Private Variables

        [Header("Light Configuration")]
        [SerializeField] private Light _light;

        [Header(("States Settings"))]
        [SerializeField] private ScriptableObject_Core_State _lightOnState;
        [SerializeField] private ScriptableObject_Core_State _lightOffState;

        [Header("State Mappings")]
        [SerializeField] private List<LightStateMapping> _lightStateMappings = new();

        [Header("Animation Settings")]
        [SerializeField] private float _transitionDuration = 0.5f;
        [SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float _lightIntensity;
        private Mono_Core_Data _coreData;
        private Coroutine currentTransition;

        [System.Serializable]
        public class LightStateMapping
        {
            public ScriptableObject_Core_State feelState;
            public Color lightColor = Color.white;
            public float lightIntensity = 1f;
        }

        #endregion

        #region Private Methods
        private void Awake()
        {
            _coreData = GetComponent<Mono_Core_Data>();
        }

        protected override void Start()
        {
            base.Start();
            Manager_Core.EventOnStateChangedToList += OnStateChangedTo;
            _lightIntensity = _light.intensity;
        }

        private void OnDestroy()
        {
            Manager_Core.EventOnStateChangedToList -= OnStateChangedTo;
        }

        protected override void DoCustomInteraction()
        {
            if(_coreData == null || _light == null) return;
            FindAnyObjectByType<Mono_InteractSystem_MouseController>().UpdateInteractText(_transitionDuration);

            if (Manager_Core.CompareState(_coreData, _lightOnState))
            {
                StartCoroutine(ToggleLight(0, false, _lightOffState));
            }
            else if (Manager_Core.CompareState(_coreData, _lightOffState))
            {
                StartCoroutine(ToggleLight(_lightIntensity, true, _lightOnState));
            }
        }

        private IEnumerator ToggleLight(float targetIntensity, bool setActive, ScriptableObject_Core_State targetState)
        {
            var startIntensity = _light.intensity;
            var elapsedTime = 0f;

            if (setActive)
            {
                _light.gameObject.SetActive(true);
                startIntensity = 0f;
            }

            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime / _transitionDuration;
                var curveValue = _transitionCurve.Evaluate(progress);

                _light.intensity = Mathf.Lerp(startIntensity, targetIntensity, curveValue);

                yield return null;
            }

            _light.intensity = targetIntensity;
            _light.gameObject.SetActive(setActive);
            Manager_Core.SetState(_coreData, targetState);

            currentTransition = null;
        }

        private void OnStateChangedTo(Mono_Core_Data coreData, List<ScriptableObject_Core_State> states)
        {
            if (coreData.GetComponent<Mono_FeelManager>() == null) return;
            HandleFeelStateChange(states);
        }

        private void HandleFeelStateChange(List<ScriptableObject_Core_State> feelStates)
        {
            if (_light == null || feelStates == null || feelStates.Count == 0) return;

            LightStateMapping bestMapping = null;

            foreach (var feelState in feelStates)
            {
                if (feelState == null) continue;

                var mapping = FindLightMapping(feelState);
                if (mapping == null) continue;
                bestMapping = mapping;
                break;
            }

            if (bestMapping != null)
            {
                TransitionToLightState(bestMapping);
            }
        }

        private LightStateMapping FindLightMapping(ScriptableObject_Core_State feelState)
        {
            return _lightStateMappings.Find(mapping => mapping.feelState == feelState);
        }

        private void TransitionToLightState(LightStateMapping mapping)
        {
            // Stop any current transition
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }

            currentTransition = StartCoroutine(TransitionCoroutine(mapping));
        }

        private IEnumerator TransitionCoroutine(LightStateMapping mapping)
        {
            var startColor = _light.color;
            var startIntensity = _light.intensity;
            var targetColor = mapping.lightColor;
            var targetIntensity = mapping.lightIntensity;
            var elapsedTime = 0f;

            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime / _transitionDuration;
                var curveValue = _transitionCurve.Evaluate(progress);

                _light.color = Color.Lerp(startColor, targetColor, curveValue);
                _light.intensity = Mathf.Lerp(startIntensity, targetIntensity, curveValue);

                yield return null;
            }

            _light.color = targetColor;
            _light.intensity = targetIntensity;
            _lightIntensity = targetIntensity;
            currentTransition = null;
        }

        #endregion
    }
}
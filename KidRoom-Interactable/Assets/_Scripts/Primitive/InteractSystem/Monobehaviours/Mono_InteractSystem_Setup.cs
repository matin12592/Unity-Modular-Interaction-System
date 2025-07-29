using Assets._Scripts.Primitive.InteractSystem.Managers;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_Setup : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private LayerMask _ignoreLayer;
        [SerializeField] private float _maxInteractionDistance = 100f;

        [Header("UI Settings")]
        [SerializeField] private GameObject _canvasPrefab;

        private void Start()
        {
            SetupInteractionSystem();
            SetupUi();
        }

        private void SetupInteractionSystem()
        {
            // Initialize the interaction system
            Manager_InteractSystem.Initialize(_interactableLayer, _ignoreLayer, _maxInteractionDistance);
        }

        private void SetupUi()
        {
            // Create UI system
            var uiGo = new GameObject("Interact System UI Manager");
            var worldUi = uiGo.AddComponent<Mono_InteractSystem_WorldUI>();

            if (_canvasPrefab == null) return;

            worldUi.CanvasPrefab = _canvasPrefab;
        }
    }
}
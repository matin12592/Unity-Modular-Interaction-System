using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_Highlighter : MonoBehaviour
    {
        [Header("Highlight Settings")]
        public Color highlightColor = Color.yellow;
        public float highlightWidth = 2f;
        public Outline.Mode outlineMode = Outline.Mode.OutlineVisible;

        [Header("Auto Setup")]
        public bool autoAddOutlineComponent = true;

        private Outline _outlineComponent;
        private bool _isHighlighted;

        private void Start()
        {
            SetupOutlineComponent();
        }

        private void SetupOutlineComponent()
        {
            // Try to find existing Outline component
            _outlineComponent = GetComponent<Outline>();

            // Auto-add Outline component if enabled and not found
            if (_outlineComponent == null && autoAddOutlineComponent)
            {
                _outlineComponent = gameObject.AddComponent<Outline>();
            }

            if (_outlineComponent == null)
            {
                Debug.LogWarning($"No Outline component found on {gameObject.name}. Please add an Outline component or enable 'Auto Add Outline Component'.");
                return;
            }

            // Store original values for reset if needed (though not used without animation)
            // _originalWidth = highlightWidth;
            // _originalColor = highlightColor;

            // Configure the outline component
            _outlineComponent.OutlineMode = outlineMode;
            _outlineComponent.OutlineColor = highlightColor;
            _outlineComponent.OutlineWidth = highlightWidth;

            // Disable outline initially
            _outlineComponent.enabled = false;
        }

        public void StartHighlight()
        {
            if (_isHighlighted || _outlineComponent == null) return;

            _isHighlighted = true;

            // Enable and configure the outline
            _outlineComponent.enabled = true;
            _outlineComponent.OutlineMode = outlineMode;
            _outlineComponent.OutlineColor = highlightColor;
            _outlineComponent.OutlineWidth = highlightWidth;
        }

        public void StopHighlight()
        {
            if (!_isHighlighted || _outlineComponent == null) return;

            _isHighlighted = false;

            // Disable the outline
            _outlineComponent.enabled = false;
        }

        // Force refresh the outline component reference (useful if Outline component is added at runtime)
        [ContextMenu("Refresh Outline Component")]
        public void RefreshOutlineComponent()
        {
            SetupOutlineComponent();
        }

        private void OnValidate()
        {
            // Update outline component properties in editor
            if (Application.isPlaying && _outlineComponent != null && _isHighlighted)
            {
                _outlineComponent.OutlineColor = highlightColor;
                _outlineComponent.OutlineWidth = highlightWidth;
                _outlineComponent.OutlineMode = outlineMode;
            }
        }

        private void OnDisable()
        {
            // Make sure outline is disabled when this component is disabled
            if (_outlineComponent != null)
            {
                _outlineComponent.enabled = false;
            }
        }

        private void OnDestroy()
        {
            // Clean up - disable outline if this component is destroyed
            if (_outlineComponent != null)
            {
                _outlineComponent.enabled = false;
            }
        }
    }
}
using Assets._Scripts.Primitive.InteractSystem.DataObjects;
using Assets._Scripts.Primitive.InteractSystem.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_WorldUI : MonoBehaviour
    {
        [Header("UI References")] public GameObject CanvasPrefab;

        [Header("Settings")] public Vector3 TextOffset = new(0, 5f, 0);

        private GameObject _currentCanvasInstance;
        private Text _currentText;
        private Canvas _canvas;
        private Camera _camera;

        private void Start()
        {
            // Get main camera
            _camera = Camera.main;

            if (_camera == null)
            {
                Debug.LogError("No camera found for WorldUI system!");
                return;
            }

            // Subscribe to events
            Manager_InteractSystem.EventOnMouseEnterGameObject += OnMouseEnterGameObject;
            Manager_InteractSystem.EventOnMouseExitGameObject += OnMouseExitGameObject;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            Manager_InteractSystem.EventOnMouseEnterGameObject -= OnMouseEnterGameObject;
            Manager_InteractSystem.EventOnMouseExitGameObject -= OnMouseExitGameObject;
        }

        private void OnMouseEnterGameObject(EventArgs_InteractSystem args)
        {
            var displayText = Manager_InteractSystem.LastHoverText();
            ShowInteractText(displayText);
            Debug.Log($"Showing text: '{displayText}' for object: {args.LastHitGameObject.name}");
        }

        private void OnMouseExitGameObject(EventArgs_InteractSystem args)
        {
            HideInteractText();
            Debug.Log($"Hiding text for object: {args.LastHitGameObject.name}");
        }

        private void ShowInteractText(string text)
        {
            // Create canvas instance if it doesn't exist
            if (_currentCanvasInstance == null)
            {
                if (CanvasPrefab != null)
                {
                    _currentCanvasInstance = Instantiate(CanvasPrefab);
                    _canvas = _currentCanvasInstance.GetComponentInChildren<Canvas>();
                    _currentText = _canvas.GetComponentInChildren<Text>();

                    if (_currentText == null)
                    {
                        Debug.LogError("No Text component found in Canvas prefab!");
                        return;
                    }
                }
            }

            // Set text and show
            _currentText.text = text;
            if (_currentCanvasInstance != null) _currentCanvasInstance.SetActive(true);

            // Immediately update position
            UpdateTextPosition();

        }

        private void HideInteractText()
        {
            if (_currentCanvasInstance != null)
            {
                _currentCanvasInstance.SetActive(false);
            }
        }

        private void UpdateTextPosition()
        {
            var targetObject = Manager_InteractSystem.LastHoverGameObject();
            if (targetObject == null || _currentCanvasInstance == null || _canvas == null) return;

            if (_canvas.renderMode != RenderMode.WorldSpace)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
                _canvas.worldCamera = _camera;
            }

            _canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 700;

            var rectTransform = _canvas.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
            }

            var rotatedOffset = targetObject.transform.TransformDirection(TextOffset);
            var worldPosition = targetObject.transform.position + rotatedOffset;

            _currentCanvasInstance.transform.position = worldPosition;

            _currentCanvasInstance.transform.LookAt(_camera.transform);
            _currentCanvasInstance.transform.Rotate(0, 180, 0);
        }
    }
}
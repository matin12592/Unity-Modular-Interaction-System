using Assets._Scripts.Primitive.InteractSystem.DataObjects;
using Assets._Scripts.Primitive.InteractSystem.Managers;
using TMPro;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_WorldUI : MonoBehaviour
    {
        #region Public Variables
        [Header("UI References")]
        public GameObject CanvasPrefab;

        [Header("Settings")]
        public Vector3 TextOffset = new(0, .5f, 0);
        #endregion

        #region Private Variables
        private GameObject _currentCanvasInstance;
        private TMP_Text _currentText;
        private Canvas _canvas;
        private Camera _camera;
        #endregion

        #region Private Methods
        private void Start()
        {
            _camera = Camera.main;

            if (_camera == null)
            {
                Debug.LogError("No camera found for WorldUI system!");
                return;
            }

            Manager_InteractSystem.EventOnMouseEnterGameObject += OnMouseEnterGameObject;
            Manager_InteractSystem.EventOnMouseExitGameObject += OnMouseExitGameObject;
        }

        private void OnDestroy()
        {
            Manager_InteractSystem.EventOnMouseEnterGameObject -= OnMouseEnterGameObject;
            Manager_InteractSystem.EventOnMouseExitGameObject -= OnMouseExitGameObject;
        }

        private void OnMouseEnterGameObject(EventArgs_InteractSystem args)
        {
            var displayText = Manager_InteractSystem.LastHoverText();
            ShowInteractText(displayText);
        }

        private void OnMouseExitGameObject(EventArgs_InteractSystem args)
        {
            HideInteractText();
        }

        private void ShowInteractText(string text)
        {
            if (_currentCanvasInstance == null)
            {
                if (CanvasPrefab != null)
                {
                    _currentCanvasInstance = Instantiate(CanvasPrefab);
                    _canvas = _currentCanvasInstance.GetComponentInChildren<Canvas>();
                    _canvas.worldCamera = _camera;
                    _currentText = _canvas.GetComponentInChildren<TMP_Text>();

                    if (_currentText == null)
                    {
                        Debug.LogError("No Text component found in Canvas prefab!");
                        return;
                    }
                }
            }

            _currentText.text = text;
            if (_currentCanvasInstance != null) _currentCanvasInstance.SetActive(true);

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

            var rectTransform = _canvas.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
            }

            var highestPoint = GetHighestPointOfObject(targetObject);

            // Apply the text offset from the highest point
            var rotatedOffset = targetObject.transform.TransformDirection(TextOffset);
            var worldPosition = highestPoint + rotatedOffset;

            _currentCanvasInstance.transform.position = worldPosition;

            _currentCanvasInstance.transform.LookAt(_camera.transform);
            _currentCanvasInstance.transform.Rotate(0, 180, 0);
        }

        // Pouriya: I got this from chat so don't ask me how it's working XD
        private Vector3 GetHighestPointOfObject(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return obj.transform.position;
            }

            var combinedBounds = renderers[0].bounds;

            foreach (var renderer in renderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            return new Vector3(combinedBounds.center.x, combinedBounds.max.y, combinedBounds.center.z);
        }
        #endregion

    }
}
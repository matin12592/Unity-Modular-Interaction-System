using System.Collections;
using Assets._Scripts.Primitive.InteractSystem.DataObjects;
using Assets._Scripts.Primitive.InteractSystem.Managers;
using Assets._Scripts.Primitive.InteractSystem.Interfaces;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_MouseController : MonoBehaviour
    {
        #region Private Variables
        private static LayerMask _interactableLayerMask;
        private static float _maxDistance;
        private EventArgs_InteractSystem _lastEventArgs;
        private GameObject _currentHoveredObject;
        #endregion

        #region Public Methods
        public static void Init(LayerMask layer, LayerMask ignoreLayer, float maxDistance)
        {
            _interactableLayerMask = layer & ~ignoreLayer;
            _maxDistance = maxDistance;
        }
        #endregion

        #region Private Methods
        private void Update()
        {
            if (!Manager_InteractSystem.Enabled) return;

            HandleMouseHover();
            HandleMouseClick();
        }

        private void HandleMouseHover()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, _maxDistance))
            {
                var hitObject = hit.collider.gameObject;

                if (((1 << hitObject.layer) & _interactableLayerMask) != 0)
                {
                    // Same object - no change needed
                    if (_currentHoveredObject == hitObject) return;

                    // Exit previous object if exists
                    if (_currentHoveredObject != null)
                    {
                        ExitHoveredObject();
                    }

                    // Enter new object
                    EnterHoveredObject(hitObject, hit.point);
                }
                else
                {
                    // Hit non-interactable object
                    if (_currentHoveredObject != null)
                    {
                        ExitHoveredObject();
                    }
                }
            }
            else
            {
                // No hit
                if (_currentHoveredObject != null)
                {
                    ExitHoveredObject();
                }
            }
        }

        private void HandleMouseClick()
        {
            if (!Input.GetMouseButtonDown(0)) return; // Left click

            if (_currentHoveredObject == null) return;

            //Manager_InteractSystem.Interact();

            // Call direct interaction if object implements IInteractable
            var interactable = _currentHoveredObject.GetComponent<IInteractable>();
            interactable?.OnInteract();
        }

        private void EnterHoveredObject(GameObject hitObject, Vector3 hitPoint)
        {
            _currentHoveredObject = hitObject;

            var displayText = Manager_InteractSystem.LastHoverText();

            _lastEventArgs = new EventArgs_InteractSystem
            {
                LastHitGameObject = hitObject,
                HitPoint = hitPoint,
                DisplayText = displayText
            };

            Manager_InteractSystem.RaiseEventOnMouseEnterGameObject(_lastEventArgs);

            // Call direct hover enter if object implements IInteractable
            var interactable = hitObject.GetComponent<IInteractable>();
            interactable?.OnHoverEnter();
        }

        private void ExitHoveredObject()
        {
            if (_currentHoveredObject == null) return;

            var exitArgs = new EventArgs_InteractSystem
            {
                LastHitGameObject = _currentHoveredObject,
                HitPoint = Vector3.zero,
                DisplayText = ""
            };

            Manager_InteractSystem.RaiseEventOnMouseExitGameObject(exitArgs);

            // Call direct hover exit if object implements IInteractable
            var interactable = _currentHoveredObject.GetComponent<IInteractable>();
            interactable?.OnHoverExit();

            _currentHoveredObject = null;
            _lastEventArgs = null;
        }

        public void UpdateInteractText(float duration)
        {
            StartCoroutine(UpdateRayCast(duration));
        }
        private IEnumerator UpdateRayCast(float duration)
        {
            var gameObject = Manager_InteractSystem.LastHoverGameObject();
            if (gameObject == null) yield break;

            var collider = gameObject.GetComponent<Collider>();
            if (collider == null || !collider.isTrigger) yield break;

            collider.enabled = false;
            yield return new WaitForSeconds(duration);
            collider.enabled = true;
        }
        #endregion
    }
}

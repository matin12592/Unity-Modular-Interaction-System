using Assets._Scripts.Primitive.InteractSystem.Interfaces;
using UnityEngine;
using Assets._Scripts.Primitive.InteractSystem.Managers;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    [RequireComponent(typeof(Mono_InteractSystem_Highlighter))]
    public class Mono_InteractSystem_InteractableObject : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private bool _canInteract = true;
        private Mono_InteractSystem_Highlighter _highlighter;

        protected virtual void Start()
        {
            _highlighter = GetComponent<Mono_InteractSystem_Highlighter>();
        }

        public virtual void OnInteract()
        {
            if (!CanInteract()) return;
            DoCustomInteraction();
        }

        public virtual void OnHoverEnter()
        {
            if (!CanInteract()) return;
            _highlighter?.StartHighlight();
        }

        public virtual void OnHoverExit()
        {
            _highlighter?.StopHighlight();
        }

        public virtual bool CanInteract()
        {
            return _canInteract && gameObject.activeInHierarchy;
        }

        public virtual string GetInteractText()
        {
            return Manager_InteractSystem.LastHoverText();
        }

        protected virtual void DoCustomInteraction() {}

        public void SetCanInteract(bool canInteract)
        {
            _canInteract = canInteract;
        }
    }
}
namespace Assets._Scripts.Primitive.InteractSystem.Interfaces
{
    public interface IInteractable
    {
        void OnInteract();
        void OnHoverEnter();
        void OnHoverExit();
        bool CanInteract();
        string GetInteractText();
    }
}

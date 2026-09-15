using DialogueEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

interface IInteractable
{
    public void Interact();
    public void OnFocus();
    public void OnLoseFocus();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    [SerializeField] public float InteractRange;
    [SerializeField] private Image crosshairImage; // GE�NDERT: nur noch ein Image statt 2 GameObjects

    [Header("Crosshair Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color focusColor = Color.red;

    private IInteractable currentInteractable;

    void Update()
    {
        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
        {
            return;
        }

        if (NoteManager.Instance != null && (NoteManager.Instance.IsNoteOpen || Time.time < NoteManager.Instance.CloseCooldownUntil))
        {
            return;
        }

        if (currentInteractable is MonoBehaviour mb && mb == null)
        {
            currentInteractable = null;
        }

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange);

        IInteractable hitInteractable = null;
        if (hitSomething)
        {
            hitInteractable = hitInfo.collider.GetComponentInParent<IInteractable>();
        }

        if (hitInteractable != currentInteractable)
        {
            currentInteractable?.OnLoseFocus();
            currentInteractable = hitInteractable;
            currentInteractable?.OnFocus();
        }


        crosshairImage.color = currentInteractable != null ? focusColor : normalColor;

        if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}
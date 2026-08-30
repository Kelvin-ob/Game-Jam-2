using DialogueEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // NEU

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
    [SerializeField] private Image crosshairImage; // GEÄNDERT: nur noch ein Image statt 2 GameObjects

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

        Debug.Log("Fokus: " + (currentInteractable != null) + " | Farbe wird gesetzt auf: " + (currentInteractable != null ? focusColor : normalColor));

        crosshairImage.color = currentInteractable != null ? focusColor : normalColor;

        if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}
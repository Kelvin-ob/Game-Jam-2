using DialogueEditor;
using UnityEngine;
using UnityEngine.InputSystem;


interface IInteractable
{
    public void Interact(); // called when player interacts with the object (presses E)
    public void OnFocus();  // called when raycast is on the object
    public void OnLoseFocus(); // called when raycast is not on the object anymore
}

public class Interactor : MonoBehaviour
{

    public Transform InteractorSource;
    [SerializeField] public float InteractRange;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject crosshairtargethit;

    private IInteractable currentInteractable; // what player is looking at in the moment

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive) // so that no  interaction triggers the same dialogue instantly
        {
            return;
        }

        if (currentInteractable is MonoBehaviour mb && mb == null) //echter unity Objektyp (sauwy needed help here)
        {
            currentInteractable = null;
        }

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange);

        IInteractable hitInteractable = null;

        if (hitSomething) // if somethin is found get try to get the component (cube, npc etc.)
        {
            hitInfo.collider.gameObject.TryGetComponent(out hitInteractable);
        }

        if (hitInteractable != currentInteractable)
        {
            currentInteractable?.OnLoseFocus();
            currentInteractable = hitInteractable;
            currentInteractable?.OnFocus();
        }

        crosshairtargethit.SetActive(currentInteractable != null); // show crosshair only when there is somethin interactable
        crosshair.SetActive(true); 

        if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}

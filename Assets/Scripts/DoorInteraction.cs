using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Interaction")]
    [SerializeField] private string promptText = "open";

    [Header("Swipe Card Lock (optional)")]
    [SerializeField] private bool requiresUnlock = false;
    [SerializeField] private string lockedMessage = "You need to swipe your card first";

    [Header("Direct Item Requirement (optional)")]
    [SerializeField] private string requiredItemId = ""; // z.B. "crowbar", leer lassen wenn nicht gebraucht
    [SerializeField] private string missingItemMessage = "You need a crowbar";

    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool isMoving = false;
    private bool isFocused = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentCoroutine;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void Interact()
    {
        // Swipe-Card Sperre (z.B. Haupttür)
        if (requiresUnlock && !isUnlocked)
        {
            StartCoroutine(ShowMessage(lockedMessage));
            return;
        }

        // Direkte Item-Sperre (z.B. Spind + Crowbar)
        if (!string.IsNullOrEmpty(requiredItemId) && !InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowMessage(missingItemMessage));
            return;
        }

        if (isMoving)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ToggleDoor());

        promptText = isOpen ? "close" : "open";
    }

    // wird vom CardSwiper aufgerufen
    public void Unlock()
    {
        isUnlocked = true;
    }

    public void OnFocus()
    {
        isFocused = true;
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        isFocused = false;
        InteractPromptManager.Instance.hidePrompt();
    }

    private IEnumerator ShowMessage(string message)
    {
        InteractPromptManager.Instance.showPrompt(message);
        yield return new WaitForSeconds(2f);

        if (isFocused)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
        else
        {
            InteractPromptManager.Instance.hidePrompt();
        }
    }

    private IEnumerator ToggleDoor()
    {
        isMoving = true;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;
        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;
        currentCoroutine = null;
    }
}
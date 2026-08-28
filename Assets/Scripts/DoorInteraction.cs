using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Door State")]
    [SerializeField] private string doorId;

    [Header("Interaction")]
    [SerializeField] private string promptText = "open";
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float transitionDelay = 0.3f;

    [Header("Swipe Card Lock (optional)")]
    [SerializeField] private bool requiresUnlock = false;
    [SerializeField] private string lockedMessage = "You need to swipe your card first";

    [Header("Direct Item Requirement (optional)")]
    [SerializeField] private string requiredItemId = "";
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

        openRotation = Quaternion.Euler(
            transform.eulerAngles + new Vector3(0, openAngle, 0)
        );

        // Gespeicherten Unlock-Zustand laden
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsDoorUnlocked(doorId))
        {
            isUnlocked = true;
        }
    }

    public void Interact()
    {
        // Swipe-Card Sperre
        if (requiresUnlock && !isUnlocked)
        {
            StartCoroutine(ShowMessage(lockedMessage));
            return;
        }

        // Item-Sperre
        if (!string.IsNullOrEmpty(requiredItemId) &&
            !InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowMessage(missingItemMessage));
            return;
        }

        if (isMoving)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // Tür mit Scene Transition
        if (!isOpen && !string.IsNullOrEmpty(sceneToLoad))
        {
            currentCoroutine = StartCoroutine(OpenAndTransition());
        }
        else
        {
            currentCoroutine = StartCoroutine(ToggleDoor());
        }

        promptText = isOpen ? "close" : "open";
    }

    public void Unlock()
    {
        isUnlocked = true;

        GameStateManager.Instance.SetDoorUnlocked(doorId);
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
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;
        currentCoroutine = null;
    }

    private IEnumerator OpenAndTransition()
    {
        isMoving = true;
        isOpen = true;

        // Tür öffnen und gleichzeitig Fade-Timer starten
        StartCoroutine(OpenDoor());

        // Nur 0,3 Sekunden warten
        yield return new WaitForSeconds(transitionDelay);

        // Fade starten
        SceneFader sceneFader = FindFirstObjectByType<SceneFader>();

        if (sceneFader != null)
        {
            sceneFader.FadeAndLoad(sceneToLoad, fadeDuration);
        }
        else
        {
            Debug.LogError("Kein SceneFader in der Szene gefunden!");
        }
    }

    private IEnumerator OpenDoor()
    {
        Quaternion targetRotation = openRotation;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;
    }
}


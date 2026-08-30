using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Door State")]
    [SerializeField] private string doorId;

    [Header("Door Sound")]
    [SerializeField] private AudioClip openSound;
    [SerializeField][Range(0f, 1f)] private float openVolume = 1f;

    [SerializeField] private AudioClip closeSound;
    [SerializeField][Range(0f, 1f)] private float closeVolume = 1f;

    [Header("Break Open (optional)")]
    [SerializeField] private bool breakOpenFirstTime = false;
    [SerializeField] private float breakOpenSpeed = 3f; // NEU: eigene Geschwindigkeit fürs Aufbrechen
    [SerializeField] private AudioClip breakOpenSound;
    [SerializeField][Range(0f, 1f)] private float breakOpenVolume = 1f;

    private bool hasBeenBrokenOpen = false;

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

    [Header("Generator Requirement (optional)")]
    [SerializeField] private bool requiresGeneratorActive = false;
    [SerializeField] private string requiredGeneratorId = "generator_01";
    [SerializeField] private string generatorRequiredMessage = "The generator has to be running";

    [Header("Additional Item Requirement (optional)")]
    [SerializeField] private string additionalRequiredItemId = "";
    [SerializeField] private string missingAdditionalItemMessage = "You need another item";

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

        // Gespeicherten Aufbruch-Zustand laden
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(doorId + "_broken"))
        {
            hasBeenBrokenOpen = true;
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

        // Generator-Sperre
        if (requiresGeneratorActive &&
            (GameStateManager.Instance == null ||
             !GameStateManager.Instance.IsGeneratorActivated(requiredGeneratorId)))
        {
            StartCoroutine(ShowMessage(generatorRequiredMessage));
            return;
        }

        // Item-Sperre
        if (!string.IsNullOrEmpty(requiredItemId) &&
            (InventoryManager.Instance == null ||
             !InventoryManager.Instance.HasItem(requiredItemId)))
        {
            StartCoroutine(ShowMessage(missingItemMessage));
            return;
        }

        if (!string.IsNullOrEmpty(additionalRequiredItemId) &&
            (InventoryManager.Instance == null ||
             !InventoryManager.Instance.HasItem(additionalRequiredItemId)))
        {
            StartCoroutine(ShowMessage(missingAdditionalItemMessage));
            return;
        }

        if (isMoving)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // ERSTES AUFBRECHEN
        if (breakOpenFirstTime && !hasBeenBrokenOpen)
        {
            currentCoroutine = StartCoroutine(BreakOpen());
            promptText = string.IsNullOrEmpty(sceneToLoad) ? "close" : "enter"; // GEÄNDERT
            return;
        }

        // NEU: Nach dem Aufbrechen, falls Szenenwechsel vorgesehen ist, direkt reingehen statt togglen
        if (hasBeenBrokenOpen && !string.IsNullOrEmpty(sceneToLoad))
        {
            currentCoroutine = StartCoroutine(OpenAndTransition());
            return;
        }

        // NORMALE TÜR (unverändert)
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

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetDoorUnlocked(doorId);
        }
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

    // =========================
    // BREAK OPEN
    // =========================

    private IEnumerator BreakOpen()
    {
        isMoving = true;

        hasBeenBrokenOpen = true;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(doorId + "_broken");
        }

        PlaySound(breakOpenSound, breakOpenVolume);

        Quaternion targetRotation = openRotation;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * breakOpenSpeed // GEÄNDERT: breakOpenSpeed statt openSpeed
            );

            yield return null;
        }

        transform.rotation = targetRotation;

        isOpen = true;
        isMoving = false;
        currentCoroutine = null;
    }

    // =========================
    // NORMAL OPEN / CLOSE
    // =========================

    private IEnumerator ToggleDoor()
    {
        isMoving = true;

        bool opening = !isOpen;

        Quaternion targetRotation = opening
            ? openRotation
            : closedRotation;

        isOpen = opening;

        // Sound
        if (opening)
        {
            PlaySound(openSound, openVolume);
        }
        else
        {
            PlaySound(closeSound, closeVolume);
        }

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

    // =========================
    // SCENE TRANSITION
    // =========================

    private IEnumerator OpenAndTransition()
    {
        isMoving = true;
        isOpen = true;

        // Open-Sound
        PlaySound(openSound, openVolume);

        // T�r �ffnen
        StartCoroutine(OpenDoor());

        yield return new WaitForSeconds(transitionDelay);

        SceneFader sceneFader = FindFirstObjectByType<SceneFader>();

        if (sceneFader != null)
        {
            sceneFader.FadeAndLoad(
                sceneToLoad,
                fadeDuration
            );
        }
        else
        {
            Debug.LogError(
                "Kein SceneFader in der Szene gefunden!"
            );
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

    // =========================
    // SOUND
    // =========================

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        AudioSource.PlayClipAtPoint(
            clip,
            transform.position,
            volume
        );
    }
}


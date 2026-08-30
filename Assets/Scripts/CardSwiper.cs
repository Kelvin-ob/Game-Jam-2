using UnityEngine;

public class CardSwiper : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorInteraction linkedDoor;

    [Header("Keycard")]
    [SerializeField] private string requiredItemId = "keycard";

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "Swipe card";

    [SerializeField] private string lockedMessage = "You need an activated keycard";

    [Header("Voice")]
    [SerializeField] private VoiceTrigger notActivatedVoiceTrigger;

    [Header("Swipe Sound")]
    [SerializeField] private AudioClip swipeSound;
    [SerializeField][Range(0f, 1f)] private float swipeVolume = 1f;

    [Header("Save State")]
    [SerializeField] private string swipeStateId = "keycard_swiped";

    private bool hasSwiped = false;

    private void Start()
    {
        // Prüfen, ob bereits geswiped wurde
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(swipeStateId))
        {
            hasSwiped = true;
        }
    }

    public void Interact()
    {
        // =========================
        // BEREITS GESWIPED
        // =========================

        if (hasSwiped)
        {
            InteractPromptManager.Instance.showPrompt("Already unlocked");
            return;
        }

        // =========================
        // KEYCARD BESITZEN?
        // =========================

        if (InventoryManager.Instance == null ||
            !InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowLockedMessage());
            return;
        }

        // =========================
        // KEYCARD AKTIVIERT?
        // =========================

        if (GameStateManager.Instance == null ||
            !GameStateManager.Instance.IsKeycardActivated())
        {
            InteractPromptManager.Instance.showPrompt(
                "Keycard needs to be activated"
            );

            // AI-Dialog starten
            if (notActivatedVoiceTrigger != null)
            {
                notActivatedVoiceTrigger.TriggerVoice();
            }

            return;
        }

        // =========================
        // TÜR ENTSPERREN
        // =========================

        linkedDoor.Unlock();

        hasSwiped = true;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(swipeStateId);
        }

        // =========================
        // SWIPE SOUND
        // =========================

        if (swipeSound != null)
        {
            AudioSource.PlayClipAtPoint(
                swipeSound,
                transform.position,
                swipeVolume
            );
        }

        InteractPromptManager.Instance.showPrompt(
            "Card accepted"
        );
    }

    private System.Collections.IEnumerator ShowLockedMessage()
    {
        InteractPromptManager.Instance.showPrompt(lockedMessage);

        yield return new WaitForSeconds(2f);

        if (hasSwiped)
        {
            InteractPromptManager.Instance.showPrompt("Already unlocked");
        }
        else
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnFocus()
    {
        if (hasSwiped)
        {
            InteractPromptManager.Instance.showPrompt("Already unlocked");
        }
        else
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }
}


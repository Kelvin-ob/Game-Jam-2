using UnityEngine;

public class CardSwiper : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorInteraction linkedDoor;
    [SerializeField] private string requiredItemId = "keycard";

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "Swipe card";
    [SerializeField] private string lockedMessage = "You need a keycard";

    [Header("Swipe Sound")]
    [SerializeField] private AudioClip swipeSound;
    [SerializeField][Range(0f, 1f)] private float swipeVolume = 1f;

    [Header("Save State")]
    [SerializeField] private string swipeStateId = "keycard_swiped";

    private bool hasSwiped = false;

    private void Start()
    {
        // Prüfen, ob die Karte bereits geswiped wurde
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(swipeStateId))
        {
            hasSwiped = true;
        }
    }

    public void Interact()
    {
        // Wenn bereits geswiped wurde
        if (hasSwiped)
        {
            InteractPromptManager.Instance.showPrompt("Already unlocked");
            return;
        }

        // Prüfen, ob Spieler die Keycard besitzt
        if (!InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowLockedMessage());
            return;
        }

        // Tür entsperren
        linkedDoor.Unlock();

        // Als geswiped speichern
        hasSwiped = true;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(swipeStateId);
        }

        // Swipe-Sound nur beim ersten erfolgreichen Swipe
        if (swipeSound != null)
        {
            AudioSource.PlayClipAtPoint(
                swipeSound,
                transform.position,
                swipeVolume
            );
        }

        // Bestätigung
        InteractPromptManager.Instance.showPrompt("Card accepted");
    }

    private System.Collections.IEnumerator ShowLockedMessage()
    {
        InteractPromptManager.Instance.showPrompt(lockedMessage);

        yield return new WaitForSeconds(2f);

        InteractPromptManager.Instance.showPrompt(promptText);
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


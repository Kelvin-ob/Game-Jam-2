using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text";

    [Header("Pickup Sound")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField][Range(0f, 1f)] private float pickupVolume = 1f;

    private void Start()
    {
        // Wenn bereits eingesammelt → Objekt entfernen
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(itemId))
        {
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        // Item ins Inventar hinzufügen
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemId);
        }

        // Item als eingesammelt speichern
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(itemId);
        }

        // Sound abspielen
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupSound,
                transform.position,
                pickupVolume
            );
        }

        // Prompt entfernen
        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.hidePrompt();
        }

        // Item aus der Szene entfernen
        Destroy(gameObject);
    }

    public void OnFocus()
    {
        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnLoseFocus()
    {
        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.hidePrompt();
        }
    }
}


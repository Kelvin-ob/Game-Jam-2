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
        InventoryManager.Instance.AddItem(itemId);

        // Item als eingesammelt speichern
        GameStateManager.Instance.SetItemCollected(itemId);

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
        InteractPromptManager.Instance.hidePrompt();

        Destroy(gameObject);
    }

    public void OnFocus()
    {
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }
}


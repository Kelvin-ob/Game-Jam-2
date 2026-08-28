using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text";

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
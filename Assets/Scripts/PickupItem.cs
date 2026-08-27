using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId; //example: "secret_room_key"
    
    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text"; //customisable inside unity



  

    public void Interact()
    {
        InventoryManager.Instance.AddItem(itemId);
        Destroy(gameObject); // item aus welt -- nicht gelöscht!! nur nicht sichtbar
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

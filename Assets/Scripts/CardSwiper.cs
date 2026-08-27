using UnityEngine;

public class CardSwiper : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorInteraction linkedDoor;
    [SerializeField] private string requiredItemId = "keycard"; // Check ist jetzt HIER

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "Swipe card";
    [SerializeField] private string lockedMessage = "You need a keycard";

    public void Interact()
    {
        if (!InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowLockedMessage());
            return;
        }

        linkedDoor.Unlock(); // Tür wird nur entsperrt, NICHT geöffnet
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
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }
}
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation TheConversation;

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "Press E";

    public void Interact()
    {
        ConversationManager.Instance.StartConversation(TheConversation);
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
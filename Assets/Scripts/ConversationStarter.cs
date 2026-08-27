using UnityEngine;
using DialogueEditor;
using UnityEngine.InputSystem;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation TheConversation;
    
    [TextArea(2, 5)]
    [SerializeField] private string promptText = "Press E"; //customisable inside unity

    
    public void Interact()
    {
        ConversationManager.Instance.StartConversation(TheConversation);

        if (ConversationManager.Instance.IsConversationActive == true)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ConversationManager.Instance.SelectNextOption();
            }
        }
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

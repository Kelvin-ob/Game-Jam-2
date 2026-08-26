using UnityEngine;
using DialogueEditor;
using UnityEngine.InputSystem;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation TheConversation;
    [SerializeField] private GameObject interactPrompt;

    void Start()
    {
        interactPrompt.SetActive(false);
    }

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
        interactPrompt.SetActive(true);
    }

    public void OnLoseFocus()
    {
        interactPrompt.SetActive(false);
    }


}

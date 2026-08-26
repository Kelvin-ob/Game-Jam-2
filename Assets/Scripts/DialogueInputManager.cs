using UnityEngine;
using DialogueEditor;
using UnityEngine.InputSystem;

public class DialogueInputManager : MonoBehaviour
{
    void Update()
    {
        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ConversationManager.Instance.PressSelectedOption();
            }
        }
    }
}

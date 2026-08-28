using UnityEngine;
using DialogueEditor;
using UnityEngine.InputSystem;

public class DialogueInputManager : MonoBehaviour
{
    [SerializeField] private Player player;

    private bool movementLocked = false;

    void Update()
    {
        if (ConversationManager.Instance != null)
        {
            bool conversationActive = ConversationManager.Instance.IsConversationActive;

            // Dialog wurde gestartet
            if (conversationActive && !movementLocked)
            {
                player.SetMovementEnabled(false);
                movementLocked = true;
            }

            // Dialog wurde beendet
            if (!conversationActive && movementLocked)
            {
                player.SetMovementEnabled(true);
                movementLocked = false;
            }

            // E für Dialog
            if (conversationActive && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ConversationManager.Instance.PressSelectedOption();
            }
        }
    }
}
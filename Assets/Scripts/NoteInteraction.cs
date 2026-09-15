using System.Collections;
using UnityEngine;

public class NoteInteractable : MonoBehaviour, IInteractable
{
    [TextArea(3, 10)]
    [SerializeField] private string noteContent = "Custom text";

    [SerializeField] private string promptText = "read";
    [SerializeField] private Player player; // for movement Lock

    private bool hasBeenRead = false;

    public void Interact()
    {
        if (NoteManager.Instance.IsNoteOpen)
            return;

        StartCoroutine(ReadNoteRoutine());
    }

    private IEnumerator ReadNoteRoutine()
    {
        if (player != null)
        {
            player.SetMovementEnabled(false);
        }

        NoteManager.Instance.ShowNote(noteContent);
        hasBeenRead = true;

        yield return new WaitUntil(() => !NoteManager.Instance.IsNoteOpen);

        if (player != null)
        {
            player.SetMovementEnabled(true);
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
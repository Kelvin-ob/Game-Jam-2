using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private string itemId;

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text";

    [Header("Voice Reaction")]
    [SerializeField] private bool playVoiceOnPickup = false;

    [SerializeField] private float voiceDelay = 1f;

    [TextArea(2, 5)]
    [SerializeField] private string[] voiceLines;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 1f;

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
        // Item ins Inventar
        InventoryManager.Instance.AddItem(itemId);

        // Item als eingesammelt speichern
        GameStateManager.Instance.SetItemCollected(itemId);

        // Prompt entfernen
        InteractPromptManager.Instance.hidePrompt();

        // Voice abspielen, falls aktiviert
        if (playVoiceOnPickup)
        {
            VoiceManager.Instance.StartPickupDialogue(
                voiceLines,
                voiceDelay,
                typingSpeed,
                displayDuration,
                timeBetweenLines
            );
        }

        Destroy(gameObject);

        // Item aus der Szene entfernen
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator PlayPickupVoice()
    {
        // Delay bevor die AI spricht
        if (voiceDelay > 0f)
        {
            yield return new WaitForSeconds(voiceDelay);
        }

        foreach (string line in voiceLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            VoiceManager.Instance.ShowVoice(
                line,
                typingSpeed,
                displayDuration
            );

            // Warten, bis die aktuelle Zeile fertig ist
            yield return new WaitUntil(
                () => !VoiceManager.Instance.IsVoiceActive
            );

            // Pause zwischen den Zeilen
            yield return new WaitForSeconds(timeBetweenLines);
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

    
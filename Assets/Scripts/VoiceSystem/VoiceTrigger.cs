using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private string triggerId = "voice_trigger_01";
    [SerializeField] private bool triggerOnce = true;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 1f;

    [Header("Requirements")]
    [SerializeField] private VoiceTrigger[] requiredTriggers;
    [SerializeField] private string[] requiredItems;

    [Header("Movement")]
    [SerializeField] private Player player;
    [SerializeField] private bool lockMovement = true;

    private bool triggered;

    private void Start()
    {
        if (triggerOnce &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.IsVoiceTriggerTriggered(triggerId))
        {
            triggered = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        TriggerVoice();
    }

    public void TriggerVoice()
    {
        // Bereits ausgelöst
        if (triggerOnce && triggered)
            return;

        // Voraussetzungen prüfen
        if (!AllRequirementsMet())
            return;

        // Als ausgelöst markieren
        triggered = true;

        // Status dauerhaft speichern
        if (triggerOnce &&
            GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetVoiceTriggerTriggered(triggerId);
        }

        StartCoroutine(PlayDialogue());
    }

    private bool AllRequirementsMet()
    {
        // ==========================================
        // REQUIRED VOICE TRIGGERS
        // ==========================================

        if (requiredTriggers != null)
        {
            foreach (VoiceTrigger requiredTrigger in requiredTriggers)
            {
                if (requiredTrigger == null)
                    continue;

                // Erst prüfen, ob der Trigger aktuell ausgelöst wurde
                if (requiredTrigger.triggered)
                    continue;

                // Falls der Trigger aus einer anderen Szene kommt:
                // gespeicherten Status prüfen
                if (GameStateManager.Instance != null &&
                    GameStateManager.Instance.IsVoiceTriggerTriggered(
                        requiredTrigger.GetTriggerId()))
                {
                    continue;
                }

                // Voraussetzung noch nicht erfüllt
                return false;
            }
        }

        // ==========================================
        // REQUIRED ITEMS
        // ==========================================

        if (requiredItems != null)
        {
            foreach (string itemId in requiredItems)
            {
                if (string.IsNullOrWhiteSpace(itemId))
                    continue;

                if (GameStateManager.Instance == null ||
                    !GameStateManager.Instance.IsItemCollected(itemId))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Trigger-ID für andere VoiceTrigger verfügbar machen
    public string GetTriggerId()
    {
        return triggerId;
    }

    private System.Collections.IEnumerator PlayDialogue()
    {
        // ==========================================
        // BEWEGUNG SPERREN
        // ==========================================

        if (lockMovement && player != null)
        {
            player.SetMovementEnabled(false);
        }

        // ==========================================
        // DIALOG ABSPIELEN
        // ==========================================

        foreach (string line in dialogueLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            VoiceManager.Instance.ShowVoice(
                line,
                typingSpeed,
                displayDuration
            );

            // Warten, bis Voice fertig ist
            yield return new WaitUntil(
                () => !VoiceManager.Instance.IsVoiceActive
            );

            // Pause zwischen den Sätzen
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // ==========================================
        // BEWEGUNG WIEDER ERLAUBEN
        // ==========================================

        if (lockMovement && player != null)
        {
            player.SetMovementEnabled(true);
        }
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}


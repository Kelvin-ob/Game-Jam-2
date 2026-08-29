using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    [Header("Trigger")]
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && triggered)
            return;

        if (!AllRequirementsMet())
            return;

        triggered = true;

        StartCoroutine(PlayDialogue());
    }

    private bool AllRequirementsMet()
    {
        // Required Voice Triggers prüfen
        if (requiredTriggers != null)
        {
            foreach (VoiceTrigger requiredTrigger in requiredTriggers)
            {
                if (requiredTrigger == null)
                    continue;

                if (!requiredTrigger.triggered)
                    return false;
            }
        }

        // Required Items prüfen
        if (requiredItems != null)
        {
            foreach (string itemId in requiredItems)
            {
                if (string.IsNullOrWhiteSpace(itemId))
                    continue;

                if (!GameStateManager.Instance.IsItemCollected(itemId))
                    return false;
            }
        }

        return true;
    }

    private System.Collections.IEnumerator PlayDialogue()
    {
        // Bewegung sperren
        if (lockMovement)
        {
            player.SetMovementEnabled(false);
        }

        // Dialog abspielen
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

        // Bewegung wieder erlauben
        if (lockMovement)
        {
            player.SetMovementEnabled(true);
        }
    }
}


using UnityEngine;
using System.Collections;

public class WakeUpSequence : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SceneFader sceneFader;

    [Header("Wake Up Dialogue")]
    [TextArea(2, 5)]
    [SerializeField]
    private string[] dialogueLines =
    {
        "Good morning...",
        "Can you hear me?",
        "Take it slow.",
        "You've been unconscious for a while.",
        "Do you remember what happened?"
    };

    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 2f;

    [Header("Movement")]
    [SerializeField] private float wakeUpDuration = 3f;
    [SerializeField] private float movementLockDuration = 5f;

    private void Start()
    {
        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        // Spieler kann sich nicht bewegen
        player.SetMovementEnabled(false);

        // Warten, bis das Aufwachen vorbei ist
        yield return new WaitForSeconds(wakeUpDuration);

        // Dialog Zeile für Zeile anzeigen
        foreach (string line in dialogueLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Voice mit den Einstellungen der WakeUpSequence starten
            VoiceManager.Instance.ShowVoice(
                line,
                typingSpeed,
                displayDuration
            );

            // Warten, bis die Voice komplett fertig ist
            yield return new WaitUntil(
                () => !VoiceManager.Instance.IsVoiceActive
            );

            // Pause zwischen den Sätzen
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // Movement Lock insgesamt berücksichtigen
        float remainingTime = movementLockDuration - wakeUpDuration;

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Spieler wieder freigeben
        player.SetMovementEnabled(true);
    }
}


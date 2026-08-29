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

    [Header("Save State")]
    [SerializeField] private string wakeUpStateId = "operation_room_wakeup";

    private void Start()
    {
        // Prüfen, ob das Aufwachen bereits einmal abgespielt wurde
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(wakeUpStateId))
        {
            // Aufwachen überspringen
            player.SetMovementEnabled(true);
            return;
        }

        // Sofort speichern, dass das Aufwachen gestartet wurde.
        // Dadurch wird es auch nach einem Szenenwechsel nicht erneut abgespielt.
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(wakeUpStateId);
        }

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        // Spieler kann sich nicht bewegen
        player.SetMovementEnabled(false);

        // Warten, bis die Aufwachbewegung vorbei ist
        yield return new WaitForSeconds(wakeUpDuration);

        // Dialog Zeile für Zeile anzeigen
        foreach (string line in dialogueLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            VoiceManager.Instance.ShowVoice(
                line,
                typingSpeed,
                displayDuration
            );

            // Warten, bis die aktuelle Voice komplett fertig ist
            yield return new WaitUntil(
                () => !VoiceManager.Instance.IsVoiceActive
            );

            // Pause zwischen den Sätzen
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // Movement Lock berücksichtigen
        float remainingTime = movementLockDuration - wakeUpDuration;

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Spieler wieder freigeben
        player.SetMovementEnabled(true);
    }
}


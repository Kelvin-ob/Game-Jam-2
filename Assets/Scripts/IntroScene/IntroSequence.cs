using UnityEngine;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Intro Dialogue")]
    [TextArea(2, 5)]
    [SerializeField]
    private string[] dialogueLines =
    {
        "Good morning...",
        "Can you hear me?",
        "Take it slow."
    };

    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 2f;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Spieler direkt sperren
        player.SetMovementEnabled(false);

        // Dialog Zeile für Zeile abspielen
        foreach (string line in dialogueLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            VoiceManager.Instance.ShowVoice(
                line,
                typingSpeed,
                displayDuration
            );

            // Warten, bis die aktuelle Voice fertig ist
            yield return new WaitUntil(
                () => !VoiceManager.Instance.IsVoiceActive
            );

            // Pause zwischen den Sätzen
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // Spieler wieder freigeben
        player.SetMovementEnabled(true);
    }
}


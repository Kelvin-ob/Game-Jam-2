using UnityEngine;
using TMPro;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text voiceText;

    [Header("Default Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Typewriter Sound")]
    [SerializeField] private AudioSource typewriterAudio;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField] private float typewriterSoundInterval = 0.05f;

    private Coroutine currentRoutine;

    public bool IsVoiceActive { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================================================
    // NORMAL VOICE
    // =========================================================

    public void ShowVoice(string text)
    {
        ShowVoice(text, typingSpeed, displayDuration);
    }

    public void ShowVoice(
        string text,
        float customTypingSpeed,
        float customDisplayDuration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(
            ShowVoiceRoutine(
                text,
                customTypingSpeed,
                customDisplayDuration
            )
        );
    }

    private IEnumerator ShowVoiceRoutine(
        string text,
        float customTypingSpeed,
        float customDisplayDuration)
    {
        IsVoiceActive = true;

        voiceText.gameObject.SetActive(true);

        // Text zunächst leer
        voiceText.text = "";

        // Typewriter
        for (int i = 0; i < text.Length; i++)
        {
            voiceText.text += text[i];

            PlayTypewriterSound();

            yield return new WaitForSeconds(customTypingSpeed);
        }

        // Text stehen lassen
        yield return new WaitForSeconds(customDisplayDuration);

        voiceText.gameObject.SetActive(false);

        IsVoiceActive = false;
        currentRoutine = null;
    }

    // =========================================================
    // PICKUP DIALOGUE
    // =========================================================

    public void StartPickupDialogue(
        string[] lines,
        float delay,
        float customTypingSpeed,
        float customDisplayDuration,
        float timeBetweenLines)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(
            PickupDialogueRoutine(
                lines,
                delay,
                customTypingSpeed,
                customDisplayDuration,
                timeBetweenLines
            )
        );
    }

    private IEnumerator PickupDialogueRoutine(
        string[] lines,
        float delay,
        float customTypingSpeed,
        float customDisplayDuration,
        float timeBetweenLines)
    {
        IsVoiceActive = true;

        // Delay bevor die AI spricht
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            yield return StartCoroutine(
                ShowVoiceLine(
                    line,
                    customTypingSpeed,
                    customDisplayDuration
                )
            );

            // Pause zwischen den Zeilen
            if (timeBetweenLines > 0f)
                yield return new WaitForSeconds(timeBetweenLines);
        }

        voiceText.gameObject.SetActive(false);

        IsVoiceActive = false;
        currentRoutine = null;
    }

    // =========================================================
    // SINGLE LINE
    // =========================================================

    private IEnumerator ShowVoiceLine(
        string text,
        float customTypingSpeed,
        float customDisplayDuration)
    {
        voiceText.gameObject.SetActive(true);

        voiceText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            voiceText.text += text[i];

            PlayTypewriterSound();

            yield return new WaitForSeconds(customTypingSpeed);
        }

        yield return new WaitForSeconds(customDisplayDuration);
    }

    // =========================================================
    // TYPEWRITER SOUND
    // =========================================================

    private void PlayTypewriterSound()
    {
        if (typewriterAudio == null || typewriterSound == null)
            return;

        typewriterAudio.PlayOneShot(typewriterSound);
    }
}


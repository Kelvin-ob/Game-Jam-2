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


    [Tooltip("Lautstärke des Typewriter-Sounds.")]
    [SerializeField][Range(0f, 1f)] private float typewriterVolume = 0.5f;

    private Coroutine currentRoutine;

    public bool IsVoiceActive { get; private set; }


    // =========================================================
    // AWAKE
    // =========================================================

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
        ShowVoice(
            text,
            typingSpeed,
            displayDuration
        );
    }


    public void ShowVoice(
        string text,
        float customTypingSpeed,
        float customDisplayDuration)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        StopTypewriterSound();

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

        voiceText.text = "";
        // =====================================================
        // TYPEWRITER
        // =====================================================

        for (int i = 0; i < text.Length; i++)
        {
            voiceText.text += text[i];


            // Typewriter-Sound
            if (!char.IsWhiteSpace(text[i]))
            {
                PlayTypewriterSound();
            }


            yield return new WaitForSeconds(customTypingSpeed);
        }


        // =====================================================
        // TEXT STEHEN LASSEN
        // =====================================================

        StopTypewriterSound();

        yield return new WaitForSeconds(
            customDisplayDuration
        );


        // =====================================================
        // ENDE
        // =====================================================

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
        {
            StopCoroutine(currentRoutine);
        }

        StopTypewriterSound();

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


        // =====================================================
        // DELAY
        // =====================================================

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }


        // =====================================================
        // DIALOGUE
        // =====================================================

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


            if (timeBetweenLines > 0f)
            {
                yield return new WaitForSeconds(
                    timeBetweenLines
                );
            }
        }


        // =====================================================
        // ENDE
        // =====================================================

        StopTypewriterSound();

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


            if (!char.IsWhiteSpace(text[i]))
            {
                PlayTypewriterSound();
            }


            yield return new WaitForSeconds(
                customTypingSpeed
            );
        }


        StopTypewriterSound();

        yield return new WaitForSeconds(
            customDisplayDuration
        );
    }


    // =========================================================
    // TYPEWRITER SOUND
    // =========================================================

    private void PlayTypewriterSound()
    {
        if (typewriterAudio == null)
            return;

        if (typewriterSound == null)
            return;

        typewriterAudio.PlayOneShot(
            typewriterSound,
            typewriterVolume
        );
    }


    // =========================================================
    // STOP TYPEWRITER
    // =========================================================

    private void StopTypewriterSound()
    {
        if (typewriterAudio == null)
            return;

        typewriterAudio.Stop();
    }
}


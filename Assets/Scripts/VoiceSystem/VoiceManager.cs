using UnityEngine;
using TMPro;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance { get; private set; }

    [SerializeField] private TMP_Text voiceText;

    [Header("Typewriter")]
    [SerializeField] private AudioSource typewriterAudio;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField] private float soundVolume = 0.5f;

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

    public void ShowVoice(string text, float typingSpeed, float displayDuration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(
            ShowVoiceRoutine(text, typingSpeed, displayDuration)
        );
    }

    private IEnumerator ShowVoiceRoutine(
        string text,
        float typingSpeed,
        float displayDuration)
    {
        IsVoiceActive = true;

        voiceText.text = "";
        voiceText.gameObject.SetActive(true);

        // Text Buchstabe für Buchstabe schreiben
        foreach (char letter in text)
        {
            voiceText.text += letter;

            // Sound abspielen
            if (typewriterAudio != null &&
                typewriterSound != null &&
                !char.IsWhiteSpace(letter))
            {
                typewriterAudio.PlayOneShot(
                    typewriterSound,
                    soundVolume
                );
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // Text sichtbar lassen
        yield return new WaitForSeconds(displayDuration);

        voiceText.gameObject.SetActive(false);

        IsVoiceActive = false;
        currentRoutine = null;
    }
}


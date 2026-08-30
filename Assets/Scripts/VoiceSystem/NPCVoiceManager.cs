using System.Collections;
using TMPro;
using UnityEngine;

public class NPCVoiceManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text npcText;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 0.5f;

    [Header("Typewriter Sound")]
    [SerializeField] private AudioSource typewriterAudio;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField][Range(0f, 1f)] private float typewriterVolume = 0.5f;

    public bool IsVoiceActive { get; private set; }

    // Nutzt die Standardwerte aus dem Inspector
    public void ShowDialogue(string[] lines)
    {
        ShowDialogue(lines, typingSpeed, displayDuration);
    }

    // Erlaubt individuelle Geschwindigkeit/Dauer pro Aufruf
    public void ShowDialogue(string[] lines, float customTypingSpeed, float customDisplayDuration)
    {
        if (lines == null || lines.Length == 0)
            return;

        StartCoroutine(PlayDialogue(lines, customTypingSpeed, customDisplayDuration));
    }

    private IEnumerator PlayDialogue(string[] lines, float customTypingSpeed, float customDisplayDuration)
    {
        IsVoiceActive = true;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (npcText == null)
            {
                Debug.LogWarning("NPCText is not assigned in NPCVoiceManager.");
                break;
            }

            npcText.gameObject.SetActive(true);
            npcText.text = "";

            if (typewriterAudio != null && typewriterSound != null)
            {
                typewriterAudio.clip = typewriterSound;
                typewriterAudio.volume = typewriterVolume;
                typewriterAudio.Play();
            }

            for (int i = 0; i < line.Length; i++)
            {
                npcText.text += line[i];
                yield return new WaitForSeconds(customTypingSpeed);
            }

            if (typewriterAudio != null)
            {
                typewriterAudio.Stop();
            }

            yield return new WaitForSeconds(customDisplayDuration);

            if (timeBetweenLines > 0f)
                yield return new WaitForSeconds(timeBetweenLines);
        }

        if (npcText != null)
            npcText.gameObject.SetActive(false);

        IsVoiceActive = false;
    }
}
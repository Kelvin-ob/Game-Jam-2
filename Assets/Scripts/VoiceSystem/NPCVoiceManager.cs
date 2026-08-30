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

    private Coroutine currentRoutine;

    public bool IsVoiceActive { get; private set; }

    public void ShowDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
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

            for (int i = 0; i < line.Length; i++)
            {
                npcText.text += line[i];
                PlayTypewriterSound();
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(displayDuration);

            if (timeBetweenLines > 0f)
                yield return new WaitForSeconds(timeBetweenLines);
        }

        if (npcText != null)
            npcText.gameObject.SetActive(false);

        IsVoiceActive = false;
        currentRoutine = null;
    }

    private void PlayTypewriterSound()
    {
        if (typewriterAudio == null || typewriterSound == null)
            return;

        typewriterAudio.PlayOneShot(typewriterSound);
    }
}

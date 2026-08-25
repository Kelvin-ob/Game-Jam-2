using UnityEngine;
using TMPro;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance { get; private set; }

    [SerializeField] private TMP_Text voiceText;
    [SerializeField] private float displayDuration = 3f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowVoice(string text)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowVoiceRoutine(text));
    }

    private IEnumerator ShowVoiceRoutine(string text)
    {
        voiceText.text = text;
        voiceText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        voiceText.gameObject.SetActive(false);

        currentRoutine = null;
    }
}
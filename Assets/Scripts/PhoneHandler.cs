using System.Collections;
using DialogueEditor;
using UnityEngine;

public class PhoneHandler : MonoBehaviour, IInteractable
{
    [Header("Generator Check")]
    [SerializeField] private string generatorId = "generator_01";

    [Header("Interaction")]
    [SerializeField] private NPCConversation phoneConversation;
    [SerializeField] private string promptText = "Press E to answer";

    [Header("Ring Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ringClip;
    [SerializeField] private float ringInterval = 1.3f;

    private bool hasAnswered = false;
    private bool isRinging = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        TryStartRingingIfGeneratorActive();
    }

    private void Update()
    {
        if (hasAnswered)
            return;

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsGeneratorActivated(generatorId))
        {
            StartRinging();
        }
        else if (isRinging)
        {
            StopRinging();
        }
    }

    public void Interact()
    {
        if (hasAnswered)
            return;

        hasAnswered = true;
        StopRinging();

        if (phoneConversation != null && ConversationManager.Instance != null)
        {
            ConversationManager.Instance.StartConversation(phoneConversation);
        }
        else
        {
            Debug.LogWarning("PhoneConversation or ConversationManager not assigned.");
        }
    }

    public void OnFocus()
    {
        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.hidePrompt();
    }

    private void TryStartRingingIfGeneratorActive()
    {
        if (hasAnswered)
            return;

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsGeneratorActivated(generatorId))
        {
            StartRinging();
        }
    }

    private void StartRinging()
    {
        if (hasAnswered || isRinging)
            return;

        isRinging = true;

        if (audioSource != null && ringClip != null)
        {
            audioSource.clip = ringClip;
            audioSource.loop = true;
            audioSource.Play();
            return;
        }

        StartCoroutine(RingLoop());
    }

    private void StopRinging()
    {
        isRinging = false;

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        StopCoroutine(RingLoop());
    }

    private IEnumerator RingLoop()
    {
        while (isRinging)
        {
            if (audioSource != null && !audioSource.isPlaying && ringClip != null)
            {
                audioSource.clip = ringClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            yield return new WaitForSeconds(ringInterval);
        }
    }
}

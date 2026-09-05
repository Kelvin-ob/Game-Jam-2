using System.Collections;
using UnityEngine;

public class PhoneHandler : MonoBehaviour, IInteractable
{
    [Header("Generator Check")]
    [SerializeField] private string generatorId = "generator_01";

    [Header("Dialogue")]
    [SerializeField] private NPCVoiceManager npcVoiceManager;
    [SerializeField] private Player player; 
    [TextArea(2, 5)]
    [SerializeField] private string[] phoneDialogueLines;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 1f;

    [Header("Interaction")]
    [SerializeField] private string promptText = "Press E to answer";

    [Header("Ring Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ringClip;
    [SerializeField] private float ringInterval = 1.3f;

    private bool hasAnswered = false;
    private bool isRinging = false;
    private Coroutine ringRoutine;

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

        bool generatorActive = GameStateManager.Instance != null &&
            GameStateManager.Instance.IsGeneratorActivated(generatorId);

        if (generatorActive)
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

        if (npcVoiceManager == null)
        {
            Debug.LogWarning("NPCVoiceManager is missing for phone dialogue.");
            return;
        }

        StartCoroutine(PlayPhoneDialogueRoutine());
    }

    private IEnumerator PlayPhoneDialogueRoutine()
    {
        if (player != null)
        {
            player.SetMovementEnabled(false);
        }

        npcVoiceManager.ShowDialogue(phoneDialogueLines);

        // warten, bis der Dialog fertig ist
        yield return new WaitUntil(() => !npcVoiceManager.IsVoiceActive);

        if (player != null)
        {
            player.SetMovementEnabled(true);
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
        }

        if (ringRoutine == null)
            ringRoutine = StartCoroutine(RingLoop());
    }

    private void StopRinging()
    {
        isRinging = false;

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        if (ringRoutine != null)
        {
            StopCoroutine(ringRoutine);
            ringRoutine = null;
        }
    }

    private IEnumerator RingLoop()
    {
        while (isRinging)
        {
            if (audioSource != null && ringClip != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = ringClip;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }

            yield return new WaitForSeconds(ringInterval);
        }

        ringRoutine = null;
    }
}

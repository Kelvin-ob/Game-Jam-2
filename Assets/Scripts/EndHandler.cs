using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EndHandler : MonoBehaviour, IInteractable
{
    [Header("Voice Dialog")]
    [SerializeField] private VoiceManager voiceManager;
    [TextArea(2, 5)]
    [SerializeField] private string[] endDialogueLines;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float timeBetweenLines = 1f;

    [Header("Interaction")]
    [SerializeField] private string promptText = "Hold E to access terminal";
    [SerializeField] private float activationDuration = 5f;
    [SerializeField] private bool triggerOnce = true;

    [Header("Progress UI")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private Image progressCircle;

    [Header("Sound")]
    [SerializeField] private AudioClip activateSound;
    [SerializeField][Range(0f, 1f)] private float activateVolume = 1f;

    private AudioSource audioSource;
    private Renderer[] renderers;

    private Player player;
    private bool hasTriggered;
    private bool isFocused;
    private bool isActivating;
    private float activationProgress;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = activateSound;
        audioSource.volume = activateVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (voiceManager == null)
            voiceManager = GetComponent<VoiceManager>();

        if (voiceManager == null)
            voiceManager = FindFirstObjectByType<VoiceManager>();

        player = FindFirstObjectByType<Player>();
        renderers = GetComponentsInChildren<Renderer>(true);
        SetScreenVisible(false);

        if (progressUI != null)
            progressUI.SetActive(false);

        if (progressCircle != null)
            progressCircle.fillAmount = activationProgress;
    }

    private void Update()
    {
        if (!isFocused || !isActivating || Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.isPressed)
        {
            activationProgress += Time.deltaTime / activationDuration;
            activationProgress = Mathf.Clamp01(activationProgress);

            if (progressCircle != null)
                progressCircle.fillAmount = activationProgress;

            if (activationProgress >= 1f)
                FinishInteraction();
        }
        else
        {
            PauseInteraction();
        }
    }

    public void Interact()
    {
        if ((triggerOnce && hasTriggered) || isActivating)
            return;

        isActivating = true;

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.showPrompt("Granting access...");

        if (progressUI != null)
            progressUI.SetActive(true);

        if (progressCircle != null)
            progressCircle.fillAmount = activationProgress;

        if (audioSource != null && activateSound != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void OnFocus()
    {
        isFocused = true;

        if (hasTriggered)
            return;

        if (VoiceManager.Instance != null && VoiceManager.Instance.IsVoiceActive)
            return;

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        isFocused = false;

        if (hasTriggered)
            return;

        if (isActivating)
            PauseInteraction();

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.hidePrompt();
    }

    private void PauseInteraction()
    {
        isActivating = false;

        if (audioSource != null)
            audioSource.Stop();

        if (progressUI != null)
            progressUI.SetActive(false);

        if (isFocused && InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.showPrompt(promptText);
    }

    private void FinishInteraction()
    {
        isActivating = false;
        hasTriggered = true;

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.hidePrompt();

        if (audioSource != null)
            audioSource.Stop();

        if (progressUI != null)
            progressUI.SetActive(false);

        if (progressCircle != null)
            progressCircle.fillAmount = 1f;

        if (player != null)
            player.SetMovementEnabled(false);

        StartCoroutine(PlayEndVoiceAndQuit());
    }

    private IEnumerator PlayEndVoiceAndQuit()
    {
        var manager = voiceManager != null ? voiceManager : VoiceManager.Instance;
        SetScreenVisible(true);

        if (endDialogueLines == null || endDialogueLines.Length == 0 || manager == null)
        {
            QuitGame();
            yield break;
        }

        manager.StartPickupDialogue(
            endDialogueLines,
            0f,
            typingSpeed,
            displayDuration,
            timeBetweenLines
        );

        yield return new WaitUntil(() => !manager.IsVoiceActive);

        QuitGame();
    }

     private void SetScreenVisible(bool visible)
    {
        if (renderers == null)
            return;

        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

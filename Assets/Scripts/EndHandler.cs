using System.Collections;
using UnityEngine;

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
    [SerializeField] private string promptText = "Press E to finish";
    [SerializeField] private bool triggerOnce = true;

    private Player player;
    private bool hasTriggered;

    private void Start()
    {
        if (voiceManager == null)
            voiceManager = GetComponent<VoiceManager>();

        if (voiceManager == null)
            voiceManager = FindFirstObjectByType<VoiceManager>();

        player = FindFirstObjectByType<Player>();
    }

    public void Interact()
    {
        if (triggerOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (player != null)
            player.SetMovementEnabled(false);

        StartCoroutine(PlayEndVoiceAndQuit());
    }

    public void OnFocus()
    {
        if (hasTriggered)
            return;

        if (VoiceManager.Instance != null && VoiceManager.Instance.IsVoiceActive)
            return;

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        if (hasTriggered)
            return;

        if (InteractPromptManager.Instance != null)
            InteractPromptManager.Instance.hidePrompt();
    }

    private IEnumerator PlayEndVoiceAndQuit()
    {
        var manager = voiceManager != null ? voiceManager : VoiceManager.Instance;

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

    private void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

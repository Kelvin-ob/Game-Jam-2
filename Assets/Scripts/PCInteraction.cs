using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PCInteraction : MonoBehaviour, IInteractable
{
    [Header("Keycard")]
    [SerializeField] private string requiredItemId = "keycard";

    [Header("Generator Requirement")]
    [SerializeField] private string generatorId = "generator_01";
    [SerializeField] private string generatorRequiredText = "The generator needs to be running.";

    [Header("Activation")]
    [SerializeField] private float activationDuration = 5f;

    [Header("Interaction")]
    [SerializeField] private string promptText = "Hold E to activate keycard";
    [SerializeField] private string activatedText = "Keycard already activated";
    [SerializeField] private string missingCardText = "I need a keycard.";

    [Header("Progress UI")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private Image progressCircle;

    [Header("Sound")]
    [SerializeField] private AudioClip activateSound;
    [SerializeField][Range(0f, 1f)] private float activateVolume = 1f;

    private AudioSource audioSource;

    private bool isFocused = false;
    private bool isActivating = false;
    private bool isActivated = false;

    private float activationProgress = 0f;

    private void Start()
    {
        // ==========================================
        // AUDIO SOURCE ERSTELLEN
        // ==========================================

        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = activateSound;
        audioSource.volume = activateVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // ==========================================
        // KEYCARD STATUS PRÜFEN
        // ==========================================

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsKeycardActivated())
        {
            isActivated = true;
            activationProgress = 1f;
        }

        // ==========================================
        // PROGRESS UI VERSTECKEN
        // ==========================================

        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        if (progressCircle != null)
        {
            progressCircle.fillAmount = activationProgress;
        }
    }

    private void Update()
    {
        if (!isFocused)
            return;

        // Keycard bereits aktiviert
        if (isActivated)
            return;

        // Aktivierung läuft
        if (isActivating)
        {
            HandleActivation();
        }
    }

    public void Interact()
    {
        // ==========================================
        // KEYCARD BEREITS AKTIVIERT
        // ==========================================

        if (isActivated)
        {
            InteractPromptManager.Instance.showPrompt(activatedText);
            return;
        }

        // ==========================================
        // GENERATOR PRÜFEN
        // ==========================================

        if (GameStateManager.Instance == null ||
            !GameStateManager.Instance.IsGeneratorActivated(generatorId))
        {
            InteractPromptManager.Instance.showPrompt(generatorRequiredText);
            return;
        }

        // ==========================================
        // KEYCARD PRÜFEN
        // ==========================================

        if (InventoryManager.Instance == null ||
            !InventoryManager.Instance.HasItem(requiredItemId))
        {
            InteractPromptManager.Instance.showPrompt(missingCardText);
            return;
        }

        // ==========================================
        // AKTIVIERUNG STARTEN
        // ==========================================

        isActivating = true;

        // Fortschritt NICHT zurücksetzen!
        // Dadurch kann man nach dem Loslassen
        // später weitermachen.

        if (progressUI != null)
        {
            progressUI.SetActive(true);
        }

        if (progressCircle != null)
        {
            progressCircle.fillAmount = activationProgress;
        }

        InteractPromptManager.Instance.showPrompt("Activating...");

        // ==========================================
        // SOUND STARTEN
        // ==========================================

        if (audioSource != null &&
            activateSound != null &&
            !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void HandleActivation()
    {
        if (Keyboard.current == null)
            return;

        // ==========================================
        // E WIRD GEHALTEN
        // ==========================================

        if (Keyboard.current.eKey.isPressed)
        {
            activationProgress += Time.deltaTime / activationDuration;

            activationProgress = Mathf.Clamp01(activationProgress);

            // Progress Circle aktualisieren
            if (progressCircle != null)
            {
                progressCircle.fillAmount = activationProgress;
            }

            // ==========================================
            // AKTIVIERUNG FERTIG
            // ==========================================

            if (activationProgress >= 1f)
            {
                FinishActivation();
            }
        }
        else
        {
            // ==========================================
            // E LOSGELASSEN
            // ==========================================

            PauseActivation();
        }
    }

    private void PauseActivation()
    {
        isActivating = false;

        // ==========================================
        // SOUND STOPPEN
        // ==========================================

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // ==========================================
        // UI AUSBLENDEN
        // ==========================================

        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        // Fortschritt bleibt erhalten!

        if (isFocused)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    private void FinishActivation()
    {
        isActivating = false;
        isActivated = true;
        activationProgress = 1f;

        // ==========================================
        // SOUND STOPPEN
        // ==========================================

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // ==========================================
        // UI AUSBLENDEN
        // ==========================================

        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        if (progressCircle != null)
        {
            progressCircle.fillAmount = 1f;
        }

        // ==========================================
        // KEYCARD STATUS SPEICHERN
        // ==========================================

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetKeycardActivated();
        }

        // ==========================================
        // BESTÄTIGUNG
        // ==========================================

        InteractPromptManager.Instance.showPrompt(
            "Keycard activated"
        );
    }

    public void OnFocus()
    {
        isFocused = true;

        // ==========================================
        // KEYCARD BEREITS AKTIVIERT
        // ==========================================

        if (isActivated)
        {
            InteractPromptManager.Instance.showPrompt(activatedText);
            return;
        }

        // ==========================================
        // GENERATOR NOCH NICHT AN
        // ==========================================

        if (GameStateManager.Instance == null ||
            !GameStateManager.Instance.IsGeneratorActivated(generatorId))
        {
            InteractPromptManager.Instance.showPrompt(generatorRequiredText);
            return;
        }

        // ==========================================
        // GENERATOR LÄUFT → KEYCARD PRÜFEN
        // ==========================================

        if (InventoryManager.Instance == null ||
            !InventoryManager.Instance.HasItem(requiredItemId))
        {
            InteractPromptManager.Instance.showPrompt(missingCardText);
            return;
        }

        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        isFocused = false;

        if (isActivating)
        {
            PauseActivation();
        }

        InteractPromptManager.Instance.hidePrompt();
    }
}


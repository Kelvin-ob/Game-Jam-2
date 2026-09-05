using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class GeneratorInteraction : MonoBehaviour, IInteractable
{
    [Header("Generator")]
    [SerializeField] private string generatorId = "generator_01";
    [SerializeField] private float refuelDuration = 10f;

    [Header("Required Item")]
    [SerializeField] private string requiredItemId = "gasoline";
    [SerializeField] private string missingItemMessage = "I need gasoline.";

    [Header("Interaction")]
    [SerializeField] private string promptText = "Hold E to refuel";
    [SerializeField] private string fullText = "Generator is full";
    [SerializeField] private string activateText = "Press E to start generator";
    [SerializeField] private string activatedText = "Generator is running";

    [Header("Progress UI")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private Image progressCircle;

    [Header("Refuel Sound")]
    [SerializeField] private AudioSource refuelAudioSource;

    [Header("Complete Sound")]
    [SerializeField] private AudioClip completeSound;
    [SerializeField][Range(0f, 1f)] private float completeVolume = 1f;

    [Header("Activate Sound")]
    [SerializeField] private AudioClip activateSound;
    [SerializeField][Range(0f, 1f)] private float activateVolume = 1f;

    private bool isFocused = false;
    private bool isRefueling = false;

    // Tank ist voll
    private bool isFilled = false;

    // Generator wurde angeschaltet
    private bool isActivated = false;

    // 0 = leer, 1 = voll
    private float refuelProgress = 0f;

    private void Start()
    {
        // Prüfen, ob Generator bereits voll ist
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsGeneratorFilled(generatorId))
        {
            isFilled = true;
            refuelProgress = 1f;
        }

        // Prüfen, ob Generator bereits angeschaltet wurde
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsGeneratorActivated(generatorId))
        {
            isActivated = true;
            isFilled = true;
            refuelProgress = 1f;
        }

        // Progress UI am Anfang verstecken
        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        // Progress Kreis setzen
        if (progressCircle != null)
        {
            progressCircle.fillAmount = refuelProgress;
        }

        // Refuel Sound vorbereiten
        if (refuelAudioSource != null)
        {
            refuelAudioSource.playOnAwake = false;
            refuelAudioSource.loop = false;
        }
    }

    private void Update()
    {
        if (!isFocused)
            return;

        // Generator läuft bereits
        if (isActivated)
            return;

        // Auftanken
        if (isRefueling)
        {
            HandleRefueling();
        }
    }

    public void Interact()
    {
        // ==========================================
        // GENERATOR LÄUFT BEREITS
        // ==========================================

        if (isActivated)
        {
            InteractPromptManager.Instance.showPrompt(activatedText);
            return;
        }

        // ==========================================
        // TANK IST VOLL → GENERATOR ANSCHALTEN
        // ==========================================

        if (isFilled)
        {
            ActivateGenerator();
            return;
        }

        // ==========================================
        // BENZIN PRÜFEN
        // ==========================================

        if (InventoryManager.Instance == null ||
            !InventoryManager.Instance.HasItem(requiredItemId))
        {
            StartCoroutine(ShowMissingItemMessage());
            return;
        }

        // ==========================================
        // AUFTANKEN STARTEN
        // ==========================================

        isRefueling = true;

        if (progressUI != null)
        {
            progressUI.SetActive(true);
        }

        if (progressCircle != null)
        {
            progressCircle.fillAmount = refuelProgress;
        }

        // Sound an der richtigen Position starten
        UpdateRefuelSound();

        InteractPromptManager.Instance.showPrompt("Refueling...");
    }

    private void HandleRefueling()
    {
        if (Keyboard.current == null)
            return;

        // ==========================================
        // E WIRD GEHALTEN
        // ==========================================

        if (Keyboard.current.eKey.isPressed)
        {
            refuelProgress += Time.deltaTime / refuelDuration;
            refuelProgress = Mathf.Clamp01(refuelProgress);

            // Kreis aktualisieren
            if (progressCircle != null)
            {
                progressCircle.fillAmount = refuelProgress;
            }

            // Sound exakt an Progress anpassen
            UpdateRefuelSound();

            // Fertig
            if (refuelProgress >= 1f)
            {
                FinishRefueling();
            }
        }
        else
        {
            // ==========================================
            // E LOSGELASSEN
            // ==========================================

            PauseRefueling();
        }
    }

    private void UpdateRefuelSound()
    {
        if (refuelAudioSource == null ||
            refuelAudioSource.clip == null)
            return;

        if (refuelAudioSource.clip.length <= 0.01f)
            return;

        float maxValidTime = Mathf.Max(0f, refuelAudioSource.clip.length - 0.01f);
        float targetTime = refuelProgress * refuelAudioSource.clip.length;

        refuelAudioSource.time = Mathf.Clamp(targetTime, 0f, maxValidTime);

        if (!refuelAudioSource.isPlaying)
        {
            refuelAudioSource.Play();
        }
    }

    private void PauseRefueling()
    {
        isRefueling = false;

        // Sound pausieren
        if (refuelAudioSource != null &&
            refuelAudioSource.isPlaying)
        {
            refuelAudioSource.Pause();
        }

        // UI verstecken
        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        if (isFocused)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    private void FinishRefueling()
    {
        isRefueling = false;
        isFilled = true;
        refuelProgress = 1f;

        if (refuelAudioSource != null && refuelAudioSource.clip != null)
        {
            float maxValidTime = Mathf.Max(0f, refuelAudioSource.clip.length - 0.01f);
            refuelAudioSource.time = maxValidTime;

            if (refuelAudioSource.isPlaying)
            {
                refuelAudioSource.Stop();
            }
        }

        // UI ausblenden
        if (progressUI != null)
        {
            progressUI.SetActive(false);
        }

        if (progressCircle != null)
        {
            progressCircle.fillAmount = 1f;
        }

        // Generator als voll speichern
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetGeneratorFilled(generatorId);
        }

        // Benzin entfernen
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem(requiredItemId);
        }

        // Fertig-Sound
        PlaySound(completeSound, completeVolume);

        // Generator NOCH NICHT anschalten
        InteractPromptManager.Instance.showPrompt(activateText);

        string[] endDialogue = { "Well done.", "Turn it on and go back to the vent." };
        VoiceManager.Instance.StartPickupDialogue(endDialogue, 0.5f, 0.05f, 1.5f, 0.5f);
    }

    private void ActivateGenerator()
    {
        // Generator wird dauerhaft angeschaltet
        isActivated = true;

        // Zustand speichern
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetGeneratorActivated(generatorId);
        }

        // Aktivierungs-Sound
        PlaySound(activateSound, activateVolume);

        InteractPromptManager.Instance.showPrompt(activatedText);

        // Hier kannst du später:
        // - Generator-Animation starten
        // - Licht anschalten
        // - Strom anschalten
        // - andere Systeme aktivieren
    }

    private IEnumerator ShowMissingItemMessage()
    {
        InteractPromptManager.Instance.showPrompt(missingItemMessage);

        yield return new WaitForSeconds(2f);

        if (isFocused)
        {
            if (isActivated)
            {
                InteractPromptManager.Instance.showPrompt(activatedText);
            }
            else if (isFilled)
            {
                InteractPromptManager.Instance.showPrompt(activateText);
            }
            else
            {
                InteractPromptManager.Instance.showPrompt(promptText);
            }
        }
        else
        {
            InteractPromptManager.Instance.hidePrompt();
        }
    }

    public void OnFocus()
    {
        isFocused = true;

        if (isActivated)
        {
            InteractPromptManager.Instance.showPrompt(activatedText);
        }
        else if (isFilled)
        {
            InteractPromptManager.Instance.showPrompt(activateText);
        }
        else
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnLoseFocus()
    {
        isFocused = false;

        if (isRefueling)
        {
            PauseRefueling();
        }

        InteractPromptManager.Instance.hidePrompt();
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        AudioSource.PlayClipAtPoint(
            clip,
            transform.position,
            volume
        );
    }
}


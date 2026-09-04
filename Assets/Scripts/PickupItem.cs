using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;
    [SerializeField] private GunHandler gunToEnable;

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text";

    [SerializeField] private VoiceTrigger keycardVoiceTrigger;

    [Header("Pickup Sound")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField][Range(0f, 1f)] private float pickupVolume = 1f;

    private void Start()
    {
        // Wenn bereits eingesammelt → Objekt entfernen
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsItemCollected(itemId))
        {

            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        // ==========================================
        // ITEM INS INVENTAR HINZUFÜGEN
        // ==========================================

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemId);
        }

        // ==========================================
        // ITEM ALS EINGESAMMELT SPEICHERN
        // ==========================================

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetItemCollected(itemId);
        }

        // ==========================================
        // VOICE TRIGGER
        // ==========================================

        if (keycardVoiceTrigger != null)
        {
            keycardVoiceTrigger.TriggerVoice();
        }

        // ==========================================
        // PICKUP SOUND
        // ==========================================

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupSound,
                transform.position,
                pickupVolume
            );
        }

        // ==========================================
        // GUN AKTIVIEREN
        // ==========================================

        if (gunToEnable != null)
        {
            gunToEnable.Pickup();
        }

        // ==========================================
        // PROMPT ENTFERNEN
        // ==========================================

        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.hidePrompt();
        }

        // ==========================================
        // ITEM AUS SZENE ENTFERNEN
        // ==========================================

        Destroy(gameObject);
    }

    public void OnFocus()
    {
        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnLoseFocus()
    {
        if (InteractPromptManager.Instance != null)
        {
            InteractPromptManager.Instance.hidePrompt();
        }
    }
}


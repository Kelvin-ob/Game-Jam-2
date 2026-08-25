using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId; //example: "secret_room_key"
    [SerializeField] private GameObject interactPrompt;


    void Start()
    {
        interactPrompt.SetActive(false);
    }

    public void Interact()
    {
        InventoryManager.Instance.AddItem(itemId);
        Destroy(gameObject); // item aus welt -- nicht gelöscht!! nur nicht sichtbar
    }

    public void OnFocus()
    {
        interactPrompt.SetActive(true);
    }

    public void OnLoseFocus()
    {
        interactPrompt.SetActive(false);
    }
}

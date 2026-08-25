using UnityEngine;

public class DoorTest : MonoBehaviour, IInteractable
{
    [SerializeField] private string requiredItemId; // example: "secret_door_key"

    public void Interact()
    {
        if (InventoryManager.Instance.HasItem(requiredItemId))
        {
            Debug.Log("Door is opeeenn");
            //more here (animation trigger oder so)
        } else
        {
            Debug.Log("You need the secret door key");
        }
    }

    public void OnFocus()
    {

    }
    public void OnLoseFocus()
    {

    }
}

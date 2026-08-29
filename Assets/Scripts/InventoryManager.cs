using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemId)
    {
        collectedItems.Add(itemId);
        Debug.Log("Item added: " + itemId);
    }

    public bool HasItem(string itemId)
    {
        return collectedItems.Contains(itemId);
    }

    public void RemoveItem(string itemId)
    {
        if (collectedItems.Contains(itemId))
        {
            collectedItems.Remove(itemId);
            Debug.Log("Item removed: " + itemId);
        }
    }
}
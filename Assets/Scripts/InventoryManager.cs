using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private HashSet<string> collectedItems = new HashSet<string>();

    public static InventoryManager EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        GameObject managerObject = new GameObject("InventoryManager");
        Instance = managerObject.AddComponent<InventoryManager>();
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
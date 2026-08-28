using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; //global access thats why we use static // instnace is the whole class so that multiple obj can make use of it bc: we can have 20+ items but they all need reference to Inventory

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
}

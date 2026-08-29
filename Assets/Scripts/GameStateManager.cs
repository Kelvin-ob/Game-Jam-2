using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private HashSet<string> collectedItems = new HashSet<string>();
    private HashSet<string> unlockedDoors = new HashSet<string>();

    public static GameStateManager EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        GameObject managerObject = new GameObject("GameStateManager");
        Instance = managerObject.AddComponent<GameStateManager>();
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

    // =========================
    // ITEMS
    // =========================

    public void SetItemCollected(string itemId)
    {
        collectedItems.Add(itemId);
    }

    public bool IsItemCollected(string itemId)
    {
        return collectedItems.Contains(itemId);
    }

    // =========================
    // DOORS
    // =========================

    public void SetDoorUnlocked(string doorId)
    {
        unlockedDoors.Add(doorId);
    }

    public bool IsDoorUnlocked(string doorId)
    {
        return unlockedDoors.Contains(doorId);
    }
}
using UnityEngine;

public class DoorChase : MonoBehaviour
{
    [SerializeField] private bool openDoor;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private NPC npc;


    private void Awake()
    {
        ApplyDoorState();
    }

    private void Update()
    {
        ApplyDoorState();
        openDoor = npc.openDoor;
    }

    private void ApplyDoorState()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = !openDoor;

        if (boxCollider != null)
            boxCollider.enabled = !openDoor;
    }
}

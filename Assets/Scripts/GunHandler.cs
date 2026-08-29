using UnityEngine;
using UnityEngine.InputSystem;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 50f;
    [SerializeField] private bool isPickedUp;

    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        SetWeaponVisible(false);
        enabled = false;
    }

    public void Pickup()
    {
        isPickedUp = true;
        SetWeaponVisible(true);
        enabled = true;
    }

    public bool IsPickedUp()
    {
        return isPickedUp;
    }

    private void Update()
    {
        if (!isPickedUp)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            NPC npc = hit.collider.GetComponentInParent<NPC>();
            if (npc != null)
            {
                npc.TakeDamage(1);
            }
        }
    }

    private void SetWeaponVisible(bool visible)
    {
        if (renderers == null)
            return;

        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }
}

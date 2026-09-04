using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 50f;
    [SerializeField] private bool isPickedUp;
    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 hitImpulseForce = new Vector3(1f, 1f, 0f);

    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        isPickedUp = false;
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
        if (!isPickedUp)
        {
            SetWeaponVisible(false);
            enabled = false;
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
                if (animator != null)
                {
                    animator.SetTrigger("HasShot");
                }

                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulse(hitImpulseForce);
                }

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

    public bool GetIsPickedUp()
    {
        return isPickedUp;
    }

    public void SetIsPickedUp(bool newisPickedUp)
    {
        isPickedUp = newisPickedUp;
    }
}

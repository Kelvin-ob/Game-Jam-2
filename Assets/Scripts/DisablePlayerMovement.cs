using UnityEngine;

public class DisablePlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Start()
    {
        player.SetMovementEnabled(false);
    }
}
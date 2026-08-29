using UnityEngine;

public class NPCStopTrigger : MonoBehaviour
{
    [SerializeField] private NPC enemy;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || enemy == null)
            return;

        NPC otherNpc = other.GetComponentInParent<NPC>();

        if (otherNpc != enemy)
            return;

        triggered = true;
        enemy.stopTrigger();
    }
}

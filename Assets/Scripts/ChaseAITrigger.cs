using UnityEngine;

public class ChaseAITrigger : MonoBehaviour
{
    [SerializeField] private NPC enemy;
    [SerializeField] private VoiceTrigger aiVoiceTrigger;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        // Nur während der Chase auslösen
        if (enemy == null || !enemy.IsChasing())
            return;

        triggered = true;

        if (aiVoiceTrigger != null)
        {
            aiVoiceTrigger.TriggerVoice();
        }
    }
}
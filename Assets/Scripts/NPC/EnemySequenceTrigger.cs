using UnityEngine;

public class EnemySequenceTrigger : MonoBehaviour
{
    [SerializeField] private EnemyAI enemy;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        enemy.StartSequence();
    }
}
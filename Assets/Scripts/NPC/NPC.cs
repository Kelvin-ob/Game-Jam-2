using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Jumpscare,
        Talking,
        Chasing,
        Dead
    }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Jumpscare")]
    [SerializeField] private Transform jumpscarePosition;
    [SerializeField] private float jumpscareDuration = 1.5f;
    [SerializeField] private float jumpscareMoveSpeed = 18f;

    [Header("Player")]
    [SerializeField] private Transform respawnPoint;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.5f;

    [Header("Health")]
    [SerializeField] private int health = 1;

    [Header("Death / Ragdoll")]
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Collider mainCollider;

    private EnemyState currentState = EnemyState.Idle;

    private bool sequenceStarted = false;


    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObject = GameObject.Find("PlayerObj");

            if (playerObject != null)
                player = playerObject.transform;
        }

        // Ragdoll deaktivieren
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = false;
        }

        // Startzustand
        SetRunning(false);
    }


    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
            case EnemyState.Jumpscare:
            case EnemyState.Talking:
            case EnemyState.Dead:
                SetRunning(false);
                break;

            case EnemyState.Chasing:
                SetRunning(true);
                ChasePlayer();

                if (IsPlayerCaughtByDistance())
                    PlayerCaught();

                break;
        }
    }


    // =========================================================
    // SEQUENCE
    // =========================================================

    public void StartSequence()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;

        StartCoroutine(JumpscareSequence());
    }


    private System.Collections.IEnumerator JumpscareSequence()
    {
        currentState = EnemyState.Jumpscare;
        SetRunning(false);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (jumpscarePosition != null)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            float moveDuration = 0.2f;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                transform.position = Vector3.Lerp(startPosition, jumpscarePosition.position, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = jumpscarePosition.position;
            transform.rotation = jumpscarePosition.rotation;

            if (agent != null)
                agent.Warp(jumpscarePosition.position);
        }

        Debug.Log("JUMPSCARE!");

        yield return new WaitForSeconds(jumpscareDuration);

        StartTalking();
    }


    // =========================================================
    // TALKING
    // =========================================================

    private void StartTalking()
    {
        currentState = EnemyState.Talking;

        SetRunning(false);

        agent.isStopped = true;

        Debug.Log("Enemy beginnt zu reden.");

        // TEMPORÄR:
        // Später hier dein Voice-/Dialogue-System aufrufen.
        Invoke(nameof(StartChase), 3f);
    }


    // =========================================================
    // CHASE
    // =========================================================

    public void StartChase()
    {
        if (currentState == EnemyState.Dead)
            return;

        CancelInvoke(nameof(StartChase));

        currentState = EnemyState.Chasing;

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        SetRunning(true);

        Debug.Log("CHASE START!");
    }


    private void ChasePlayer()
    {
        if (player == null)
            return;

        if (!agent.enabled)
            return;

        agent.SetDestination(player.position);
    }


    // =========================================================
    // PLAYER CAUGHT
    // =========================================================

    private bool IsPlayerCaughtByDistance()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= catchDistance;
    }

    public void PlayerCaught()
    {
        if (currentState != EnemyState.Chasing)
            return;

        Debug.Log("Spieler wurde erwischt!");

        RespawnPlayer();
    }


    private void RespawnPlayer()
    {
        if (player == null || respawnPoint == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = respawnPoint.position;
        player.rotation = respawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        Debug.Log("Spieler respawnt!");

        ResetNpcToJumpscarePosition();
    }

    private void ResetNpcToJumpscarePosition()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        currentState = EnemyState.Jumpscare;
        SetRunning(false);
        sequenceStarted = false;

        Debug.Log("NPC zurück zur Jumpscare-Position!");

        StartSequence();
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead)
            return;

        health -= damage;

        Debug.Log("Enemy Damage: " + damage);

        if (health <= 0)
        {
            Die();
        }
    }


    // =========================================================
    // DEATH
    // =========================================================

    private void Die()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Dead;

        SetRunning(false);

        CancelInvoke();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (animator != null)
            animator.enabled = false;

        EnableRagdoll();

        Debug.Log("Enemy ist tot!");
    }


    private void EnableRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }
    }


    // =========================================================
    // ANIMATION
    // =========================================================

    private void SetRunning(bool running)
    {
        if (animator == null)
            return;

        animator.SetBool("IsRunning", running);
    }


    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (jumpscarePosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jumpscarePosition.position, 0.3f);
        }

        if (respawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(respawnPoint.position, 0.3f);
        }
    }
}
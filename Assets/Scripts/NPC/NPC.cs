using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    [Header("Jumpscare")]
    [SerializeField] private Transform jumpscarePosition;
    [SerializeField] private float jumpscareDuration = 1.5f;

    [Header("Player")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private MonoBehaviour playerMovement;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.2f;

    [Header("Health")]
    [SerializeField] private int health = 1;

    [Header("Death")]
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider mainCollider;

    private EnemyState currentState = EnemyState.Idle;

    private bool sequenceStarted = false;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.Find("PlayerObj").transform;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Jumpscare:
                break;

            case EnemyState.Talking:
                break;

            case EnemyState.Chasing:
                ChasePlayer();
                break;

            case EnemyState.Dead:
                break;
        }
    }

    // Wird vom Trigger aufgerufen
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

        agent.isStopped = true;

        // Enemy an Jumpscare-Position setzen
        if (jumpscarePosition != null)
        {
            transform.position = jumpscarePosition.position;
            transform.rotation = jumpscarePosition.rotation;
        }

        // Hier später Jumpscare-Animation/Sound
        Debug.Log("JUMPSCARE!");

        yield return new WaitForSeconds(jumpscareDuration);

        StartTalking();
    }

    private void StartTalking()
    {
        currentState = EnemyState.Talking;

        Debug.Log("Enemy beginnt zu reden.");

        // HIER dein Voice/Dialogue-System starten.
        //
        // Wenn dein Dialogue fertig ist:
        // StartChase();

        // Zum Testen:
        Invoke(nameof(StartChase), 3f);
    }

    public void StartChase()
    {
        if (jumpscarePosition != null)
        {
            transform.position = jumpscarePosition.position;
            transform.rotation = jumpscarePosition.rotation;
        }
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Chasing;

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        Debug.Log("CHASE START!");
    }

    private void ChasePlayer()
    {
        if (player == null)
            return;

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            PlayerCaught();
        }
    }

    // Wird aufgerufen, wenn Enemy den Spieler berührt
    private void OnCollisionEnter(Collision collision)
    {
        TryCatchPlayer(collision.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCatchPlayer(other.transform);
    }

    private void TryCatchPlayer(Transform other)
    {
        if (currentState != EnemyState.Chasing)
            return;

        if (other.CompareTag("Player") || other.root.CompareTag("Player"))
        {
            PlayerCaught();
        }
    }

    private void PlayerCaught()
    {
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

        // Chase wieder starten
        StartChase();
    }

    // Wird von der Waffe aufgerufen
    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentState = EnemyState.Dead;

        Debug.Log("Enemy tot!");

        agent.isStopped = true;
        agent.enabled = false;

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (animator != null)
            animator.enabled = false;

        EnableRagdoll();
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
}
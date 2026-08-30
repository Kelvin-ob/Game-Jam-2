using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
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
    public bool openDoor;

    [System.Serializable]
    public class DialogueLine
    {
        public bool isAI; // true = VoiceManager (AI im Kopf), false = NPCVoiceManager (echte Person)
        [TextArea(2, 4)]
        public string text;
    }

    [Header("Talking")]
    [SerializeField] private NPCVoiceManager npcVoiceManager; // NEU: Referenz
    [SerializeField] private DialogueLine[] talkingDialogue; // NEU: abwechselnde Zeilen



    [Header("Jumpscare")]
    [SerializeField] private Transform jumpscarePosition;
    [SerializeField] private float jumpscareDuration = 1.5f;
    [SerializeField] private float jumpscareMoveSpeed = 18f;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 hitImpulseForce = new Vector3(1f, 1f, 0f);

    [Header("Jumpscare Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField][Range(0f, 1f)] private float jumpscareVolume = 1f;

    [Header("Chase Scene")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip chaseSound; // Chase Scene Track

    [Header("Player")]
    [SerializeField] private Transform respawnPoint;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.5f;

    [Header("Health")]
    [SerializeField] private int health = 1;

    [SerializeField] private string sceneToLoad = "";

    private EnemyState currentState = EnemyState.Idle;
    private Collider npcCollider;

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

        npcCollider = GetComponent<Collider>();
        if (npcCollider == null)
            npcCollider = GetComponentInChildren<Collider>();

        // Startzustand
        openDoor = false;
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
        if (impulseSource != null)
        {
                impulseSource.GenerateImpulse(hitImpulseForce);
        }

        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound, jumpscareVolume);
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

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }

        SetRunning(true);
        openDoor = true;

        if (audioSource != null && chaseSound != null)
        {
            musicAudioSource.clip = chaseSound;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
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

        changeScene();
        openDoor = false;
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

        animator.SetBool("IsDead", true);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        if (npcCollider != null)
            npcCollider.enabled = false;

        Debug.Log("Enemy ist tot!");
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

    public void stopTrigger()
    {
        currentState = EnemyState.Idle;
        SetRunning(false);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
    }

    private void changeScene()
    {
        SceneFader sceneFader = FindFirstObjectByType<SceneFader>();

        if (sceneFader != null)
        {
            sceneFader.FadeAndLoad(sceneToLoad, 0f);
        }
        else
        {
            Debug.LogError("Kein SceneFader in der Szene gefunden!");
        }
    }
}
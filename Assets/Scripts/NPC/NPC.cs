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

    public enum DialogueSpeaker
    {
        NPC,
        AI
    }

    [System.Serializable]
    public class DialogueLine
    {
        public DialogueSpeaker speaker;

        [TextArea(2, 5)]
        public string text;
    }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    public bool openDoor;

    // =========================================================
    // JUMPSCARE
    // =========================================================

    [Header("Jumpscare")]
    [SerializeField] private Transform jumpscarePosition;
    [SerializeField] private float jumpscareDuration = 1.5f;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 hitImpulseForce = new Vector3(1f, 1f, 0f);

    [Header("Jumpscare Sound")]
    [SerializeField] private AudioClip jumpscareSound;
    [SerializeField][Range(0f, 1f)] private float jumpscareVolume = 1f;

    // =========================================================
    // DIALOGUE
    // =========================================================

    [Header("Dialogue Sequence")]
    [SerializeField] private DialogueLine[] dialogueSequence;

    [SerializeField] private float npcTypingSpeed = 0.05f;
    [SerializeField] private float npcDisplayDuration = 3f;
    [SerializeField] private float delayBetweenLines = 0.5f;

    // =========================================================
    // NPC MUMBLING
    // =========================================================

    [Header("NPC Mumbling")]
    [SerializeField] private AudioClip npcMumblingSound;
    [SerializeField][Range(0f, 1f)] private float npcMumblingVolume = 0.4f;

    private AudioSource npcMumblingSource;


    [Header("Chase Music")]
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField][Range(0f, 1f)] private float chaseMusicVolume = 0.7f;

    private AudioSource chaseMusicSource;

    // =========================================================
    // AI
    // =========================================================

    [Header("AI Dialogue")]
    [SerializeField] private VoiceTrigger aiVoiceTrigger;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    [SerializeField] private Transform respawnPoint;

    [Header("Movement Lock")]
    [SerializeField] private bool lockPlayerDuringDialogue = true;

    // =========================================================
    // CHASE
    // =========================================================

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.5f;

    // =========================================================
    // HEALTH
    // =========================================================

    [Header("Health")]
    [SerializeField] private int health = 1;

    [SerializeField] private string sceneToLoad = "";

    // =========================================================
    // PRIVATE
    // =========================================================

    private EnemyState currentState = EnemyState.Idle;
    private Collider npcCollider;
    private bool sequenceStarted = false;


    // =========================================================
    // AWAKE
    // =========================================================

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

        openDoor = false;

        SetRunning(false);

        // NPC Mumbling AudioSource
        npcMumblingSource = gameObject.AddComponent<AudioSource>();

        npcMumblingSource.playOnAwake = false;
        npcMumblingSource.loop = true;
        npcMumblingSource.spatialBlend = 0f;
        npcMumblingSource.volume = npcMumblingVolume;

        if (npcMumblingSound != null)
        {
            npcMumblingSource.clip = npcMumblingSound;
        }

        chaseMusicSource = gameObject.AddComponent<AudioSource>();

        chaseMusicSource.playOnAwake = false;
        chaseMusicSource.loop = true;
        chaseMusicSource.spatialBlend = 0f;
        chaseMusicSource.volume = chaseMusicVolume;
        chaseMusicSource.clip = chaseMusic;
    }


    // =========================================================
    // UPDATE
    // =========================================================

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
    // START SEQUENCE
    // =========================================================

    public void StartSequence()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;

        StartCoroutine(JumpscareSequence());
    }


    // =========================================================
    // JUMPSCARE SEQUENCE
    // =========================================================

    private System.Collections.IEnumerator JumpscareSequence()
    {
        currentState = EnemyState.Jumpscare;

        SetRunning(false);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // Jumpscare Sound
        if (jumpscareSound != null)
        {
            AudioSource.PlayClipAtPoint(
                jumpscareSound,
                jumpscarePosition != null
                    ? jumpscarePosition.position
                    : transform.position,
                jumpscareVolume
            );
        }

        // NPC zur Jumpscare-Position
        if (jumpscarePosition != null)
        {
            Vector3 startPosition = transform.position;

            float elapsedTime = 0f;
            float moveDuration = 0.2f;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;

                transform.position = Vector3.Lerp(
                    startPosition,
                    jumpscarePosition.position,
                    t
                );

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.position = jumpscarePosition.position;
            transform.rotation = jumpscarePosition.rotation;

            if (agent != null)
                agent.Warp(jumpscarePosition.position);
        }

        // Camera Shake
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(hitImpulseForce);
        }

        Debug.Log("JUMPSCARE!");

        yield return new WaitForSeconds(jumpscareDuration);

        // =====================================================
        // TALKING START
        // =====================================================

        currentState = EnemyState.Talking;

        SetRunning(false);

        if (agent != null)
            agent.isStopped = true;

        // Spieler kann sich während des gesamten Dialogs nicht bewegen
        if (lockPlayerDuringDialogue)
            SetPlayerMovement(false);

        // =====================================================
        // FLEXIBLE DIALOGUE SEQUENCE
        // =====================================================

        if (dialogueSequence != null)
        {
            foreach (DialogueLine dialogueLine in dialogueSequence)
            {
                if (dialogueLine == null)
                    continue;

                if (string.IsNullOrWhiteSpace(dialogueLine.text) &&
                    dialogueLine.speaker == DialogueSpeaker.NPC)
                    continue;

                // =================================================
                // NPC SPRICHT
                // =================================================

                if (dialogueLine.speaker == DialogueSpeaker.NPC)
                {
                    StartNpcMumbling();

                    if (VoiceManager.Instance != null)
                    {
                        VoiceManager.Instance.ShowVoice(
                            dialogueLine.text,
                            npcTypingSpeed,
                            npcDisplayDuration
                        );

                        yield return new WaitUntil(
                            () => !VoiceManager.Instance.IsVoiceActive
                        );
                    }

                    StopNpcMumbling();
                }

                // =================================================
                // AI SPRICHT
                // =================================================

                else if (dialogueLine.speaker == DialogueSpeaker.AI)
                {
                    if (aiVoiceTrigger != null)
                    {
                        aiVoiceTrigger.TriggerVoice();

                        if (VoiceManager.Instance != null)
                        {
                            yield return new WaitUntil(
                                () => !VoiceManager.Instance.IsVoiceActive
                            );
                        }
                    }
                }

                // Pause zwischen den Dialogzeilen
                if (delayBetweenLines > 0f)
                {
                    yield return new WaitForSeconds(
                        delayBetweenLines
                    );
                }
            }
        }

        StopNpcMumbling();

        // =====================================================
        // MOVEMENT FREIGEBEN
        // =====================================================

        if (lockPlayerDuringDialogue)
            SetPlayerMovement(true);

        // =====================================================
        // CHASE
        // =====================================================

        StartChase();
    }


    // =========================================================
    // NPC MUMBLING
    // =========================================================

    private void StartNpcMumbling()
    {
        if (npcMumblingSource == null)
            return;

        if (npcMumblingSound == null)
            return;

        npcMumblingSource.volume = npcMumblingVolume;

        if (!npcMumblingSource.isPlaying)
            npcMumblingSource.Play();
    }


    private void StopNpcMumbling()
    {
        if (npcMumblingSource == null)
            return;

        if (npcMumblingSource.isPlaying)
            npcMumblingSource.Stop();
    }


    // =========================================================
    // PLAYER MOVEMENT
    // =========================================================

    private void SetPlayerMovement(bool enabled)
    {
        if (player == null)
            return;

        Player playerScript = player.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.SetMovementEnabled(enabled);
        }
    }


    // =========================================================
    // CHASE
    // =========================================================

    public void StartChase()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Chasing;

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }

        SetRunning(true);

        openDoor = true;

        // =========================================
        // CHASE MUSIC
        // =========================================

        if (chaseMusic != null)
        {
            if (chaseMusicSource != null && chaseMusic != null)
            {
                chaseMusicSource.Play();
            }
            
        }

        Debug.Log("CHASE START!");
    }

    public bool IsChasing()
    {
        return currentState == EnemyState.Chasing;
    }


    private void ChasePlayer()
    {
        if (player == null)
            return;

        if (agent == null || !agent.enabled)
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

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

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
            Die();
    }


    // =========================================================
    // DEATH
    // =========================================================

    private void Die()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Dead;

        StopNpcMumbling();

        SetPlayerMovement(true);

        if (animator != null)
            animator.SetBool("IsDead", true);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = false;

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


    // =========================================================
    // STOP TRIGGER
    // =========================================================

    public void stopTrigger()
    {
        currentState = EnemyState.Idle;

        StopNpcMumbling();

        SetRunning(false);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
    }


    // =========================================================
    // SCENE CHANGE
    // =========================================================

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
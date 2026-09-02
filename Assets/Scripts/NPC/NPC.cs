using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField][Range(0f, 2f)] private float jumpscareVolume = 1f;

    [Header("Chase Scene")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip chaseTrack; // Chase Scene Track

    [Header("NPC getting Shot")]
    [SerializeField] private AudioSource shotgunAudioSource;
    [SerializeField] private AudioClip shotgunSound;
    [SerializeField][Range(0f, 1f)] private float shotgunVolume = 1f;
    


    [Header("Player")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private MonoBehaviour playerMovement;
    [SerializeField] private float playerFreezeDuration;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.5f;

    [Header("Chase Dialogue")]
    [SerializeField] private string[] chaseDialogueLines;
    [SerializeField] private float[] chaseDialogueDelays;

    [Header("After NPC's Death Dialogue")]
    [SerializeField] private string[] afterDialogueLines;
    [SerializeField] private float[] afterDialogueDelay;

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

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
            

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

        StartTalking();

        yield return new WaitForSeconds(playerFreezeDuration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        
            
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

        StartCoroutine(TalkingSequence()); // GEÄNDERT: statt Invoke
    }

    private System.Collections.IEnumerator TalkingSequence()
    {
        foreach (DialogueLine line in talkingDialogue)
        {
            if (string.IsNullOrWhiteSpace(line.text))
                continue;

            if (line.isAI)
            {
                VoiceManager.Instance.ShowVoice(line.text);
                yield return new WaitUntil(() => !VoiceManager.Instance.IsVoiceActive);
            }
            else
            {
                npcVoiceManager.ShowDialogue(new string[] { line.text });
                yield return new WaitUntil(() => !npcVoiceManager.IsVoiceActive);
            }
        }

        StartChase();
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

        if (audioSource != null && chaseTrack != null)
        {
            musicAudioSource.clip = chaseTrack;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        StartCoroutine(ChaseDialogueSequence());

        Debug.Log("CHASE START!");


        
    }

    private IEnumerator ChaseDialogueSequence()
    {
        for (int i = 0; i < chaseDialogueDelays.Length; i++)
        {
            float delay = (i < chaseDialogueDelays.Length) ? chaseDialogueDelays[i] : 3f;
            yield return new WaitForSeconds(delay);

            if (currentState != EnemyState.Chasing)
                yield break;

            VoiceManager.Instance.ShowVoice(chaseDialogueLines[i]);
        }
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

        if (shotgunAudioSource != null && shotgunSound != null)
        {
            shotgunAudioSource.PlayOneShot(shotgunSound, shotgunVolume);
        }

        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }

        animator.SetBool("IsDead", true);

        StartCoroutine(AfterDeathDialogue());

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

    private IEnumerator AfterDeathDialogue()
    {
        for (int i = 0; i < afterDialogueLines.Length; i++)
        {
            float delay = (i < afterDialogueDelay.Length) ? afterDialogueDelay[i] : 3f;
            yield return new WaitForSeconds(delay);

            if (currentState != EnemyState.Dead)
                yield break;

            VoiceManager.Instance.ShowVoice(afterDialogueLines[i]);
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
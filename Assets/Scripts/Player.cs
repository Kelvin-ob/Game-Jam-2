using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CharacterController controller;
    [SerializeField] private FootSteps footSteps;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform playerBody;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float normalHeight = 1.9f;

    [Header("Steps")]
    [Tooltip("Grundintervall zwischen Schritten (Sekunden) beim normalen Gehen")]
    [SerializeField] private float stepInterval = 0.5f;
    [Tooltip("Wie viel schneller die Schritte beim Sprinten sind (kleiner = öfter)")]
    [SerializeField] private float sprintStepMultiplier = 0.65f;
    [SerializeField] private float crouchStepMultiplier = 0.95f;

    Vector3 velocity;
    bool isGrounded;
    private float stepTimer;
    private bool movementEnabled = true;

    public bool IsSprinting => gameInput.getIsSprinting();
    public bool IsCrouching => gameInput.getIsCrouching();


    void Start()
    {
        stepTimer = 0f;
    }

    void Update()
    {
        if (!movementEnabled)
        {
            return; 
        }

        HandleMovement();

    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 inputVector = gameInput.GetMovementNormalized();
        Vector3 moveDir = transform.right * inputVector.x + transform.forward * inputVector.y;

        bool isMoving = inputVector != Vector2.zero;
        bool isSprinting = gameInput.getIsSprinting();
        bool isCrouching = gameInput.getIsCrouching();

        float currentSpeed = isSprinting && !isCrouching ? sprintSpeed : moveSpeed;
        if (isMoving)
        {
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (cam != null)
        {
            var perlin = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                if (isSprinting && isMoving && !isCrouching)
                {
                    perlin.AmplitudeGain = 1f;
                    perlin.FrequencyGain = 5f;
                }
                else if (isMoving)
                {
                    perlin.AmplitudeGain = 0.5f;
                    perlin.FrequencyGain = 2.5f;
                }
                else
                {
                    perlin.AmplitudeGain = 0.4f;
                    perlin.FrequencyGain = 2f;
                }
            }
        }

        // --- Crouch / Stand mit fester Fußposition ---
        float prevBottom = controller.bounds.min.y;

        // Höhe und Center abhängig vom Zustand setzen
        float targetHeight = (isGrounded && isCrouching) ? crouchHeight : normalHeight;
        controller.height = targetHeight;
        controller.center = new Vector3(controller.center.x, targetHeight * 0.5f, controller.center.z);

        // Positionsdifferenz ausgleichen (damit Füße am Boden bleiben)
        float delta = prevBottom - controller.bounds.min.y;
        if (Mathf.Abs(delta) > 0.0001f)
            transform.position += new Vector3(0f, delta, 0f);

        // --- Safety: nie unter y = 0 rutschen ---
        if (transform.position.y < 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            velocity.y = Mathf.Max(velocity.y, 0f);
        }

        if (isGrounded && isMoving)
        {
            if (isMoving) // If player is walking
            {
                stepTimer += Time.deltaTime;
                float interval = stepInterval;

                if (isSprinting && !isCrouching)
                {
                    if (stepTimer >= sprintStepMultiplier)
                    {
                        footSteps.FootStep();

                        interval *= sprintStepMultiplier;
                        stepTimer = interval;
                    }
                }
                else if (!isSprinting && isCrouching)
                {
                    interval *= crouchStepMultiplier;
                    stepTimer = interval;
                }
                else
                {
                    if (stepTimer >= sprintStepMultiplier)
                    {
                        footSteps.FootStep();

                        stepTimer = 0f;
                    }
                }
            }
        }

    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }
}

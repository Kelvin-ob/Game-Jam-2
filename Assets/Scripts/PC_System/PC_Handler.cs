using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PC_Handler : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineCamera pc_cam;
    [SerializeField] private CinemachineCamera fps_cam;

    [SerializeField] private Player player;
    [SerializeField] private string promptText = "use";
    [SerializeField] private GameInput gameInput;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private GameObject crosshair;

    [Header("Camera")]
    [SerializeField] private float cameraWaitTime = 0.5f;

    [SerializeField] private bool onComputer;

    private void Start()
    {
        onComputer = false;
    }

    private void Update()
    {
        if (onComputer && gameInput.getIsEscaped())
        {
            ExitComputer();
        }
    }

    public void Interact()
    {
        onComputer = true;

        // Kamera zum PC wechseln
        CameraManager.SwitchCamera(pc_cam);

        // Spieler deaktivieren
        player.SetMovementEnabled(false);
        mouseLook.SetCanLook(false);

        // Maus aktivieren
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        // Andere UI ausblenden
        InteractPromptManager.Instance.hidePrompt();
        crosshair.SetActive(false);
    }


    public void OnFocus()
    {
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }

    private void ExitComputer()
    {
        // Eventuelle Coroutine stoppen
        StopAllCoroutines();
        // Zurück zur FPS-Kamera
        CameraManager.SwitchCamera(fps_cam);

        // Spieler wieder aktivieren
        player.SetMovementEnabled(true);
        mouseLook.SetCanLook(true);

        // Crosshair wieder anzeigen
        crosshair.SetActive(true);

        // Maus wieder sperren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Interaktions-Prompt wieder anzeigen
        InteractPromptManager.Instance.showPrompt(promptText);

    }
}
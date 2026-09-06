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

    [Header("Computer UI")]
    [SerializeField] private GameObject computerUI;
    [SerializeField] private PCUIFade pcUIFade;

    [Header("Camera")]
    [SerializeField] private float cameraWaitTime = 0.5f;

    [SerializeField] private bool onComputer;
    private bool isTransitioning;

    private void Start()
    {
        onComputer = false;
        isTransitioning = false;
        computerUI.SetActive(false);
    }

    private void Update()
    {
        if (onComputer && !isTransitioning && gameInput.getIsEscaped())
        {
            ExitComputer();
        }
    }

    public void Interact()
    {
        if (onComputer || isTransitioning)
            return;

        onComputer = true;
        isTransitioning = true;

        // Kamera zum PC wechseln
        CameraManager.SwitchCamera(pc_cam);

        // Spieler deaktivieren
        player.SetMovementEnabled(false);
        mouseLook.SetCanLook(false);

        // Maus anzeigen
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        // Andere UI ausblenden
        InteractPromptManager.Instance.hidePrompt();
        crosshair.SetActive(false);

        // Computer UI erst nach dem Kamera-Zoom öffnen
        StartCoroutine(OpenComputer());
    }

    private IEnumerator OpenComputer()
    {
        // Warten, bis die Kamera am PC angekommen ist
        yield return new WaitForSeconds(cameraWaitTime);

        // UI aktivieren
        computerUI.SetActive(true);
        pcUIFade.SetInteractionEnabled(false);

        // UI langsam einblenden
        pcUIFade.ShowUI();
        yield return new WaitForSeconds(pcUIFade.FadeDuration);
        pcUIFade.SetInteractionEnabled(true);
        isTransitioning = false;
    }

    public void OnFocus()
    {
        if (!onComputer)
        {
            InteractPromptManager.Instance.showPrompt(promptText);
        }
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }

    private void ExitComputer()
    {
        if (!onComputer || isTransitioning)
            return;

        StopAllCoroutines();

        StartCoroutine(CloseComputer());
    }

    private IEnumerator CloseComputer()
    {
        isTransitioning = true;
        pcUIFade.SetInteractionEnabled(false);

        // UI langsam ausblenden
        pcUIFade.HideUI();

        // Warten, bis Fade-Out fertig ist
        yield return new WaitForSeconds(pcUIFade.FadeDuration);

        // UI deaktivieren
        computerUI.SetActive(false);

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

        onComputer = false;
        isTransitioning = false;
    }
}
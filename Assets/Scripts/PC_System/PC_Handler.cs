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





    private void Update()
    {
        if (gameInput.getIsEscaped())
        {
            ExitComputer();
        }
    }


    public void Interact()
    {
        CameraManager.SwitchCamera(pc_cam);
        player.SetMovementEnabled(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        mouseLook.SetCanLook(false);
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
        CameraManager.SwitchCamera(fps_cam);
        player.SetMovementEnabled(true);
        mouseLook.SetCanLook(true);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InteractPromptManager.Instance.showPrompt(promptText);
    }
}

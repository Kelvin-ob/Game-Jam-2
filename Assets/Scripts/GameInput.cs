using UnityEngine;

public class GameInput : MonoBehaviour
{

    private InputSystem_Actions inputActions;
    private bool isSprinting;
    private bool isCrouching;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        if (inputActions != null)
            inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        if (inputActions != null)
            inputActions.Dispose();
    }

    public Vector2 GetMovementNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        return inputVector;
    }

    public bool getIsSprinting()
    {
        isSprinting = inputActions.Player.Sprint.IsPressed();
        return isSprinting;
    }

    public bool getIsCrouching()
    {
        isCrouching = inputActions.Player.Crouch.IsPressed();
        return isCrouching;
    }

    public Vector2 GetMousePos()
    {
        Vector2 mouseVector = inputActions.Player.Look.ReadValue<Vector2>();
        return mouseVector;
    }

}

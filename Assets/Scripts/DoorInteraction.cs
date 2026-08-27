using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Interaction")]
    [SerializeField] private string promptText = "open";

    private bool isOpen = false;
    private bool isMoving = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private Coroutine currentCoroutine;

    private void Start()
    {
        closedRotation = transform.rotation;

        openRotation = Quaternion.Euler(
            transform.eulerAngles + new Vector3(0, openAngle, 0)
        );
    }

    public void Interact()
    {
        // Während die Tür sich bewegt, nichts machen
        if (isMoving)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ToggleDoor());
        if (isOpen)
        {
            promptText = "close";
        }
        else
        {
            promptText = "open";
        }
    }

    public void OnFocus()
    {
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }

    private IEnumerator ToggleDoor()
    {
        isMoving = true;

        Quaternion targetRotation = isOpen
            ? closedRotation
            : openRotation;

        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        transform.rotation = targetRotation;

        isMoving = false;
        currentCoroutine = null;
    }
}
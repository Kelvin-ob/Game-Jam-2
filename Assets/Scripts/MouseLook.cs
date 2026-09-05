using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSens = 10f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private bool canLook;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        canLook = true;
    }

    void Update()
    {
        if (!canLook)
        {
            return;
        }
        Vector2 mouseVector = gameInput.GetMousePos();
        float mouseX = mouseVector.x * mouseSens * Time.deltaTime;
        float mouseY = mouseVector.y * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Math.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        bool isCrouching = gameInput.getIsCrouching();
        float targetY = isCrouching ? -0.5f : 0f;

        // aktuelle Position holen
        Vector3 pos = transform.localPosition;

        // Y-Wert sanft angleichen
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 8f);

        // neue Position setzen
        transform.localPosition = pos;
    }

    public void SetCanLook(bool newcanLook)
    {
        canLook = newcanLook;
    }

}

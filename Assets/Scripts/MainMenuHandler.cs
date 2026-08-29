using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider clickableCollider;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private bool quitGame = false;

    [Header("Text Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.red;

    private TMP_Text textComponent;
    private bool isHovering = false;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        ApplyColor(normalColor);
    }

    private void Update()
    {
        if (mainCamera == null || textComponent == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        bool hitSomething = false;
        RaycastHit hit;

        if (clickableCollider != null)
        {
            hitSomething = Physics.Raycast(ray, out hit, 1000f);
            hitSomething = hitSomething && hit.collider == clickableCollider;
        }
        else
        {
            hitSomething = Physics.Raycast(ray, out hit, 1000f, interactableLayer.value);
        }

        isHovering = hitSomething;
        ApplyColor(isHovering ? hoverColor : normalColor);

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TriggerAction();
        }
    }

    private void ApplyColor(Color color)
    {
        if (textComponent != null)
        {
            textComponent.color = color;
        }
    }

    private void TriggerAction()
    {
        if (quitGame)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}

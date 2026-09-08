using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;

    [SerializeField] private CanvasGroup noteCanvasGroup;
    [SerializeField] private RectTransform noteRect; // für die Zoom-Animation
    [SerializeField] private TMP_Text noteText;

    [SerializeField] private float zoomDuration = 0.25f;
    [SerializeField] private Vector3 startScale = new Vector3(0.7f, 0.7f, 0.7f);

    public bool IsNoteOpen { get; private set; }
    public float CloseCooldownUntil {  get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        noteCanvasGroup.alpha = 0f;
        noteCanvasGroup.interactable = false;
        noteCanvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (!IsNoteOpen)
            return;

        
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
        {
            CloseNote();
        }
    }

    public void ShowNote(string text)
    {
        noteText.text = text;
        IsNoteOpen = true;
        StartCoroutine(ZoomIn());
    }

    private void CloseNote()
    {
        IsNoteOpen = false;
        CloseCooldownUntil = Time.time + 0.2f;
        StartCoroutine(ZoomOut());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator ZoomIn()
    {
        noteCanvasGroup.interactable = true;
        noteCanvasGroup.blocksRaycasts = true;

        float time = 0f;
        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = time / zoomDuration;

            noteCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            noteRect.localScale = Vector3.Lerp(startScale, Vector3.one, t);

            yield return null;
        }

        noteCanvasGroup.alpha = 1f;
        noteRect.localScale = Vector3.one;
    }

    private IEnumerator ZoomOut()
    {
        float time = 0f;
        Vector3 currentScale = noteRect.localScale;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            float t = time / zoomDuration;

            noteCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            noteRect.localScale = Vector3.Lerp(currentScale, startScale, t);

            yield return null;
        }

        noteCanvasGroup.alpha = 0f;
        noteCanvasGroup.interactable = false;
        noteCanvasGroup.blocksRaycasts = false;
    }
}
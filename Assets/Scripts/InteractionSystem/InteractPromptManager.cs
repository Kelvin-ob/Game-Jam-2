using TMPro;
using UnityEngine;

public class InteractPromptManager : MonoBehaviour
{
    public static InteractPromptManager Instance { get; private set; }

    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        promptRoot.SetActive(false);
    }

    public void showPrompt(string text)
    {
        promptText.text = text;
        promptRoot.SetActive(true);
    }

    public void hidePrompt()
    {
        promptRoot.SetActive(false);
    }
}
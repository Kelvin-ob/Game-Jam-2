using UnityEngine;

public class Interactables : MonoBehaviour, IInteractable
{

    [TextArea(2, 5)]
    [SerializeField] private string promptText = "custom text"; //customisable inside unity

    private MeshRenderer meshRenderer;
   

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Interact()
    {
        Debug.Log("You pressed me!");
        meshRenderer.material.color = Random.ColorHSV();
    }


    public void OnFocus()
    {
        InteractPromptManager.Instance.showPrompt(promptText);
    }

    public void OnLoseFocus()
    {
        InteractPromptManager.Instance.hidePrompt();
    }
}

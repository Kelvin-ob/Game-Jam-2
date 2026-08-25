using UnityEngine;

public class Interactables : MonoBehaviour, IInteractable
{
    private MeshRenderer meshRenderer;
    [SerializeField] private GameObject interactPrompt; // later texts like: "press E to interact"

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        interactPrompt.SetActive(false); // anfang unsichtbar
    }

    public void Interact()
    {
        Debug.Log("You pressed me!");
        meshRenderer.material.color = Random.ColorHSV();
    }


    public void OnFocus()
    {
        interactPrompt.SetActive(true);
    }

    public void OnLoseFocus()
    {
        interactPrompt.SetActive(false);
    }
}

using UnityEngine;

public class DoorTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Name der nächsten Szene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger enter:" + other.name);
            SceneTransitionManager.Instance.LoadScene(sceneToLoad);
        }
    }
}
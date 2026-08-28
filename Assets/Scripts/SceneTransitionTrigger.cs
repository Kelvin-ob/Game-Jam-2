using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string sceneName;
    public float fadeDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<SceneFader>().FadeAndLoad(sceneName, fadeDuration);
        }
    }
}
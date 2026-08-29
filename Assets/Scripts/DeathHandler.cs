using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private float waitBeforeLoad = 2f;

    private void Start()
    {
        if (animator != null)
            animator.SetBool("IsJumpScare", true);

        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeLoad);

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Keine Szene für DeathHandler gesetzt.");
            yield break;
        }

        SceneFader sceneFader = FindFirstObjectByType<SceneFader>();

        if (sceneFader != null)
        {
            sceneFader.FadeAndLoad(sceneToLoad, 0.5f);
            yield break;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}

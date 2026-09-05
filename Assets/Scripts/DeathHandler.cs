using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private float waitBeforeLoad = 2f;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 hitImpulseForce = new Vector3(1f, 1f, 0f);
    [SerializeField] private AudioClip dyingSound;
    [SerializeField] private AudioSource audioSource;
    private void Start()
    {
        Debug.Log("DEATH HANDLER START!");

        if (animator != null)
            animator.SetBool("IsJumpScare", true);

        if (impulseSource != null)
            impulseSource.GenerateImpulse(hitImpulseForce);

        if (audioSource != null && dyingSound != null)
        {
            Debug.Log("DYING SOUND WIRD ABGESPIELT!");
            audioSource.PlayOneShot(dyingSound);
        }

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

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ClearItemCollected("gun");
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem("gun");
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

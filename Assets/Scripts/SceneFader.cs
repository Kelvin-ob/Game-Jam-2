using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Player player;
    public Image image;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    public void FadeAndLoad(string sceneName, float duration)
    {
        player.enabled = false;  
        StartCoroutine(Fader(sceneName, duration));

    }

    IEnumerator Fader(string sceneName, float duration)
    {
        float t = 0;
        Color c = image.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            image.color = c;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color c = image.color;

        while (t < 1)
        {
            t += Time.deltaTime;
            c.a = 1f - t;
            image.color = c;
            yield return null;
        }
    }
}
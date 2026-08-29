using UnityEngine;
using System.Collections;

public class WakeUpCamera : MonoBehaviour
{
    [SerializeField] private float startAngle = 70f;
    [SerializeField] private float wakeUpDuration = 2f;

    private Quaternion normalRotation;

    private void Start()
    {
        normalRotation = transform.localRotation;

        // Startposition: Kamera schaut nach unten
        transform.localRotation = Quaternion.Euler(startAngle, 0f, 0f);

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        float time = 0f;

        Quaternion startRotation = transform.localRotation;

        while (time < wakeUpDuration)
        {
            time += Time.deltaTime;

            float t = time / wakeUpDuration;

            // Smoothes Aufrichten
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localRotation = Quaternion.Slerp(
                startRotation,
                normalRotation,
                t
            );

            yield return null;
        }

        transform.localRotation = normalRotation;
    }
}
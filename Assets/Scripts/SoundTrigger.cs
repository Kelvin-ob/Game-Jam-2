using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private bool triggerOnce = true;

    [Header("Sound")]
    [SerializeField] private AudioClip sound;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    [Header("Audio Settings")]
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playIn3D = true;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && triggered)
            return;

        triggered = true;

        PlaySound();
    }

    private void PlaySound()
    {
        if (sound == null)
            return;

        GameObject soundObject = new GameObject("TriggeredSound");

        soundObject.transform.position = transform.position;

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = sound;
        audioSource.volume = volume;
        audioSource.loop = loop;

        if (playIn3D)
        {
            audioSource.spatialBlend = 1f;
        }
        else
        {
            audioSource.spatialBlend = 0f;
        }

        audioSource.Play();

        if (!loop)
        {
            Destroy(soundObject, sound.length);
        }
    }
}
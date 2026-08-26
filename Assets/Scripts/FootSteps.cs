using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceType
{
    public string surfaceTag;                  // Tag of the surface (e.g., "Wood", "Metal", "Grass")
    public AudioClip[] footstepSounds;         // Array of footstep sounds for this surface
}

public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public List<SurfaceType> surfaceTypes;      // List of surface types and their corresponding sounds
    public float stepIntervalWalk = 0.4f;       // Time interval between steps when walking (aktuell von dir nicht genutzt)
    public float stepIntervalSprint = 0.7f;     // Time interval between steps when sprinting (aktuell nicht genutzt)

    private CharacterController controller;     // Reference to the CharacterController component
    private float stepTimer = 0f;               // Timer to track time between steps
    private int lastPlayedIndex = -1;           // Index of the last played footstep sound
    public static bool walking;

    [Header("Raycast")]
    [SerializeField] private Transform RayStart;
    [SerializeField] private float range = 1f;
    [SerializeField] private LayerMask layerMask;


    void Start()
    {
        controller = GetComponent<CharacterController>();

        // If no AudioSource is assigned, use the one attached to the same GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void FootStep()
    {
        AudioClip[] selectedFootstepSounds = GetFootstepSoundsForCurrentSurface();

        if (selectedFootstepSounds != null && selectedFootstepSounds.Length > 0)
        {
            int newIndex;

            // Ensure the new sound is different from the last one played
            do
            {
                newIndex = Random.Range(0, selectedFootstepSounds.Length);
            }
            while (newIndex == lastPlayedIndex);

            // Play the selected footstep sound
            audioSource.PlayOneShot(selectedFootstepSounds[newIndex]);

            // Store the index of the played sound
            lastPlayedIndex = newIndex;
        }

        // Lokale Funktion für die Oberflächen-Sounds
        AudioClip[] GetFootstepSoundsForCurrentSurface()
        {
            RaycastHit hit;

            // Cast a ray downward from the character to detect the surface
            if (Physics.Raycast(RayStart.position, Vector3.down, out hit, range, layerMask))
            {
                string surfaceTag = hit.collider.tag;

                // Find the SurfaceType that matches the tag
                foreach (SurfaceType surfaceType in surfaceTypes)
                {
                    if (surfaceType.surfaceTag == surfaceTag)
                    {
                        return surfaceType.footstepSounds;
                    }
                }
            }

            // Return null if no matching surface type is found
            return null;
        }
    }
}

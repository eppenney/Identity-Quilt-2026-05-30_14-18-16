using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FootstepPlayer : MonoBehaviour
{
    [Header("Footstep Pacing")]
    [Tooltip("How many seconds to wait between footsteps while moving.")]
    [SerializeField] private float stepInterval = 0.5f;

    [Header("Audio Component")]
    private CharacterController controller;
    private AudioSource audioSource;
    private float stepTimer;

    void Awake()
    {
        // Grab the components attached to this GameObject automatically
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        // Ensure the audio source doesn't blast the sound byte the microsecond the game loads
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        // 1. Is the player actually touching the ground?
        if (!controller.isGrounded) return;

        // 2. Is the player physically moving across the floor?
        // We check if the horizontal velocity (X and Z axes) is greater than a tiny threshold
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        
        if (horizontalVelocity.magnitude > 0.1f)
        {
            // Advance our step timer over time
            stepTimer += Time.deltaTime;

            // 3. Has enough time passed to take another step?
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f; // Reset the timer loop
            }
        }
        else
        {
            // If the player stops moving, reset the timer so the next step triggers instantly when they walk
            stepTimer = stepInterval;
        }
    }

    private void PlayFootstepSound()
    {
        if (audioSource.clip == null) return;

        // Subtle Pitch Randomization (Crucial for a single sound byte!)
        // Modulating the pitch slightly up and down prevents the footstep from sounding repetitive and artificial
        audioSource.pitch = Random.Range(0.85f, 1.15f);

        // Play the sound once without interrupting any other instances
        audioSource.PlayOneShot(audioSource.clip);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class AudioETrigger : MonoBehaviour
{
    [Header("Audio Component")]
    private AudioSource audioSource;

    [Header("Timeline Events")]
    [Tooltip("Fires the moment the audio file begins playing.")]
    public UnityEvent onAudioStart;

    [Tooltip("Fires automatically the exact frame the audio file finishes playing.")]
    public UnityEvent onAudioComplete;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Call this function from your Area Triggers or Item Interactions!
    /// </summary>
    public void PlayTrackWithSequence()
    {
        // Prevent overlapping tracking loops if double-clicked
        StopAllCoroutines();
        
        StartCoroutine(AudioSequenceRoutine());
    }

    private IEnumerator AudioSequenceRoutine()
    {
        Debug.Log("Audio Trigger Started");
        // 1. Ensure we actually have a voice take or sound clip loaded
        if (audioSource.clip == null)
        {
            Debug.LogWarning($"[AudioEventTrigger] No AudioClip found on {gameObject.name} to track!", this);
            yield break;
        }

        // 2. Fire the start actions and turn on the play state
        if (onAudioStart != null) onAudioStart.Invoke();
        audioSource.Play();

        // 3. SUSPEND EXECUTION: Keep tracking this line until the audio timeline is exhausted
        // We wait for the precise length of the clip in seconds
        yield return new WaitForSeconds(audioSource.clip.length);

        // 4. Time is up! Fire the completion sequence
        if (onAudioComplete != null) onAudioComplete.Invoke();
    }
}
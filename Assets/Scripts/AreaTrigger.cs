using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Should this event only fire the first time the player crosses it?")]
    [SerializeField] private bool triggerOnlyOnce = true;
    
    [Header("Events")]
    [Tooltip("The actions or scripts that will execute the moment the player enters this zone.")]
    public UnityEvent onPlayerEnter;

    private bool hasTriggered = false;

    private void Awake()
    {
        // Enforce that the collider acts as an invisible boundary pass-through, not a physical wall
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Stop execution if it's a one-time event and has already been tripped
        if (triggerOnlyOnce && hasTriggered) return;

        // Check if the object crossing the boundary is our Player capsule
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            hasTriggered = true;
            
            // Fire all actions hooked up in the Inspector
            if (onPlayerEnter != null)
            {
                onPlayerEnter.Invoke();
            }
        }
    }
}
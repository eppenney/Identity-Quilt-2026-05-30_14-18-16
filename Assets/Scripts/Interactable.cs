using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Prompt shown to the player when looking at the object")]
    public string interactPrompt = "Press E to interact"; // Prompt shown to the player when looking at the object
    
    [Tooltip("What happens when this object is clicked?")]
    public UnityEvent onInteract; // What happens when this object is clicked?
    
    // This is called by the Player's raycast script
    public void Interact()
    {
        if (onInteract != null)
        {
            onInteract.Invoke();
        }
    }
}

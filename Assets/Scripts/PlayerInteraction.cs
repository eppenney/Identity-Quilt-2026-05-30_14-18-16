using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactRange = 3f; // Distance for interaction ray
    [SerializeField] private LayerMask interactableLayer; // Layer for interactable objects

    [Header("Input Action")]
    public InputActionReference interactAction;

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();

    // Update is called once per frame
    void Update()
    {
        // 1. Cast a ray out from the center of the viewport
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            // 2. Check if the object has our Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null)
            {
                // UI Prompt hook goes here if you want text on screen
                
                // 3. Listen for the interaction click
                if (interactAction.action.WasPressedThisFrame())
                {
                    interactable.Interact();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 1. Set the starting point to match the script's position (the eyes/camera)
        Vector3 startPosition = transform.position;
        
        // 2. Calculate the endpoint based on the direction the object is looking and the range
        Vector3 endPosition = startPosition + (transform.forward * interactRange);

        // 3. Perform a quick check to change color if we are hitting something right now
        Gizmos.color = Color.yellow; // Default color when looking at empty air

        Ray ray = new Ray(startPosition, transform.forward);
        RaycastHit hit;
        
        // If the laser is actively hitting an object on our target layer, turn it green
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            Gizmos.color = Color.green;
            
            // Draw a little wire sphere at the exact pixel point where the laser hits the surface
            Gizmos.DrawWireSphere(hit.point, 0.05f);
        }

        // 4. Draw the actual line in the editor scene window
        Gizmos.DrawLine(startPosition, endPosition);
    }
}

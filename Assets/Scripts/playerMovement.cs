using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f; // Speed of the player movement
    [SerializeField] private float jumpHeight = 1.5f; // Height of the player's jump
    [SerializeField] private float gravity = 9.81f; // Gravity applied to the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public CharacterController controller; // Reference to the CharacterController component
    private Vector3 velocity; // Velocity of the player
    private bool isGrounded; // Check if the player is grounded

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded; 
        if (isGrounded)
        {
            if (velocity.y < -2f) velocity.y = -2f; // Keep player on ground. 
        }

        // Read inputs 
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = -input.x * transform.forward + input.y * transform.right; // Calculate movement direction based on input and player orientation
        move = Vector3.ClampMagnitude(move, 1f); // Limit diagonal movement speed

        // if (move != Vector3.zero)
        // {
        //     transform.forward = move; // Set tranform forward to movement value to be used later? 
        // }

        // Jump
        if (isGrounded && jumpAction.action.WasPressedThisFrame())
        {
            velocity.y += Mathf.Sqrt(Mathf.Abs(jumpHeight * -3.0f * gravity)); // Calculate jump velocity
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime; // Apply gravity to vertical velocity

        // Move
        Vector3 finalMove = move * playerSpeed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime); // Move the player using CharacterController
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class firstPersonCamera : MonoBehaviour
{
    [Header("Camera Sensitivity")]
    [SerializeField] private float mouseSensitivityX = 0.1f;
    [SerializeField] private float mouseSensitivityY = 0.1f;
    [SerializeField] private float upperLookLimit = 80f; 
    [SerializeField] private float lowerLookLimit = -80f;

    [Header("References")]
    public Transform eyesTransform;

    public InputActionReference lookAction;

    private float verticalRotation = 0f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        LockCursor();
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) LockCursor();
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();
        
        // Horizontal rotation
        float horizontalRotation = mouseDelta.x * mouseSensitivityX;
        transform.Rotate(Vector3.up * horizontalRotation); // Rotate player horizontally

        // Vertical rotation
        float verticalLook = mouseDelta.y * mouseSensitivityY;
        verticalRotation -= verticalLook;

        verticalRotation = Mathf.Clamp(verticalRotation, lowerLookLimit, upperLookLimit); // Clamp vertical rotation
        
        eyesTransform.localRotation = Quaternion.Euler(verticalRotation, eyesTransform.localRotation.eulerAngles.y, eyesTransform.localRotation.eulerAngles.z);    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

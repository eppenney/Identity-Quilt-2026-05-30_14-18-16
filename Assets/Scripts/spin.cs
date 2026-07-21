using UnityEngine;

public class spin : MonoBehaviour
{
    [SerializeField] private float speed = 50f; // Degrees per second to rotate over the Y axis

    // Update is called once per frame
    void Update()
    {
        // Calculate the degrees to rotate this specific frame
        float rotationAmount = speed * Time.deltaTime;

        // Rotate the object smoothly around its local Y (up) axis
        transform.Rotate(Vector3.up * rotationAmount);
    }
}

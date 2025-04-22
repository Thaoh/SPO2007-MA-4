using UnityEngine;

public class SmoothRotateObject : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 100f;

    // Target rotation angle
    private float targetRotation = 0f;

    // Current rotation
    private float currentRotation = 0f;

    void Update()
    {
        // Check if the left arrow key is pressed
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            targetRotation -= 45f; // Rotate 45 degrees left
        }

        // Check if the right arrow key is pressed
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            targetRotation += 45f; // Rotate 45 degrees right
        }

        // Smoothly rotate towards the target rotation
        currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform playerBody;

    private float xRotation = 0f; // Pitch (Up-Down)
    private float yRotation = 0f; // Yaw (Left-Right)

    // Rotation Limits
    public float minXRotation = -30f;   // Look down limit
    public float maxXRotation = 30f;    // Look up limit

    public float minYRotation = -45f;   // Look left limit
    public float maxYRotation = 45f;    // Look right limit

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

       
        yRotation += mouseX;
        xRotation -= mouseY;

        // Apply clamping
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        yRotation = Mathf.Clamp(yRotation, minYRotation, maxYRotation);

        // Apply rotations
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f); // Yaw (left-right)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Pitch (up-down)
    }
}

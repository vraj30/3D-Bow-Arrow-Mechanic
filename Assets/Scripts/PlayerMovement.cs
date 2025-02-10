using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed;
    public float turnSmoothTime;
    private float turnSMoothVelocity;

    public Transform cam;
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); //-1 / 1
        float vertical = Input.GetAxisRaw("Vertical"); // -1 / 1 but up and down

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,ref turnSMoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0,Angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}

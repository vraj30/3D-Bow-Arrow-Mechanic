using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fppMovement : MonoBehaviour
{

    public  float speed;
    public float gravity = -9.81f;
    public CharacterController controller;

    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask ground;
    public float jumpHeight;
    bool isGrounded;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed* Time.deltaTime);
            
        if (x != 0 || z != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity*Time.deltaTime);

    }
}

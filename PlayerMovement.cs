using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;

    public Transform groundCheck; //position to check ground at
    public float groundDistance = 0.4f; //radius
    public LayerMask groundMask;

    private bool isGrounded;

    Vector3 velocity;

    public float jumpHeight = 3f; 
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //negative to force onto ground
        } 

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //some physics equation for velocity needed to jump a height
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}

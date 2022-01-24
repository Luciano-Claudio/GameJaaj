using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public Rigidbody2D rb;
    //public CinemachineVirtualCamera CamStart;



    public float runSpeed = 20f;
    float horizontalMove = 0f;
    bool jump = false, crouch = false;
    private Animator animator;

    void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        //animator.SetFloat("yVelocity", rb.velocity.y);
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W))
            jump = true;

        if (Input.GetButtonDown("Crouch") || Input.GetKeyDown(KeyCode.LeftControl))
        {
            //animator.SetBool("IsCrouching", true);
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch") || Input.GetKeyUp(KeyCode.LeftControl))
        {
            //animator.SetBool("IsCrouching", false);
            crouch = false;
        }
    }

    void FixedUpdate()
    {
        //Mover o personagem
        controller.Move(horizontalMove * Time.deltaTime, crouch, jump);
        jump = false;
    }
}

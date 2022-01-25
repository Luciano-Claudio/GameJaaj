using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public Rigidbody2D rb;

    [SerializeField] private float _dashBufferLength = .1f;
    private float _dashBufferCounter;
    //public CinemachineVirtualCamera CamStart;



    public float runSpeed = 20f;
    float horizontalMove = 0f;
    bool jump = false, crouch = false, dash = false;
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
        if (Input.GetButtonDown("Jump"))
            jump = true;

        if (Input.GetButtonDown("Crouch"))
        {
            //animator.SetBool("IsCrouching", true);
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            //animator.SetBool("IsCrouching", false);
            crouch = false;
        }
        if (Input.GetButtonDown("Dash"))
        {
            _dashBufferCounter = _dashBufferLength;
            dash = true;
        }
        else _dashBufferCounter -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        //Mover o personagem
        //controller.Move(horizontalMove * Time.deltaTime, crouch, jump, dash);
        jump = dash = false;
    }
}

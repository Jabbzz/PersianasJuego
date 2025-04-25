using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Import the InputSystem namespace
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))] // Require a Rigidbody2D component on the GameObject
public class Playercontroller : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections; 
    
    public float walkSpeed = 5f;
    public float jumpImpulse;
    public float runSpeed = 10f; // Variable to store the speed of the player when running
    [SerializeField] 
    private bool _isMoving = false; // Variable to track if the player is moving
    public bool IsMoving { get
    {
        return _isMoving; // Return the value of isMoving
    } private set
    {
        _isMoving = value; // Set the value of isMoving
        animator.SetBool(AnimationStrings.isMoving, value); // Set the animator parameter "isMoving" based on the value
    }
    }
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning { get
    {
        return _isRunning; // Return the value of isRunning
    } private set
    {
        _isRunning = value; // Set the value of isRunning
        animator.SetBool(AnimationStrings.isRunning, value); // Set the animator parameter "isRunning" based on the value
    }
    }

    public float CurrentMoveSpeed { get
        {
            if (IsMoving)
            {
                if(IsRunning)
                {
                    return runSpeed; 
                }
                else
                {
                    return walkSpeed; 
                }
            }
            else
            {
                return 0f; // Return 0 if not moving
            }
        } }

    public bool _isFacingRight = true; 
    public bool isFacingRight { get {return _isFacingRight;} private set 
    {
        if (_isFacingRight != value) 
        {
            //flip the player sprite
            transform.localScale *= new Vector2(-1,1);
        }
        _isFacingRight = value; 
    } }

    Vector2 moveInput; // Variable to store the movement input
    Animator animator; // Variable to store the Animator component

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        touchingDirections = GetComponent<TouchingDirections>(); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y); // Set the horizontal velocity based on input and walk speed   

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y); //makes the character fall/rise

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        //Facing right
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true; 
        }
        //facing left
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false; 
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true; 
        }
        else if (context.canceled)
        {
            IsRunning = false; 
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //TODO check if alive as well
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        } 
}
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Import the InputSystem namespace
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable)),] // Require a Rigidbody2D component on the GameObject
public class Playercontroller : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;

    public float walkSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;

    private LedgeDetector ledgeDetector;
    public bool isHanging = false;
    public Vector2 ledgeHangOffset = new Vector2(0f, -0.5f); // Adjust for pixel-perfect hang
    [SerializeField] private Transform ledgeDetectorTransform;
    [SerializeField] private Vector2 ledgeDetectorOffset = new Vector2(0.5f, 0.5f);


    Damageable damageable;
    public float runSpeed = 10f; // Variable to store the speed of the player when running
    [SerializeField]
    private bool _isMoving = false; // Variable to track if the player is moving
    public bool IsMoving
    {
        get
        {
            return _isMoving; // Return the value of isMoving
        }
        private set
        {
            _isMoving = value; // Set the value of isMoving
            animator.SetBool(AnimationStrings.isMoving, value); // Set the animator parameter "isMoving" based on the value
        }
    }
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning; // Return the value of isRunning
        }
        private set
        {
            _isRunning = value; // Set the value of isRunning
            animator.SetBool(AnimationStrings.isRunning, value); // Set the animator parameter "isRunning" based on the value
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        //ground state checks
                        if (IsRunning)
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
                        return airWalkSpeed;
                    }

                }
                else
                {
                    return 0f; // Return 0 if not moving
                }

            }
            else
            {
                //movement Locked
                return 0f;
            }
        }
    }

    public bool _isFacingRight = true;
    public bool isFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                //flip the player sprite
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }


    Vector2 moveInput; // Variable to store the movement input
    Animator animator; // Variable to store the Animator component

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        ledgeDetector = GetComponentInChildren<LedgeDetector>();
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
        if (!damageable.LockVelocity)
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y); //makes the character fall/rise


        if (!isHanging && !touchingDirections.IsGrounded && ledgeDetector.ledgeAvailable)
        {
            EnterLedgeHang();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        //Facing right
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            UpdateLedgeDetectorPosition();
        }
        //facing left
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            UpdateLedgeDetectorPosition();
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
        if (!IsAlive)
            return;
        if (isHanging)
        {
            StartCoroutine(ClimbUp());
        }
        else if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    void CheckForLedge()
    {
        if (!touchingDirections.IsGrounded && !isHanging && ledgeDetector.ledgeAvailable)
        {
            EnterLedgeHang();
        }
    }

    void EnterLedgeHang()
    {
        isHanging = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        // Snap to ledge position
        Vector3 snapPos = ledgeDetector.transform.position + (Vector3)ledgeHangOffset;
        transform.position = new Vector3(
            Mathf.Round(snapPos.x * 16) / 16f,
            Mathf.Round(snapPos.y * 16) / 16f,
            transform.position.z
        );

        animator.SetBool(AnimationStrings.isHanging, true); // make sure to add this parameter
    }

    void ExitLedgeHang()
    {
        isHanging = false;
        rb.gravityScale = 1;
        animator.SetBool(AnimationStrings.isHanging, false);
    }

    IEnumerator ClimbUp()
    {
        animator.SetTrigger(AnimationStrings.climbUp); // optional
        yield return new WaitForSeconds(0.1f); // simulate climb time

        ExitLedgeHang(); // reset states

        // Snap player to stand on the ledge
        transform.position += new Vector3(0, 1f, 0);
    }

    private void UpdateLedgeDetectorPosition()
{
    float direction = isFacingRight ? 1 : -1;
    ledgeDetectorTransform.localPosition = new Vector3(
        ledgeDetectorOffset.x * direction,
        ledgeDetectorOffset.y,
        ledgeDetectorTransform.localPosition.z
    );
}
}

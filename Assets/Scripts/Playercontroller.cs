using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Import the InputSystem namespace

[System.Serializable]
public struct InputSnapshot
{
    public Vector2 move;
    public bool jump;
    public bool run;
    public bool attack;

    public InputSnapshot(Vector2 move, bool jump, bool run, bool attack)
    {
        this.move = move;
        this.jump = jump;
        this.run = run;
        this.attack = attack;
    }
}








[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable)),] // Require a Rigidbody2D component on the GameObject
public class Playercontroller : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected TouchingDirections touchingDirections;

    public float walkSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool isDashing = false;
    private bool canDash = true;
    private SpriteRenderer spriteRenderer;
    public Color dashColor = new Color(0f, 1f, 221f / 255f);
    private Color originalColor;



    //cositas para manipulacion del tiempo
    public float inputRecordDuration = 2f;
    public float inputSnapshotInterval = 0.1f;
    private float inputSnapshotTimer = 0f;

    private List<InputSnapshot> inputHistory = new List<InputSnapshot>();

    private bool jumpPressed = false;
    protected private bool runHeld = false;
    private bool attackPressed = false;
    public GameObject ghostClonePrefab;

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
        protected private set
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
        protected private set
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


    protected Vector2 moveInput; // Variable to store the movement input
    protected Animator animator; // Variable to store the Animator component

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        ledgeDetector = GetComponentInChildren<LedgeDetector>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateLedgeDetectorPosition(); // Always keep ledge detector on correct side
    }

    void FixedUpdate()
    {
        if (!damageable.LockVelocity && !isDashing)
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y); //makes the character fall/rise


        if (!isHanging && !touchingDirections.IsGrounded && ledgeDetector.ledgeAvailable)
        {
            EnterLedgeHang();
        }


        //time manipulation
        inputSnapshotTimer += Time.fixedDeltaTime;
        if (inputSnapshotTimer >= inputSnapshotInterval)
        {
            inputSnapshotTimer = 0f;

            inputHistory.Add(new InputSnapshot(moveInput, jumpPressed, runHeld, attackPressed));

            // Reset one-time triggers
            jumpPressed = false;
            attackPressed = false;

            // Trim to fit duration
            if (inputHistory.Count > inputRecordDuration / inputSnapshotInterval)
                inputHistory.RemoveAt(0);
        }
    }

    public virtual void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
    }

    public void OnSpawnClone(InputAction.CallbackContext context)
    {
        if (context.started && inputHistory.Count > 0)
        {
            GameObject ghost = Instantiate(ghostClonePrefab, transform.position, Quaternion.identity);
            GhostPlayerController ghostScript = ghost.GetComponent<GhostPlayerController>();
            ghostScript.InitReplay(inputHistory);
        }

         // Trigger the impulse
        CinemachineImpulseSource impulse = GetComponent<CinemachineImpulseSource>();
        if (impulse != null)
        {
            impulse.GenerateImpulse();
        }
    }

    protected private void SetFacingDirection(Vector2 moveInput)
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

    public virtual void OnRun(InputAction.CallbackContext context)
    {
        runHeld = context.ReadValueAsButton(); // true when held down
        // Your existing run logic...
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public virtual void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpPressed = true;

        // Your existing jump logic...
        if (!IsAlive)
            return;
        if (isHanging)
        {
            animator.SetTrigger(AnimationStrings.climbUp); // optional
            StartCoroutine(ClimbUp());
        }
        else if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            attackPressed = true;

        // Your existing attack logic...
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

        // Snap to ledge position, accounting for facing direction
        //
        Vector3 snapPos = ledgeDetector.transform.position + new Vector3(
            ledgeHangOffset.x * (isFacingRight ? 1 : -1),
            ledgeHangOffset.y,
            0f
        );
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
        yield return new WaitForSeconds(0.3f); // simulate climb time

        ExitLedgeHang(); // reset states

        // Snap player to stand on the ledge
        float direction = isFacingRight ? 0.5f : -0.5f;
        transform.position += new Vector3(direction, 1f, 0);
    }

    private void UpdateLedgeDetectorPosition()
    {
        float direction = isFacingRight ? 1 : -1;
        Vector3 newPos = new Vector3(
            ledgeDetectorOffset.x * direction,
            ledgeDetectorOffset.y,
            ledgeDetectorTransform.localPosition.z
        );
        ledgeDetectorTransform.localPosition = newPos;
        Debug.Log($"LedgeDetector localPosition: {ledgeDetectorTransform.localPosition}, isFacingRight: {isFacingRight}");
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && canDash && IsAlive && CanMove)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        spriteRenderer.color = new Color(0f, 0.9f, 5f);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        //slowing time
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (ledgeDetectorTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ledgeDetectorTransform.position, 0.05f);

            // Show ledge snap position based on facing direction and offset
            float direction = _isFacingRight ? 1f : -1f;
            Vector3 snapPos = ledgeDetectorTransform.position + new Vector3(ledgeHangOffset.x * direction, ledgeHangOffset.y, 0f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(snapPos, 0.05f);
        }
}
}

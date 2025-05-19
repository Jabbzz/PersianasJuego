using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Ninja : MonoBehaviour
{

    public float walkStopRate = 0.6f;
    public float walkSpeed = 3f;
    public float stopDistance = 4f;
    Damageable damageable;
    public DetectionZone attackZone;
    public DetectionZone cliffDectionZone;
    public DetectionZone chaseZone;

    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private AudioSource movementAudioSource;

    private bool canPlayFootstep = true;
    [SerializeField] private float footstepCooldown = 0.4f;



    public float flipDelay = 0.5f; // Time in seconds before flipping
    private float flipTimer = 0f;
    private bool playerBehind = false;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.left;


    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                //directions flipped
                //nota: las direcciones son invertidas porque el sprite del caballero mira a la izquierda
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;

    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        damageable.damageableHit.AddListener(OnHit);

        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.playOnAwake = false;
        movementAudioSource.volume = 0.5f;
    }
    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }


    private void FixedUpdate()
    {
        if (touchingDirections.IsOnWall && touchingDirections.IsGrounded || cliffDectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            // Chase logic
            if (chaseZone.detectedColliders.Count > 0)
            {
                // Assume the first collider is the player
                Transform player = chaseZone.detectedColliders[0].transform;
                float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
                float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

                // Determine which way the Knight is facing
                float knightFacing = (WalkDirection == WalkableDirection.Right) ? 1f : -1f;

                // Check if player is behind
                if (Mathf.Sign(directionToPlayer) != knightFacing)
                {
                    if (!playerBehind)
                    {
                        playerBehind = true;
                        flipTimer = flipDelay; // Start the timer
                    }
                }
                else
                {
                    playerBehind = false;
                    flipTimer = 0f;
                }

                // Handle flip delay
                if (playerBehind)
                {
                    flipTimer -= Time.fixedDeltaTime;
                    if (flipTimer <= 0f)
                    {
                        FlipDirection();
                        playerBehind = false;
                    }
                }

                // Stop at mid-range distance
                if (distanceToPlayer > stopDistance)
                {
                    rb.linearVelocity = new Vector2(walkSpeed * directionToPlayer, rb.linearVelocity.y);
                }
                else
                {
                    // Stop moving but keep facing the player
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
            else if (CanMove)
            {
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }

            float speed = Mathf.Abs(rb.linearVelocity.x);

            if (speed > 0 && speed <= walkSpeed) // Caminar
            {
                if (canPlayFootstep && walkSound != null)
                {
                    movementAudioSource.clip = walkSound;
                    movementAudioSource.Play();
                    StartCoroutine(FootstepCooldownRoutine());
                }
            }
            else if (speed > walkSpeed) // Correr
            {
                if (canPlayFootstep && runSound != null)
                {
                    movementAudioSource.clip = runSound;
                    movementAudioSource.Play();
                    StartCoroutine(FootstepCooldownRoutine());
                }
            }
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.Log("current Walk direction is not set to legal values of right or left");
        }
    }

    //Llamar a este método desde la animación
    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioManager.instance.PlaySound(attackSound);
        }
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);

        if (!damageable.IsAlive)
        {
            if (deathSound != null)
                audioManager.instance.PlaySound(deathSound);
        }
        else
        {
            if (hitSound != null)
                audioManager.instance.PlaySound(hitSound);
        }
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
    
    private IEnumerator FootstepCooldownRoutine()
    {
        canPlayFootstep = false;
        yield return new WaitForSeconds(footstepCooldown);
        canPlayFootstep = true;
    }

}
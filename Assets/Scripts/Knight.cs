using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections),typeof(Damageable))]
public class Knight : MonoBehaviour
{

    public float walkStopRate = 0.6f;
    public float walkSpeed = 3f;
    Damageable damageable;
    public DetectionZone attackZone;
    public DetectionZone cliffDectionZone;
    Rigidbody2D rb;
    TouchingDirections touchingDirections; 
    Animator animator;

    public enum WalkableDirection { Right, Left}

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.left;
    

    public WalkableDirection WalkDirection
    {
        get {return _walkDirection;}
        set {
            if (_walkDirection != value)
            {
                //directions flipped
                //nota: las direcciones son invertidas porque el sprite del caballero mira a la izquierda
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.left;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.right;
                }
            }
            
            _walkDirection = value;}
    }

    public bool _hasTarget = false;

    public bool HasTarget { get{
        return _hasTarget;
    } private set
    {
        _hasTarget = value;
        animator.SetBool(AnimationStrings.hasTarget, value);
    } }

    public float AttackCooldown { get
    {
        return animator.GetFloat(AnimationStrings.attackCooldown);
    } private set
    {
        animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    } }

    public bool CanMove
    {
        get {
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
    }
       // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if(AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }    
    }


    private void FixedUpdate()
    {
        if(touchingDirections.IsOnWall && touchingDirections.IsGrounded || cliffDectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }
        if(!damageable.LockVelocity)
        {
            if(CanMove)
            {
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
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

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

}

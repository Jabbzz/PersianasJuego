using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    CapsuleCollider2D touchingCol; 
    Animator animator; 


    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    Rigidbody2D rb; // Reference to the Rigidbody2D component
    [SerializeField]
    private bool _isGrounded;
    public bool IsGrounded { get
    {
        return _isGrounded; 
    } private set
    {
        _isGrounded = value; 
        animator.SetBool(AnimationStrings.isGrounded, value);
    } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingCol = GetComponent<CapsuleCollider2D>(); 
        animator = GetComponent<Animator>(); 
    }


    private void FixedUpdate()
    {
            IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) >0;
    }


}

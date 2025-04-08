using UnityEngine;
using UnityEngine.InputSystem; // Import the InputSystem namespace
[RequireComponent(typeof(Rigidbody2D))] // Require a Rigidbody2D component on the GameObject
public class Playercontroller : MonoBehaviour
{
    Rigidbody2D rb;
    public float walkSpeed = 5f;

    public bool IsMoving { get; private set;}
    Vector2 moveInput; // Variable to store the movement input

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocity.y); // Set the horizontal velocity based on input and walk speed   
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;
    }
}

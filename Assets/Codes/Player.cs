using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 5f;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[12];
    private static readonly int Move = Animator.StringToHash("move");
    private Vector2 moveInput;
    private bool jumpRequested;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Mathf.Abs(moveInput.x) > 0.01f && sprite != null)
        {
            sprite.flipX = moveInput.x < 0f;
        }
        if (animator != null)
        {
            animator.SetBool(Move, Mathf.Abs(moveInput.x) > 0.01f);
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocityX = moveInput.x * speed;

        if (jumpRequested && IsGrounded())
        {
            body.linearVelocityY = jumpPower;
        }
        jumpRequested = false;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpRequested = true;
        }
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;
        jumpRequested = false;
    }

    public void Respawn(Vector2 position)
    {
        body.position = position;
        body.linearVelocity = Vector2.zero;
        jumpRequested = false;
    }

    private bool IsGrounded()
    {
        if (body.linearVelocityY > 0.1f)
        {
            return false;
        }
        int count = body.GetContacts(contacts);
        for (int index = 0; index < count; index++)
        {
            if (contacts[index].normal.y > 0.55f)
            {
                return true;
            }
        }
        return false;
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public GameObject spawnPoint;
    bool isJump = false;
    bool isGrounded = false;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Vector2 InputVector;
    Animator anima;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
    }
    private void Update()
    {
        if (InputVector.x < 0)
        sprite.flipX = true;
        else if (InputVector.x > 0)
            sprite.flipX = false;
        anima.SetBool("move", InputVector.x != 0);
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(InputVector.x * speed, rb.linearVelocityY);
        if(isJump && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            isJump = false;
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
        if (collision.gameObject.CompareTag("Spike"))
        gameObject.transform.position = spawnPoint.transform.position;
    }

    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();

    }
    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            isJump=true;
        }
    }

}
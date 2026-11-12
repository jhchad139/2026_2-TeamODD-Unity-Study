using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public int NowStage;
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
        NowStage = 0;
        Debug.Log("GameManager: " + GameManager.instance);
        GameManager.instance.SpawnPlayer(NowStage);
        if (GameManager.instance != null)
    {
        GameManager.instance.SpawnPlayer(NowStage);
    }
    }
    private void Update()
    {
        if (InputVector.x < 0)
        sprite.flipX = true;
        else if (InputVector.x > 0)
            sprite.flipX = false;
        anima.SetBool("move", InputVector.x != 0);
        if (transform.position.y < -7f)
            GameManager.instance.SpawnPlayer(NowStage);
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
        GameManager.instance.SpawnPlayer(NowStage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            NowStage++;
            GameManager.instance.MoveStage(NowStage);
        }
        if (collision.gameObject.CompareTag("Coin"))
        {
            GameManager.instance.addcoin();
            Destroy(collision.gameObject);
        }
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
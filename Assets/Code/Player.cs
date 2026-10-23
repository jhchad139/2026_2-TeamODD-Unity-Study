using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;

    public GameObject spawnpoint;

    bool isJump = false;
    bool isGrounded = false;

    Rigidbody2D rigid;

    SpriteRenderer sprite;

    Animator anime;

    Vector2 inputVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
    }

    private void Update()
    {
        if(inputVec.x < 0)
            sprite.flipX = true;
        else if (inputVec.x > 0)
            sprite.flipX = false;

        anime.SetBool("move", inputVec.x != 0);
    }

    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(inputVec.x * speed, rigid.linearVelocityY);

        if (isJump && isGrounded)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpPower);

            isJump = false;
            isGrounded = false;
        }
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            isJump = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
            isGrounded = true;

        if(collision.gameObject.CompareTag("Spike"))
            gameObject.transform.position = spawnpoint.transform.position;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{


    public float speed;

    public float JumPow;

    public int nowStage;
    //(nowStage 전달)

    public GameObject spawnPoint;

    bool isJump = false;

    bool isGrounded = false;

    Rigidbody2D rigid;
    Vector2 inputVec;

    SpriteRenderer sprite;
    Animator anima;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
        nowStage = 0;
    }

    private void Update()
    {
        if (inputVec.x < 0)
            sprite.flipX = true;
        else if (inputVec.x > 0)
            sprite.flipX = false;

        anima.SetBool("move", inputVec.x != 0);
        anima.SetBool("jump", !isGrounded);
    }


    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(inputVec.x * speed, rigid.linearVelocityY);

        if(isJump && isGrounded)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, JumPow);
     
            isJump = false;
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Spike"))
            Gamemanager.instance.SpawnPlayer(nowStage);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            transform.position = spawnPoint.transform.position;
            rigid.linearVelocity = Vector2.zero;
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            nowStage++;
            Gamemanager.instance.MoveStage(nowStage);
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            Gamemanager.instance.Addcoin();

            Destroy(collision.gameObject);
        }
    }

    void OnMove(InputValue value)
    {

        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if(value.isPressed && isGrounded)
        {
            isJump = true;
        }
    }
}

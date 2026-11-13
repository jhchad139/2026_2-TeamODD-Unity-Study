using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameManager GameManager;
    public float speed;
    public float jumppower;
    public GameObject spawn;

    public int nowStage;

    bool isjump = false;
    bool isGrounded = false;

    Rigidbody2D rigid;
    SpriteRenderer sprite;

    Animator anima;
    Vector2 inputvec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
        nowStage = 0;

    }

    private void Update()
    {
        if(inputvec.x < 0)
        {
            sprite.flipX = true;
        }
        else if(inputvec.x > 0)
        {
            sprite.flipX = false;
        }
        anima.SetBool("moving", isGrounded && inputvec.x !=0);
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(inputvec.x*speed,rigid.linearVelocityY);

        if(isjump && isGrounded)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumppower);
            isjump = false;
            isGrounded = false;
        }
        anima.SetBool("isGrounded", isGrounded);

        anima.SetFloat("yVelocity", rigid.linearVelocityY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            GameManager.instance.SpawnPlayer(nowStage);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            nowStage++;
            GameManager.instance.MoveStage(nowStage);
        }

        if (collision.gameObject.CompareTag("coin"))
        {
            GameManager.instance.Addcoin();

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
            GameManager.instance.SpawnPlayer(nowStage);
    }

    void OnMove(InputValue value)
    {
        inputvec = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (value.isPressed&&isGrounded)
        {
            isjump = true;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;
    public GameObject spawnPoint;

    bool isJump = false;
    bool isGrounded = false;

    Rigidbody2D rigid;

    SpriteRenderer sprite;
    Animator anima;

    Vector2 inputVec;

    private void Awake(){
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
    }

    private void Update(){
        if(inputVec.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        anima.SetBool("move", inputVec.x != 0);
    }

    void FixedUpdate(){
        rigid.linearVelocity = new Vector2(inputVec.x * speed,rigid.linearVelocityY);

        if(isJump && isGrounded){
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX,jumpPow);

            isJump = false;
            isGrounded = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if(collision.gameObject.CompareTag("Spike"))
            gameObject.transform.position = spawnPoint.transform.position;
    }

    void OnMove(InputValue value){
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if(value.isPressed && isGrounded){
            isJump = true;
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;
    public GameObject spawnPoint;

    bool isJump = false;  // 점프를 눌렀는지 체크

    bool isGrounded = false;  // 땅에 닿아있는지 체크

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anima;

    Vector2 inputVec;

    // Awake -> 코드가 실행될 때 가장 먼저 실행됨
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
    }

    // 화면 프레임 업데이트
    private void Update()
    {
        if (inputVec.x < 0) // 왼쪽 보고있을 때
            sprite.flipX = true;
        else if (inputVec.x >0) // 오른쪽 보고있을 때
            sprite.flipX = false;
    
        anima.SetBool("move", inputVec.x != 0);
    }

    // 물리적인 이동 업데이트
    private void FixedUpdate()
    {
        // 좌우 눌렀을 때 위치 이동
        rigid.linearVelocity = new Vector2(inputVec.x * speed,
            rigid.linearVelocityY);
        
        // 점프했을 때 위치 이동
        if(isJump && isGrounded)
        {
            rigid.linearVelocity = new Vector2(
                rigid.linearVelocityX, jumpPow);

            isJump = false;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
        
        if (collision.gameObject.CompareTag("Spike"))
            gameObject.transform.position = spawnPoint.transform.position;
    }

    // 좌우 이동 함수
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 점프 함수
    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            isJump = true;
        }
    }
}

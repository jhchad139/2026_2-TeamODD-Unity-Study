using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;

    bool isJump = false;

    Rigidbody2D rigid;

    Vector2 inputVec;

    // Awake -> 코드가 실행될 때 가장 먼저 실행됨
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 좌우 눌렀을 때 위치 이동
        rigid.linearVelocity = new Vector2(inputVec.x * speed,
            rigid.linearVelocityY);
        
        // 점프했을 때 위치 이동
        if(isJump)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX,
                jumpPow);

            isJump = false;
        }
    }

    // 좌우 이동 함수
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 점프 함수
    void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            isJump = true;
        }
    }
}

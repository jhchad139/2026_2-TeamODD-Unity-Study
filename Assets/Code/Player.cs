using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;

    bool isJump = false; // 지금 점프함
    Vector2 inputVec;
    Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(
            inputVec.x*speed,rigid.linearVelocityY
            );

        if (isJump) {
            rigid.linearVelocity = new Vector2(
            rigid.linearVelocityX, jumpPow
            );

            isJump = false;
        }
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        inputVec.y = 0;
    }

    void OnJump(InputValue value)
    {
        if(value.isPressed)
            isJump = true;
    }
}

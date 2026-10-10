using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;

    bool isJump = false;

    Rigidbody2D rigid;

    Vector2 inputVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(inputVec.x * speed, 
            rigid.linearVelocityY); 

        if(isJump)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX,
                jumpPow);

            isJump = false;
        }
    }


    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

   
    void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            isJump = true;
        }
    }

}

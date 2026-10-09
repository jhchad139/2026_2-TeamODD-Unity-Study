using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    bool isJump = false;
    Rigidbody2D rb;
    Vector2 InputVector;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(InputVector.x * speed, rb.linearVelocityY);
        if(isJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            isJump = false;
        }
    }

    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();

    }
    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            isJump=true;
        }
    }

}
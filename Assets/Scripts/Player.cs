using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    private Vector2 inputVec;
    
    public float speed;
    public float jumpPower;

    private bool isJump = false;

    void Awake(){
        rigid = this.GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value){
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if(value.isPressed)
            isJump = true;
    }

    void FixedUpdate(){
        rigid.linearVelocity = new Vector2(inputVec.x * speed, rigid.linearVelocityY); //현재 속도
        
        if(isJump){
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpPower);

            isJump = false;
        }
    }
}

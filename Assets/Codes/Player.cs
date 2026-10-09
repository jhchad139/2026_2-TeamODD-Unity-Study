using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    
    private Vector2 _inputVec;
    private bool _isJump;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_inputVec.x * speed, _rb.linearVelocityY);
        if (_isJump)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpPower);
            
            _isJump = false;
        }
    }

    public void OnMove(InputValue value)
    {
        _inputVec = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed) _isJump = true;
    }
}

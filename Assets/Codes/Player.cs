using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private Animator _animator;
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    public int nowStage;
    
    [SerializeField] private Camera cam;
    
    private Vector2 _inputVec;
    private bool _isJump;
    private bool _isGrounded = false;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(_inputVec.x < 0)
            _sprite.flipX = true;
        else if(_inputVec.x > 0)
            _sprite.flipX = false;
        
        _animator.SetBool("move", _inputVec.x != 0);
        _animator.SetBool("isRise", _rb.linearVelocity.y > 0);
        _animator.SetBool("isFall", _rb.linearVelocity.y < 0);
        
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            GameManager.Instance.SpawnPlayer(nowStage);
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_inputVec.x * speed, _rb.linearVelocityY);
        if (_isJump && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpPower);
            _animator.SetTrigger("jump");
            
            _isJump = false;
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) _isGrounded = true;
        if(collision.gameObject.CompareTag("Spike")) GameManager.Instance.SpawnPlayer(nowStage);
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) _isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            nowStage++;
            GameManager.Instance.MoveStage(nowStage);
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin();
            Destroy(collision.gameObject);
        }
    }

    public void OnMove(InputValue value)
    {
        _inputVec = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed && _isGrounded) _isJump = true;
    }
}

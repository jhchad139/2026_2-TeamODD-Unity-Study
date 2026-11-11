using System.Diagnostics;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;
    public int nowStage;  // Gamemanager에 전달

    public GameObject spawnPoint;

    public Sprite fallSprite;  // 점프 없이 떨어질 때 보여줄 스프라이트 (Assets/Anima/Jump 2)

    bool jumpRequested = false;  // 점프 키를 눌렀는지 체크 (다음 물리 업데이트에서 점프)

    bool isJump = false;  // 점프해서 공중에 있는지 체크 (착지하면 false)

    bool isGrounded = false;  // 땅을 "위에서" 밟고 있는지 체크

    ContactPoint2D[] contacts = new ContactPoint2D[16];  // 접촉 지점을 담아둘 배열

    string currentAnim = "";  // 지금 재생 중인 애니메이션 이름

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
        nowStage = 0;

        // 마찰력 0인 물리 재질 적용 -> 벽을 밀고 있어도 붙지 않고 미끄러져 내려감
        rigid.sharedMaterial = new PhysicsMaterial2D("PlayerNoFriction")
        {
            friction = 0f,
            bounciness = 0f
        };
    }

    // 화면 프레임 업데이트
    private void Update()
    {
        if (inputVec.x < 0) // 왼쪽 보고있을 때
            sprite.flipX = true;
        else if (inputVec.x > 0) // 오른쪽 보고있을 때
            sprite.flipX = false;

        UpdateAnimation();

        // 카메라 아래쪽으로 완전히 벗어나면 리스폰
        if (IsBelowCamera())
            Respawn();
    }

    // 상태에 맞는 애니메이션 / 스프라이트로 바꿔주기
    void UpdateAnimation()
    {
        string nextAnim;

        if (isJump)
            nextAnim = "player 0 jump";   // 점프해서 공중에 있음
        else if (isGrounded)
            nextAnim = inputVec.x != 0 ? "player 0 run" : "player 0 idle";  // 땅에서 이동 / 정지
        else
            nextAnim = "fall";            // 점프 없이 떨어지는 중

        if (nextAnim == currentAnim) return;  // 같은 상태면 다시 재생하지 않음
        currentAnim = nextAnim;

        if (nextAnim == "fall")
        {
            // Animator가 켜져 있으면 매 프레임 스프라이트를 덮어쓰므로 끄고 직접 교체
            anima.enabled = false;
            sprite.sprite = fallSprite;
        }
        else
        {
            anima.enabled = true;
            anima.Play(nextAnim);
        }
    }

    // 플레이어가 메인 카메라의 아래 끝보다 더 아래에 있는지 체크
    bool IsBelowCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        // 뷰포트 (0, 0) = 화면 왼쪽 아래 → 월드 좌표로 변환해서 아래 끝의 y값을 구함
        float distance = transform.position.z - cam.transform.position.z;
        float camBottomY = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance)).y;

        // 스프라이트의 윗부분(max.y)까지 아래 끝보다 낮으면 = 완전히 벗어남
        return sprite.bounds.max.y < camBottomY;
    }

    // 가시에 닿거나, 맵 밖으로 떨어졌을 때 리스폰
    void Respawn()
    {
        rigid.linearVelocity = Vector2.zero;  // 떨어지던 속도 초기화

        Gamemanager.instance.DecreaseCoin();  // 코인 수 감소
        Gamemanager.instance.SpawnPlayer(nowStage);  // 현재 스테이지의 스폰포인트로 이동

    }

    // 물리적인 이동 업데이트
    private void FixedUpdate()
    {
        // 땅을 위에서 밟고 있는지 매 물리 프레임 확인
        isGrounded = CheckGrounded();

        if (isGrounded && inputVec.x == 0 && !jumpRequested)
        {
            rigid.gravityScale = 0f;
            rigid.linearVelocity = Vector2.zero;
        }
        else
        {
            rigid.gravityScale = 1f;
        }

        if (isGrounded)
            isJump = false;  // 착지했으니 점프 끝

        // 좌우 눌렀을 때 위치 이동
        rigid.linearVelocity = new Vector2(inputVec.x * speed,
            rigid.linearVelocityY);

        // 점프했을 때 위치 이동
        if (jumpRequested && isGrounded)
        {
            rigid.linearVelocity = new Vector2(
                rigid.linearVelocityX, jumpPow);

            isJump = true;  // 착지할 때까지 true로 유지
            isGrounded = false;
        }
        jumpRequested = false;
    }

    // 닿아 있는 Ground 중에 "발 아래"에 있는 것이 있는지 확인
    bool CheckGrounded()
    {
        if (rigid.linearVelocityY > 0.01f) return false;  // 위로 올라가는 중이면 착지 아님

        int count = rigid.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            if (!contacts[i].collider.CompareTag("Ground")) continue;

            // normal = 부딪힌 면이 플레이어를 미는 방향
            // 위쪽(y > 0.7)을 향하면 바닥, 옆을 향하면 벽, 아래를 향하면 천장
            if (contacts[i].normal.y > 0.7f)
                return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
            Respawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            nowStage++;
            Gamemanager.instance.MoveStage(nowStage);
        }

        if (collision.gameObject.CompareTag("coin"))
        {
            Gamemanager.instance.AddCoin();

            Destroy(collision.gameObject);
        }
    }

    // 좌우 이동 함수
    public void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 점프 함수
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            jumpRequested = true;
        }
    }
}

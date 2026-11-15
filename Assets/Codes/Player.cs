using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPow;

    public int nowStage = 0;

    public bool canMove = true; // 클리어 시 조작 차단

    [Header("점프 보정")]
    public float coyoteTime = 0.1f;  // 땅에서 떨어진 직후에도 이 시간 동안은 점프 가능
    public float jumpBuffer = 0.15f; // 착지 직전에 누른 점프도 이 시간 동안 기억

    bool isGrounded = false;
    bool wasGrounded = false;   // 바로 전 물리 프레임에 땅에 있었는지
    float lastGroundedTime = -10f;
    float lastJumpPressedTime = -10f;
    float lastJumpTime = -10f;   // 실제로 점프한 시각
    float defaultGravity;

    Rigidbody2D rigid;
    Collider2D col;
    SpriteRenderer sprite;
    Animator anima;

    Vector2 inputVec;
    float groundGap = 0f;          // 발과 땅 사이 거리
    Vector2 groundNormal = Vector2.up; // 발밑 땅이 바라보는 방향 (평지는 위쪽, 비탈은 기울어짐)
    readonly ContactPoint2D[] contacts = new ContactPoint2D[16];
    const float checkDist = 0.3f;  // 발밑으로 땅을 찾는 거리

    private void Awake(){
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
        defaultGravity = rigid.gravityScale;
    }

    private void Update(){
        if(inputVec.x < 0)
            sprite.flipX = true;
        else if(inputVec.x > 0)
            sprite.flipX = false;

        anima.SetBool("move", canMove && inputVec.x != 0);

        // 낙사
        if(transform.position.y < -10f)
            Die();
    }

    void FixedUpdate(){
        wasGrounded = isGrounded;
        isGrounded = CheckGrounded();
        if(isGrounded)
            lastGroundedTime = Time.time;

        if(!canMove){
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocityY);
            return;
        }

        float moveX = inputVec.x * speed;

        if(isGrounded && moveX != 0 && Time.time - lastJumpTime > 0.1f){
            // 땅 위에서 걸을 때는 땅의 기울기를 따라 이동
            // (옆으로만 밀면 오르막에서 벽을 미는 것처럼 힘이 빠져서 못 올라감)
            Vector2 along = new Vector2(groundNormal.y, -groundNormal.x); // 비탈을 따라 오른쪽 방향
            Vector2 v = along * moveX;

            // 내리막(또는 평지)에서 발이 살짝 떠 있으면 땅에 붙여줌. 오르막에서는 하지 않음
            if(wasGrounded && v.y <= 0.01f && groundGap > 0.01f)
                v.y -= Mathf.Min(groundGap / Time.fixedDeltaTime, speed);

            rigid.linearVelocity = v;
        }
        else {
            rigid.linearVelocity = new Vector2(moveX, rigid.linearVelocityY);
        }

        // 비탈에 가만히 서 있을 때 미끄러져 내려가지 않도록 중력을 잠깐 끔
        if(isGrounded && groundGap < 0.05f && inputVec.x == 0 && Time.time - lastJumpTime > 0.1f){
            rigid.gravityScale = 0f;
            rigid.linearVelocity = Vector2.zero;
        }
        else {
            rigid.gravityScale = defaultGravity;
        }

        bool jumpRequested = Time.time - lastJumpPressedTime <= jumpBuffer;
        bool canJump = Time.time - lastGroundedTime <= coyoteTime;

        if(jumpRequested && canJump){
            rigid.gravityScale = defaultGravity;
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, jumpPow);
            anima.SetTrigger("jump");
            AudioManager.Play(Sfx.Jump);
            lastJumpTime = Time.time;

            lastJumpPressedTime = -10f;
            lastGroundedTime = -10f; // 공중 2단 점프 방지
            isGrounded = false;
        }
    }

    // 땅 판정 2단계
    // 1) 실제로 몸이 닿아 있는 면 중에 발밑 쪽 면이 있으면 땅 (평지, 비탈, 착지 순간 모두 정확)
    // 2) 바로 전까지 땅 위를 걷던 중이면, 발밑 0.3칸 안에 땅이 있어도 땅으로 인정 (내리막에서 살짝 뜨는 경우)
    bool CheckGrounded(){
        groundGap = 0f;
        groundNormal = Vector2.up;

        // 점프 직후 0.1초는 공중으로 처리
        if(Time.time - lastJumpTime < 0.1f) return false;

        Bounds b = col.bounds;

        // ---- 1) 닿아 있는 면 검사 ----
        int n = rigid.GetContacts(contacts);
        bool touching = false;
        float bestScore = -10f;
        for(int i = 0; i < n; i++){
            ContactPoint2D c = contacts[i];
            Collider2D other = c.collider == col ? c.otherCollider : c.collider;
            if(other == null || other.isTrigger) continue;
            if(other.CompareTag("Spike")) continue;

            if(c.point.y > b.min.y + 0.2f) continue;     // 발 근처 접촉만

            Vector2 normal = c.normal.y < 0 ? -c.normal : c.normal; // 위쪽을 향하게
            if(normal.y < 0.5f) continue;                 // 벽이나 너무 가파른 면

            // 여러 면에 닿아 있으면, 가려는 방향의 오르막 면을 우선
            float score = normal.y;
            if(inputVec.x != 0 && normal.x * inputVec.x < 0) score += 1f;
            if(score > bestScore){
                bestScore = score;
                groundNormal = normal;
            }
            touching = true;
        }
        if(touching) return true;

        // ---- 2) 걷던 중이면 발밑 조금 아래까지 인정 ----
        if(!wasGrounded) return false;

        float startY = b.min.y + 0.05f;
        Vector2 origin = new Vector2(b.center.x, startY);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.05f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, checkDist);
        foreach(RaycastHit2D hit in hits){
            Collider2D c = hit.collider;
            if(c == col || c.isTrigger) continue;
            if(c.CompareTag("Spike") || c.CompareTag("Player")) continue;
            if(hit.normal.y < 0.5f) continue;

            // 한쪽 방향 발판: 아래에서 뚫고 올라가는 중이면 땅이 아님
            if(c.usedByEffector && hit.point.y > b.min.y + 0.05f) continue;

            groundGap = Mathf.Max(0f, hit.distance - 0.035f);
            groundNormal = hit.normal;

            // 기울기는 발 가운데에서 아래로 쏜 레이로 한 번 더 정확하게 측정
            foreach(RaycastHit2D ray in Physics2D.RaycastAll(new Vector2(b.center.x, b.min.y + 0.1f), Vector2.down, 0.6f)){
                if(ray.collider == col || ray.collider.isTrigger) continue;
                if(ray.normal.y >= 0.5f) groundNormal = ray.normal;
                break;
            }
            return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Spike"))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Finish"))
            GameManager.instance.StageClear();

        if(collision.gameObject.CompareTag("coin")){
            GameManager.instance.AddCoin();
            AudioManager.Play(Sfx.Coin);
            Destroy(collision.gameObject);
        }
    }

    void Die(){
        AudioManager.Play(Sfx.Death);
        GameManager.instance.SpawnPlayer(nowStage);
    }

    // 리스폰/스테이지 이동 시 속도와 상태 초기화
    public void ResetState(){
        if(rigid == null) rigid = GetComponent<Rigidbody2D>();
        rigid.linearVelocity = Vector2.zero;
        if(defaultGravity > 0f) rigid.gravityScale = defaultGravity;
        isGrounded = false;
        lastGroundedTime = -10f;
        lastJumpPressedTime = -10f;
    }

    void OnMove(InputValue value){
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if(canMove && value.isPressed)
            lastJumpPressedTime = Time.time;
    }

    // Scene 뷰에서 바닥 검사 범위를 초록 상자로 보여줌
    private void OnDrawGizmosSelected(){
        Collider2D c = GetComponent<Collider2D>();
        if(c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = Color.green;
        float top = b.min.y + 0.075f, bottom = b.min.y + 0.025f - checkDist;
        Gizmos.DrawWireCube(new Vector3(b.center.x, (top + bottom) / 2f, 0), new Vector3(b.size.x * 0.9f, top - bottom, 0));
    }
}

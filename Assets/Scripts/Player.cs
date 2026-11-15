using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anime;
    Collider2D coll2d; // 플레이어 충돌 처리를 끄고 켜기 위한 변수

    private Vector2 inputVec;
    private Vector3 lastPosition;
    public GameObject spawnPoint;
    
    public float speed;
    public float jumpPower;
    public int nowStage = 0;
    public int coinCount = 0;

    private bool isGroundEnd = false;
    private bool isJump = false;
    public bool isRespawning = false; // 중복 리스폰 방지용 플래그

    [Header("Dash Settings")]
    public float dashMultiplier = 1.8f; // Shift 누를 때 속도 배율 (원하는 만큼 조절)
    private bool isSprintPressed = false; // Shift 키를 누르고 있는지 여부
    private bool canDash = true;        // 땅에 닿으면 다시 true로 초기화되는 대시 가능 플래그

    void Awake(){
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        coll2d = GetComponent<Collider2D>();

        GameManager.Instance.MoveStage(nowStage);
    }

    public void SetSpawnPoint(GameObject target){
        spawnPoint = target;
    }

    void OnMove(InputValue value){
        if (isRespawning) return; // 리스폰 중에는 입력 차단
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if (isRespawning) return; // 리스폰 중에는 점프 차단
        if(value.isPressed)
            isJump = true;
    }


    void OnSprint(InputValue value)
    {
        if (isRespawning) return;
        isSprintPressed = value.isPressed;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("ground")){
            isGroundEnd = true;
            lastPosition = transform.position;
            
            // 땅에 닿으면 대시 상태 초기화
            canDash = true;
        }
        if(collision.gameObject.CompareTag("Spike")){
            Respawn();
        }
        if(collision.gameObject.CompareTag("out world")){
            Respawn();
        }
        if(collision.gameObject.CompareTag("Coin")){
            AddCoin();
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            nowStage++;
            GameManager.Instance.MoveStage(nowStage);
        }
    }

    public void Respawn(){
        if (!isRespawning)
            StartCoroutine(RespawnRoutine());
    }

    // 발판 코드와 유사한 구조의 리스폰 코루틴
    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // 리스폰 시 대시 관련 상태 초기화
        isSprintPressed = false;
        canDash = true;

        // 1. 죽는 순간 플레이어 숨기기 및 물리/입력 정지
        if (sprite != null) sprite.enabled = false;
        if (coll2d != null) coll2d.enabled = false;
        if (rigid != null)
        {
            rigid.simulated = false; // 물리 연산 완전 정지 (중력 영향 안 받게)
            rigid.linearVelocity = Vector2.zero;
        }
        inputVec = Vector2.zero;

        // 2. 원하는 대기 시간 (예: 1초 동안 사망 연출 대기)
        yield return new WaitForSeconds(0.1f);

        // 3. 스폰 포인트 위치로 이동
        transform.position = spawnPoint.transform.position;

        // 4. 플레이어 다시 보이기 및 물리/충돌 켜기
        if (sprite != null) sprite.enabled = true;
        if (coll2d != null) coll2d.enabled = true;
        if (rigid != null)
        {
            rigid.simulated = true;
            rigid.linearVelocity = Vector2.zero;
        }

        isRespawning = false;
    }

    void FixedUpdate(){
        if (isRespawning) return;

        // 현재 속도 계산 (Shift를 누르고 있고 대시가 가능한 상태라면 dashMultiplier 곱함)
        float currentSpeed = speed;
        if (isSprintPressed && canDash && inputVec.x != 0)
        {
            currentSpeed *= dashMultiplier;
        }

        rigid.linearVelocity = new Vector2(inputVec.x * currentSpeed, rigid.linearVelocity.y); 
        
        if(isJump && isGroundEnd){
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpPower);

            isJump = false;
            isGroundEnd = false;
        }
    }

    void Update(){
        if (isRespawning) return;

        if(inputVec.x < 0)
            sprite.flipX = true;
        else if(inputVec.x > 0)
            sprite.flipX = false;

        if (transform.position.y <= -10f)
        {
            Respawn();
        }


        anime.SetBool("isMove", inputVec.x != 0);
    }

    void AddCoin(){
        coinCount++;
    }
}
using System.Collections;
using UnityEngine;

public class FallingPf : MonoBehaviour
{
    private Vector3 startPosition; // 처음에 있던 위치를 저장할 변수
    private SpriteRenderer spriteRenderer; // 시각적으로 껐다 켜기 위한 컴포넌트 (선택 사항)
    private Collider2D coll2d;

    private void Start()
    {
        // 게임 시작 시 초기 위치와 컴포넌트 캐싱
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll2d = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0.1f)
            return;

        if (collision.gameObject.CompareTag("Player")) // 이미지의 이전 코드 및 일반적인 대소문자 규칙에 맞춰 "Player"로 수정했습니다.
            StartCoroutine(Dissaper());
    }

    IEnumerator Dissaper()
    {
        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 오브젝트를 파괴하는 대신 숨기고(비활성화) 충돌을 막음
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (coll2d != null) coll2d.enabled = false;
    }

    // 다시 생성(활성화)하는 함수
    IEnumerator Respawn(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // 위치 초기화
        transform.position = startPosition;

        // 물리 및 시각 요소 다시 켜기
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (coll2d != null) coll2d.enabled = true;
        
        // 만약 강제 리셋해야 할 물리적 속도가 있다면 여기서 초기화 (예: Rigidbody2D)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
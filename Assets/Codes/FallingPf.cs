using System.Collections;
using UnityEngine;

public class FallingPf : MonoBehaviour
{
    Renderer[] renderers;
    Collider2D[] colliders;
    Coroutine disappearRoutine;  // 사라지는 중인 코루틴 (중복 실행 방지)

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    // 활성화될 때 리스폰 이벤트 구독, 비활성화될 때 해제
    private void OnEnable()
    {
        Gamemanager.OnPlayerRespawn += ResetPlatform;
    }

    private void OnDisable()
    {
        Gamemanager.OnPlayerRespawn -= ResetPlatform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0.1f)
            return;

        if (collision.gameObject.CompareTag("Player") && disappearRoutine == null)
            disappearRoutine = StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1);
        SetVisible(false);  // Destroy 대신 숨기기만 함
        disappearRoutine = null;
    }

    // 플레이어가 리스폰되면 호출됨
    void ResetPlatform()
    {
        if (disappearRoutine != null)
        {
            StopCoroutine(disappearRoutine);
            disappearRoutine = null;
        }
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers)
            r.enabled = visible;
        foreach (Collider2D c in colliders)
            c.enabled = visible;
    }
}

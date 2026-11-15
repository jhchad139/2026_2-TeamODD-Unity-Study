using UnityEngine;
using System.Collections;

// 플레이어가 위에서 밟으면 깜빡이다가 떨어지고, 일정 시간 뒤 다시 나타나는 발판
public class FallingPf : MonoBehaviour
{
    public float warnTime = 0.6f;     // 밟은 뒤 떨어지기까지 시간 (이동안 깜빡임)
    public float fallTime = 0.5f;     // 떨어지는 연출 시간
    public float respawnTime = 3f;    // 다시 나타나기까지 시간

    Vector3 startPos;
    Collider2D col;
    SpriteRenderer sr;
    Color startColor;
    bool triggered = false;
    bool initialized = false;

    private void Awake(){
        startPos = transform.position;
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        initialized = true;
    }

    private void OnDisable(){
        ResetPlatform();
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(triggered) return;
        if(!collision.gameObject.CompareTag("Player")) return;

        // 플레이어 발이 발판 윗면 근처에 있을 때만 (옆/아래에서 부딪힌 건 무시)
        if(collision.collider.bounds.min.y < col.bounds.max.y - 0.1f) return;

        StartCoroutine(Fall());
    }

    IEnumerator Fall(){
        triggered = true;
        AudioManager.Play(Sfx.Crumble);

        // 1. 경고: 위치는 그대로 두고 색만 깜빡임 (발판이 움직이면 점프가 씹힘)
        float t = 0;
        while(t < warnTime){
            bool dark = ((int)(t * 12f)) % 2 == 0;
            sr.color = dark ? startColor * new Color(1f, 0.6f, 0.6f, 1f) : startColor;
            t += Time.deltaTime;
            yield return null;
        }
        sr.color = startColor;

        // 2. 떨어지면서 사라지기
        col.enabled = false;
        t = 0;
        while(t < fallTime){
            t += Time.deltaTime;
            transform.position = startPos + Vector3.down * (t * t * 10f);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, startColor.a * (1f - t / fallTime));
            yield return null;
        }
        sr.enabled = false;

        // 3. 잠시 후 다시 등장
        yield return new WaitForSeconds(respawnTime);
        ResetPlatform();
    }

    public void ResetPlatform(){
        if(!initialized) return;

        StopAllCoroutines();
        triggered = false;
        transform.position = startPos;
        col.enabled = true;
        sr.enabled = true;
        sr.color = startColor;
    }
}

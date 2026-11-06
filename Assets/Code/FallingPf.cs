using UnityEngine;
using System;
using System.Collections;

public class FallingPf : MonoBehaviour
{

    private static event Action OnResetAll;
    public static void ResetAll() => OnResetAll?.Invoke();

    private Collider2D col;
    private SpriteRenderer sr;
    private Coroutine disappearRoutine;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable() => OnResetAll += ResetPlatform;
    private void OnDisable() => OnResetAll -= ResetPlatform;

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
        col.enabled = false;
        sr.enabled = false;
        disappearRoutine = null;
    }

    private void ResetPlatform()
    {
       
        if (disappearRoutine != null)
        {
            StopCoroutine(disappearRoutine);
            disappearRoutine = null;
        }
        col.enabled = true;
        sr.enabled = true;
    }
}

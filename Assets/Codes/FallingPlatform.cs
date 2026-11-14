using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float disappearDelay = 1f;

    private Coroutine countdown;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (countdown == null && collision.gameObject.GetComponent<Player>() != null)
        {
            countdown = StartCoroutine(Disappear());
        }
    }

    public void ResetPlatform()
    {
        if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearDelay);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        countdown = null;
    }
}

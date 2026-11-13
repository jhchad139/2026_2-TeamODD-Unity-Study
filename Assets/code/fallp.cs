using System.Collections;
using UnityEngine;

public class fallp : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    bool isFall = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        col = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.relativeVelocity.y > 0.1f)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Dissap());
        }
    }
    IEnumerator Dissap()
    {
        isFall = true;
        
        yield return new WaitForSeconds(1);
        spriteRenderer.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(3);
        spriteRenderer.enabled = true;
        col.enabled = true;

    }
}

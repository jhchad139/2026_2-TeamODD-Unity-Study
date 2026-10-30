using UnityEngine;
using System.Collections;

public class FallingPf : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0.1f)
            return;

        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(Dissaper());
    }

    IEnumerator Dissaper()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}


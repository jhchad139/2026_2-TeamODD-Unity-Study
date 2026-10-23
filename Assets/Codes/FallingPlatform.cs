using System;
using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.y > 0.1f) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

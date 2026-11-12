using System.Collections;
using UnityEngine;

public class Fallingplatform : MonoBehaviour
{
    public static Fallingplatform instance;
       private void Awake()
    {
        instance = this;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0.1f)
        return;

        if (collision.gameObject.CompareTag("Player"))
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
    public void respawn()
    {
        gameObject.SetActive(true);
    }

}

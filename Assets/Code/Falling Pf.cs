using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.relativeVelocity.y > 0.1f)
            return;


        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(Disap());
    }

    IEnumerator Disap()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}

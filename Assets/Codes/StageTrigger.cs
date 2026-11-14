using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StageTrigger : MonoBehaviour
{
    public enum Kind { Spike, Finish, Coin }

    [SerializeField] private Kind kind;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameManager == null || gameManager.IsFinished || other.GetComponent<Player>() == null)
        {
            return;
        }

        switch (kind)
        {
            case Kind.Spike:
                gameManager.LoseLife();
                break;
            case Kind.Finish:
                gameManager.MoveToNextStage();
                break;
            case Kind.Coin:
                gameObject.SetActive(false);
                gameManager.AddCoin();
                break;
        }
    }
}

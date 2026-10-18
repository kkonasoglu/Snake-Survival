using UnityEngine;

[RequireComponent(typeof(SnakeMovement))]
public class SnakeCollision : MonoBehaviour
{
    private SnakeMovement snakeMovement;
    private void Awake()
    {
        snakeMovement = GetComponent<SnakeMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Item>(out var item))
        {
            item.Collect(snakeMovement);
            return;
        }

        if(collision.CompareTag("Wall"))
        {
            snakeMovement.ResetState();
            return;
        }

        if(collision.CompareTag("SnakeBody"))
        {
            if (snakeMovement.Segments != null && snakeMovement.Segments.Count > 1 && collision.transform == snakeMovement.Segments[1])
            {
                return;
            }

            snakeMovement.ResetState();
        }
    }

}
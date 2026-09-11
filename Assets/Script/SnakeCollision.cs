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

        if(collision.CompareTag("Wall") || collision.CompareTag("SnakeBody"))
        {
            snakeMovement.ResetState();
        }
    }

}
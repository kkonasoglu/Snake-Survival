using UnityEngine;

public class BananaItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        base.Collect(snake);

        // Kafa ile kuyruk yer değiştirir
        snake.ReverseSnake();

        // 1 boy büyür
        snake.Grow();

        Respawn();
    }
}

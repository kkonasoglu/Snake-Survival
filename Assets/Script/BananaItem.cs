using UnityEngine;

public class BananaItem : Item
{
    protected override void Awake()
    {
        base.Awake();
        scoreValue = 4;
    }
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

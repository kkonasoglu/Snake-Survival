using UnityEngine;

public class DevilFruitItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        snake.Grow();
        snake.Grow();
        Respawn();
    }
}

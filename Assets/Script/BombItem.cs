using UnityEngine;

public class BombItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        snake.Shrink();
        Respawn();
    }
}

using UnityEngine;

public class BananaItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        snake.Grow();
        Respawn();
    }
}

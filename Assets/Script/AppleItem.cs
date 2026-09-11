using UnityEngine;

public class AppleItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        snake.Grow();
        Respawn();
    }
}

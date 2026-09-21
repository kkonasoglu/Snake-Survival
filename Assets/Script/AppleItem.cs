using UnityEngine;

public class AppleItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        base.Collect(snake);
        
        snake.Grow();
        snake.Accelerate();
        Respawn();
    }
}

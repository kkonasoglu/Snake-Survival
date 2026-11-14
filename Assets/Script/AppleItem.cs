using UnityEngine;

public class AppleItem : Item
{
    public override void Collect(SnakeMovement snake)
    {
        base.Collect(snake);
        ParticleManager.Instance?.PlayEatEffect(transform.position, Color.red);
        AudioManager.Instance?.PlayAppleEat();
        snake.Grow();
        snake.Accelerate();
        Respawn();
    }
}

using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DevilFruitItem : Item
{
    protected override void Awake()
    {
        base.Awake();
        scoreValue = 8;
    }
    public override void Collect(SnakeMovement snake)
    {
        base.Collect(snake);
        snake.ActivateDevilFruit();

        Respawn();
    }
    

}

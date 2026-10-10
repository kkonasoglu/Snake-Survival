using UnityEngine;

public class BombItem : Item
{
    protected override void Awake()
    {
        base.Awake();
        scoreValue = 0;
    }
    public override void Collect(SnakeMovement snake)
    {
        Debug.Log("<color = red><b>[BombItem] BOOM! yılan bomyaya çarptı ve öldü</b></color>");
        snake.ResetState();

        Respawn();
    }
}

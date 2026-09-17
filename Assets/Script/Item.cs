using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour
{
    [Header("Spawn Bounds")]
    [SerializeField] protected Vector2Int gridMin = new Vector2Int(-12, -7);
    [SerializeField] protected Vector2Int gridMax = new Vector2Int(12, 7);

    protected virtual void OnEnable()
    {
        SnakeMovement.OnSnakeReset += Respawn;

    }

    protected virtual void Start()
    {
        Respawn();
    }

    protected virtual void OnDisable()
    {
        SnakeMovement.OnSnakeReset -= Respawn;
    }

    public abstract void Collect(SnakeMovement snake);

    public virtual void Respawn()
    {
        int MaxAttempts = 100;
        int attempt = 0;
        int x, y;
        do
        {
            x = Random.Range(gridMin.x, gridMax.x + 1);
            y = Random.Range(gridMin.y, gridMax.y + 1);
            attempt++;

            if(SnakeMovement.Instance == null || !SnakeMovement.Instance.IsOccupying(x, y))
            {
                break;
            }
        }
        while(attempt < MaxAttempts);
        transform.position = new Vector3(x,y,0);
    }
}

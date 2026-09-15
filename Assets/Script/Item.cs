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
        int x = Random.Range(gridMin.x, gridMax.x + 1);
        int y = Random.Range(gridMin.y, gridMax.y + 1);
        transform.position = new Vector3(x, y, 0f);
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour
{
    [Header("Spawn Bounds")]
    [SerializeField] protected Vector2Int phase1Min = new Vector2Int(-4, -4);
    [SerializeField] protected Vector2Int phase1Max = new Vector2Int(4, 4);
    [SerializeField] protected Vector2Int phase2Min = new Vector2Int(-18, -10);
    [SerializeField] protected Vector2Int phase2Max = new Vector2Int(18, 10);

    [Header("Score")]
    [SerializeField] protected int scoreValue = 1;
    public int ScoreValue =>scoreValue;

    protected Vector2Int currentMin;
    protected Vector2Int currentMax;

    protected virtual void Awake()
    {
        currentMin = phase1Min;
        currentMax = phase1Max;
    }

    protected virtual void OnEnable()
    {
        SnakeMovement.OnSnakeReset += HandleReset;
        PhaseManager.OnPhase2Started += HandlePhase2;

        if (PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == 2)
        {
            HandlePhase2();
        }
    }

    protected virtual void OnDisable()
    {
        SnakeMovement.OnSnakeReset -= HandleReset;
        PhaseManager.OnPhase2Started -= HandlePhase2;
    }

    private void HandleReset()
    {
        currentMin = phase1Min;
        currentMax = phase1Max;
        Respawn();
    }
    private void HandlePhase2()
    {
        currentMin = phase2Min;
        currentMax = phase2Max;
    }

    protected virtual void Start()
    {
        Respawn();
    }

    public virtual void Collect(SnakeMovement snake)
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.RegisterItemCollected(this);
        }
    }
    public virtual void Respawn()
    {
        int MaxAttempts = 100;
        int attempt = 0;
        int x, y;
        do
        {
            x = Random.Range(currentMin.x, currentMax.x + 1);
            y = Random.Range(currentMin.y, currentMax.y + 1);
            attempt++;

            if (SnakeMovement.Instance == null || !SnakeMovement.Instance.IsOccupying(x, y))
            {
                break;
            }
        }
        while (attempt < MaxAttempts);
        transform.position = new Vector3(x, y, 0);
    }


    public void SetBounds(Vector2Int min, Vector2Int max)
    {
        currentMin = min;
        currentMax = max;
    }
}

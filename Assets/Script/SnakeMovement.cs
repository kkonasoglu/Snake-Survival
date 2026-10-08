using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeInput))]
public class SnakeMovement : MonoBehaviour
{
    private readonly Queue<Vector2> inputQueue = new Queue<Vector2>();
    private const int MaxQueueSize = 2;
    public static SnakeMovement Instance { get; private set; }
    public static event Action OnSnakeReset;

    [Header("Speed Settings")]
    [SerializeField] private float initialStepTime = 0.14f;
    [SerializeField] private float minStepTime = 0.05f;
    [SerializeField] private float speedUpFactor = 0.003f;

    [Header("Prefabs")]
    [SerializeField] private Transform bodyPrefab;

    private float currentStepTime;
    private float stepTimer;
    private SnakeInput snakeInput;
    private Vector2 currentDirection = Vector2.right;
    private readonly List<Transform> segments = new List<Transform>();
    public IReadOnlyList<Transform> Segments => segments;
    public Vector2 CurrentDirection => currentDirection;

    private void Awake()
    {
        Instance = this;
        snakeInput = GetComponent<SnakeInput>();
    }

    private void Start()
    {
        ResetState();
    }

    public void Update()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer >= currentStepTime)
        {
            stepTimer -= currentStepTime;
            Step();
        }
    }

    public void Step()
    {

        if(inputQueue.Count > 0)
        {
            currentDirection = inputQueue.Dequeue();
        }
        

        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        transform.position = new Vector3(
            Mathf.Round(transform.position.x) + currentDirection.x,
            Mathf.Round(transform.position.y) + currentDirection.y,
            0f
        );
    }

    public void Grow()
    {
        Transform segment = Instantiate(bodyPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void Shrink()
    {
        if (segments.Count > 2)
        {
            Transform lastSegment = segments[segments.Count - 1];
            segments.RemoveAt(segments.Count - 1);
            Destroy(lastSegment.gameObject);
        }
    }

    public void Accelerate()
    {
        currentStepTime = Mathf.Max(minStepTime, currentStepTime - speedUpFactor);
    }
    public void ResetState()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            if (segments[i] != null)
            {
                Destroy(segments[i].gameObject);
            }
            
        }
        inputQueue.Clear();
        segments.Clear();
        segments.Add(transform);
        transform.position = Vector3.zero;
        currentDirection = Vector2.right;
        snakeInput.ResetDirection();
        currentStepTime = initialStepTime;
        stepTimer = 0f;


        for (int i = 1; i <= 2; i++)
        {
            Transform segment = Instantiate(bodyPrefab);
            segment.position = new Vector3(-i, 0, 0);
            segments.Add(segment);
        }
        OnSnakeReset?.Invoke();
    }

    public bool IsOccupying(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (segment == null) continue;
            if (Mathf.RoundToInt(segment.position.x) == x && Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddDirectionInput(Vector2 newDir)
    {
        if (inputQueue.Count >= MaxQueueSize) return;

        Vector2 lastDir = inputQueue.Count > 0 ? inputQueue.ToArray()[inputQueue.Count - 1] : currentDirection;
        if (newDir != -lastDir && newDir != lastDir)
        {
            inputQueue.Enqueue(newDir);
        }
    }

    public void ChangeDirection(DirectionChangeMode mode = DirectionChangeMode.PerpendicularTurn)
    {
        if (mode == DirectionChangeMode.Reverse180)
        {
            ReverseSnake();
        }
        else
        {
            TurnPerpendicular();
        }
    }

    public void TurnPerpendicular()
    {
        Vector2 dirA;
        Vector2 dirB;

        if (currentDirection.x != 0)
        {
            dirA = Vector2.up;
            dirB = Vector2.down;
        }
        else
        {
            dirA = Vector2.left;
            dirB = Vector2.right;
        }

        Vector3 headPos = transform.position;
        bool canMoveA = !IsOccupying(Mathf.RoundToInt(headPos.x + dirA.x), Mathf.RoundToInt(headPos.y + dirA.y));
        bool canMoveB = !IsOccupying(Mathf.RoundToInt(headPos.x + dirB.x), Mathf.RoundToInt(headPos.y + dirB.y));

        Vector2 chosenDir;
        if (canMoveA && !canMoveB)
        {
            chosenDir = dirA;
        }
        else if (!canMoveA && canMoveB)
        {
            chosenDir = dirB;
        }
        else
        {
            chosenDir = UnityEngine.Random.value > 0.5f ? dirA : dirB;
        }

        currentDirection = chosenDir;
        inputQueue.Clear();
    }

    public void ReverseSnake()
    {
        if (segments.Count <= 1)
        {
            currentDirection = -currentDirection;
            inputQueue.Clear();
            return;
        }

        List<Vector3> positions = new List<Vector3>(segments.Count);
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null)
            {
                positions.Add(segments[i].position);
            }
        }

        positions.Reverse();
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null && i < positions.Count)
            {
                segments[i].position = positions[i];
            }
        }

        Vector2 diff = (Vector2)(segments[0].position - segments[1].position);
        Vector2 newDir = Vector2.zero;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            newDir = diff.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (Mathf.Abs(diff.y) > 0)
        {
            newDir = diff.y > 0 ? Vector2.up : Vector2.down;
        }
        else
        {
            newDir = -currentDirection;
        }

        currentDirection = newDir;
        inputQueue.Clear();
        Physics2D.SyncTransforms();
    }
}

public enum DirectionChangeMode
{
    PerpendicularTurn,
    Reverse180
}

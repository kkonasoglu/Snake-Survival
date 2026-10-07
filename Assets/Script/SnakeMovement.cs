using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeInput))]
public class SnakeMovement : MonoBehaviour
{
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
            stepTimer = 0f;
            Step();
        }
    }

    public void Step()
    {
        Vector2 targetDir = snakeInput.CurrentInputDirection;
        if (targetDir != -currentDirection)
        {
            currentDirection = targetDir;
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
}

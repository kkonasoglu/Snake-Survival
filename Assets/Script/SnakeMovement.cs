using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(SnakeInput))]
public class SnakeMovement : MonoBehaviour
{
    public static event Action OnSnakeReset;
    [SerializeField] private float stepTime = 0.12f;
    [SerializeField] private Transform bodyPrefab;

    private SnakeInput snakeInput;
    private Vector2 currentDirection = Vector2.right;
    private readonly List<Transform> segments = new List<Transform>();

    public IReadOnlyList<Transform> Segments => segments;
    public Vector2 CurrentDirection => currentDirection;

    private void Awake()
    {
        snakeInput = GetComponent<SnakeInput>();
    }

    private void Start()
    {
        ResetState();
        InvokeRepeating(nameof(Step), stepTime, stepTime);

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
        for (int i = 1; i <= 2; i++)
        {
            Transform segment = Instantiate(bodyPrefab);
            segment.position = new Vector3(-i, 0, 0);
            segments.Add(segment);
        }
        OnSnakeReset?.Invoke();
    }
}

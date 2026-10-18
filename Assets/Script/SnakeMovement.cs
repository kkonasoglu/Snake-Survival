using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Rendering.RenderGraphModule;

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

    [Header("Boundary Limits (Wall Collision)")]
    [SerializeField] private Vector2Int phase1Min = new Vector2Int(-9, -5);
    [SerializeField] private Vector2Int phase1Max = new Vector2Int(11, 6);
    [SerializeField] private Vector2Int phase2Min = new Vector2Int(-12, -7);
    [SerializeField] private Vector2Int phase2Max = new Vector2Int(16, 9);
    private Vector2Int currentMin;
    private Vector2Int currentMax;

    [Header("Infinite Mode(Phase 3)")]
    [SerializeField] private Vector2Int infiniteMin = new Vector2Int(-14, -9);
    [SerializeField] private Vector2Int infiniteMax = new Vector2Int(16, 9);


    [Header("Reverse Feedback (Banana Delay & Pop)")]
    [SerializeField] private float reversePauseDuration = 0.35f;
    [SerializeField] private float popScaleMultiplier = 1.35f;
    [SerializeField] private Color popColor = Color.yellow;



    [Header("Devil fruit (Stretch & Shrink)")]
    [SerializeField] private float devilFruitDuration = 5f;
    [SerializeField] private float shrinkInterval = 0.08f;
    private Coroutine devilFruitRoutine;
    private bool isPaused = false;
    private SpriteRenderer headRenderer ; 
    private Vector3 baseHeadScale;
    private Color baseHeadColor = Color.white;


    private bool isInfiniteMode = false;
    private int pendingGrowth = 0;

    private float currentStepTime;
    private float stepTimer;
    private SnakeInput snakeInput;
    private Vector2 currentDirection = Vector2.right;
    private readonly List<Transform> segments = new List<Transform>();
    public IReadOnlyList<Transform> Segments => segments;
    public Vector2 CurrentDirection => currentDirection;

    private void OnEnable()
    {
        PhaseManager.OnPhase2Started += HandlePhase2Started;
        PhaseManager.OnPhase3Started += EnableInfiniteMode;
    }

    private void OnDisable()
    {
        PhaseManager.OnPhase2Started -= HandlePhase2Started;
        PhaseManager.OnPhase3Started -= EnableInfiniteMode;
    }

    private void HandlePhase2Started()
    {
        currentMin = phase2Min;
        currentMax = phase2Max;
        Debug.Log("<color=green>[SnakeMovement] FAZ 2 SINIRLARI AKTİF! Alan genişledi.</color>");
    }

    private void EnableInfiniteMode()
    {
        isInfiniteMode = true;
        Debug.Log("<color=magenta>[SnakeMovement] SONSUZ MOD AKTİF! Kenarlardan geçiş açıldı.</color>");
    }

    private void Awake()
    {
        Instance = this;
        snakeInput = GetComponent<SnakeInput>();

        currentMin = phase1Min;
        currentMax = phase1Max;

        headRenderer = GetComponent<SpriteRenderer>();
        baseHeadScale =  transform.localScale;
        if(baseHeadScale == Vector3.zero) baseHeadScale = Vector3.one;
        if(headRenderer !=null) baseHeadColor = headRenderer.color;
    }

    private void Start()
    {
        ResetState();
    }

    public void Update()
    {
        if(isPaused) return;

        stepTimer += Time.deltaTime;
        if (stepTimer >= currentStepTime)
        {
            stepTimer -= currentStepTime;
            Step();
        }
    }

    public void Step()
    {

        if (inputQueue.Count > 0)
        {
            currentDirection = inputQueue.Dequeue();
        }

        float nextX = Mathf.Round(transform.position.x) + currentDirection.x;
        float nextY = Mathf.Round(transform.position.y) + currentDirection.y;

        // Duvar çarpışma kontrolü (Faz 1 & 2) veya Sonsuz Mod geçişi (Faz 3)
        if (isInfiniteMode)
        {
            if (nextX > infiniteMax.x) nextX = infiniteMin.x;
            else if (nextX < infiniteMin.x) nextX = infiniteMax.x;
            if (nextY > infiniteMax.y) nextY = infiniteMin.y;
            else if (nextY < infiniteMin.y) nextY = infiniteMax.y;
        }
        else
        {
            if (nextX < currentMin.x || nextX > currentMax.x || nextY < currentMin.y || nextY > currentMax.y)
            {
                Debug.Log($"<color=red>[SnakeMovement] Duvara çarpıldı! Hedef: ({nextX}, {nextY}) | Sınırlar: X:[{currentMin.x}, {currentMax.x}] Y:[{currentMin.y}, {currentMax.y}]</color>");
                ResetState();
                return;
            }
        }

        // Kuyruğun hareket etmeden önceki eski pozisyon ve açısını kaydet
        Vector3 oldTailPos = segments[segments.Count - 1].position;
        Quaternion oldTailRot = segments[segments.Count - 1].rotation;

        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        transform.position = new Vector3(nextX, nextY, 0f);

        // Büyüme bekliyorsa, kuyruğun az önce terk ettiği boş yere yeni parçayı ekle
        if (pendingGrowth > 0)
        {
            pendingGrowth--;
            Transform newSegment = Instantiate(bodyPrefab);
            newSegment.position = oldTailPos;
            newSegment.rotation = oldTailRot;
            segments.Add(newSegment);
        }
    }

    public void Grow()
    {
        pendingGrowth++;
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
        StopAllCoroutines();
        isPaused = false;
        transform.localScale = baseHeadScale;
        if(headRenderer != null )  headRenderer.color = baseHeadColor; 
        for (int i = 1; i < segments.Count; i++)
        {
            if (segments[i] != null)
            {
                Destroy(segments[i].gameObject);
            }

        }
        inputQueue.Clear();
        isInfiniteMode = false;
        currentMin = phase1Min;
        currentMax = phase1Max;
        pendingGrowth = 0;
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

    public bool IsInsideBounds(int x, int y)
    {
        if (isInfiniteMode) return true;
        return x >= currentMin.x && x <= currentMax.x && y >= currentMin.y && y <= currentMax.y;
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
        bool canMoveA = IsInsideBounds(Mathf.RoundToInt(headPos.x + dirA.x), Mathf.RoundToInt(headPos.y + dirA.y)) &&
                        !IsOccupying(Mathf.RoundToInt(headPos.x + dirA.x), Mathf.RoundToInt(headPos.y + dirA.y));
        bool canMoveB = IsInsideBounds(Mathf.RoundToInt(headPos.x + dirB.x), Mathf.RoundToInt(headPos.y + dirB.y)) &&
                        !IsOccupying(Mathf.RoundToInt(headPos.x + dirB.x), Mathf.RoundToInt(headPos.y + dirB.y));

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

        StartCoroutine(ReverseFeedBackRoutine(reversePauseDuration));
    }

    private IEnumerator ReverseFeedBackRoutine(float duration)
    {
        isPaused = true;
        Vector3 poppedScale = baseHeadScale * popScaleMultiplier;
        float elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.localScale = Vector3.Lerp(poppedScale,baseHeadScale,t);

            if(headRenderer != null)
            {
                headRenderer.color = Color.Lerp(popColor,baseHeadColor,t);
            }

            yield return null ; 
        }

        transform.localScale = baseHeadScale;
        if(headRenderer != null) headRenderer.color = baseHeadColor;
        isPaused = false;
    }

    public void ActivateDevilFruit()
    {
        if(devilFruitRoutine != null)
        {
            StopCoroutine(devilFruitRoutine);
        }
        devilFruitRoutine = StartCoroutine(DevilFruitRoutine());
    }

    private IEnumerator DevilFruitRoutine()
    {
        float timer = 0;
        int extraSegments = 0;

        while(timer < devilFruitDuration)
        {
            Grow();
            extraSegments++;
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }
        

        while(extraSegments > 0 && segments.Count > 3)
        {
            Shrink();
            extraSegments--;
            yield return new WaitForSeconds(shrinkInterval);
        }
        devilFruitRoutine = null ;

    }
}

public enum DirectionChangeMode
{
    PerpendicularTurn,
    Reverse180
}

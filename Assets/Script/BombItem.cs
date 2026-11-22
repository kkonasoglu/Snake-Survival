using UnityEngine;
using System.Collections;

public class BombItem : Item
{
    [Header("Bomb Fuse & lifeTime")]
    [SerializeField] private float fuseTime = 5f;
    [SerializeField] private float respawnDelay = 1.5f;

    [Header("Visual Effect (Pulse & Flash)")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.12f;
    [SerializeField] private float flashSpeed = 4f;
    [SerializeField] private Color flashColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Collider2D bombCollider;
    private Vector3 initialScale;
    private Color initialColor;
    private float currentTimer = 0f;
    private float pulsePhase = 0f;
    private float flashPhase = 0f;
    private bool isExplode = false;

    protected override void Awake()
    {
        base.Awake();
        scoreValue = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        bombCollider = GetComponent<Collider2D>();

        initialScale = transform.localScale;
        if (initialScale == Vector3.zero) initialScale = Vector3.zero;
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetBombState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetBombState();
    }

    private void Update()
    {
        if (isExplode) return;
        currentTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(currentTimer / fuseTime);
        float speedMultiplier = 1f + (progress * 1.5f);

        pulsePhase += Time.deltaTime * pulseSpeed * speedMultiplier;
        float scaleOffset = Mathf.Sin(pulsePhase) * pulseAmount;
        transform.localScale = initialScale * (1f + scaleOffset);

        if (spriteRenderer != null)
        {
            flashPhase += Time.deltaTime * flashSpeed * speedMultiplier;
            float flash = Mathf.PingPong(flashPhase, 1f);
            spriteRenderer.color = Color.Lerp(initialColor, flashColor, flash);
        }

        if (currentTimer >= fuseTime)
        {
             StartCoroutine(ExplodeAndRespawnRoutine());
        }
    }

    private IEnumerator ExplodeAndRespawnRoutine()
    {
        isExplode = true;
        AudioManager.Instance?.PlayBomb();
        CameraExpender.Instance?.TriggerShake(0.3f, 0.3f);

        if(spriteRenderer != null) spriteRenderer.enabled = false;
        if(bombCollider !=null) bombCollider.enabled = false;
        
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
        ResetBombState();
    }

    private void ResetBombState()
    {
        StopAllCoroutines();
        isExplode = false;
        currentTimer = 0f;
        pulsePhase = 0f;
        flashPhase = 0f;
        transform.localScale = initialScale;

        if(spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
            spriteRenderer.enabled = true;
        }

        if(bombCollider != null)
        {
            bombCollider.enabled = true ; 
        }
    }

    public override void Collect(SnakeMovement snake)
    {
        if(isExplode) return;
        ParticleManager.Instance?.PlayEatEffect(transform.position,Color.gray);

        AudioManager.Instance?.PlayBomb();
        snake.Die();
        ResetBombState();
        Respawn();
    }


}

using UnityEngine;
using System.Collections;
using UnityEditor.ShaderGraph;

public class BombItem : Item
{
    [Header("Bomb Fuse & lifeTime")]
    [SerializeField] private float fuseTime = 5f;
    [SerializeField] private float respawnDelay = 1.5f;

    [Header("Visual Effect (Pulse & Flash)")]
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float pulseAmount = 0.25f;
    [SerializeField] private float flashSpeed = 8f;
    [SerializeField] private Color flashColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Collider2D bombCollider;
    private Vector3 initialScale;
    private Color initialColor;
    private float currentTimer = 0f;
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
        float speedMutliplier = 1f + (progress * 1.5f);

        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed * speedMutliplier) * pulseAmount;
        transform.localScale = initialScale * (1f + scaleOffset);

        if(spriteRenderer != null)
        {
            float flash = Mathf.PingPong(Time.time * flashSpeed * speedMutliplier , 1f );
            spriteRenderer.color = Color.Lerp(initialColor , flashColor , flash);
        }

        if(currentTimer >= fuseTime)
        {
             StartCoroutine(ExplodeAndRespawnRoutine());
        }
    }

    private IEnumerator ExplodeAndRespawnRoutine()
    {
        isExplode = true;
        Debug.Log("<color = orange>[BombItem] bomba süresi doldu,patladı ve yok oldu! </color>");

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
        Debug.Log("<color = red><b>[BombItem] BOOM! yılan bombaya çarptı ve öldü!</b></color>");

        snake.ResetState();
        ResetBombState();
        Respawn();
    }


}

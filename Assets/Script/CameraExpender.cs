using UnityEngine;
using System.Collections;

public class CameraExpender : MonoBehaviour
{
    public static CameraExpender Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float initialOrthoSize = 7.5f;
    [SerializeField] private float phase2OrthoSize  = 12f;
    [SerializeField] private float zoomDuration = 1.2f;

    [Header("Screen Shake Settings")]
    [SerializeField] private float defaultShakeDuration = 0.22f;
    [SerializeField] private float defaultShakeMagnitude = 0.22f;

    private Coroutine zoomCoroutine;
    private Coroutine shakeCoroutine;
    private Vector3 basePosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        basePosition = transform.position;
    }

    private void OnEnable()
    {
        PhaseManager.OnPhase2Started += ExpandView;
        SnakeMovement.OnSnakeReset += ResetCamera;
        SnakeMovement.OnSnakeDied += HandleSnakeDied;
    }

    private void OnDisable()
    {
        PhaseManager.OnPhase2Started -= ExpandView;
        SnakeMovement.OnSnakeReset -= ResetCamera;
        SnakeMovement.OnSnakeDied -= HandleSnakeDied;
    }

    private void ExpandView()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothZoom(phase2OrthoSize));
    }

    private void ResetCamera()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        transform.position = basePosition;

        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        if (mainCam != null)
        {
            mainCam.orthographicSize = initialOrthoSize;
        }  
    }

    private IEnumerator SmoothZoom(float targetSize)
    {
        float startSize = mainCam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            mainCam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomDuration);
            yield return null;
        }

        mainCam.orthographicSize = targetSize;
        zoomCoroutine = null;
    }

    // --- SCREEN SHAKE (SARSINTI) FONKSİYONLARI ---
    private void HandleSnakeDied()
    {
        TriggerShake(defaultShakeDuration, defaultShakeMagnitude);
    }

    public void TriggerShake(float duration = 0.22f, float magnitude = 0.22f)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = 1f - (elapsed / duration); // Zamanla azalan sarsıntı (Decay)
            Vector2 randomOffset = Random.insideUnitCircle * (magnitude * percent);

            transform.position = new Vector3(basePosition.x + randomOffset.x, basePosition.y + randomOffset.y, basePosition.z);
            yield return null;
        }

        transform.position = basePosition;
        shakeCoroutine = null;
    }
}

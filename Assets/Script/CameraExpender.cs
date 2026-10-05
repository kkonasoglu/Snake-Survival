using UnityEngine;
using System.Collections;

public class CameraExpender : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float initialOrthoSize = 7.5f;
    [SerializeField] private float phase2OrthoSize  = 12f;
    [SerializeField] private float zoomDuration = 1.2f;

    [Header("New Item Spawn Bounds for Phase 2")]
    [SerializeField] private Vector2Int newGridMin = new Vector2Int(-18,-10);
    [SerializeField] private Vector2Int newGridMax = new Vector2Int(18,10);
    [SerializeField] private Item currentItem;

    [Header("Phase 1 Item Spawn Bound")]
    [SerializeField] private Vector2Int initialGridMin = new Vector2Int(-4,-4);
    [SerializeField] private Vector2Int initialGridMax = new Vector2Int(4,4);

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    private void OnEnable()
    {
        PhaseManager.OnPhase2Started += ExpandView;
        SnakeMovement.OnSnakeReset += ResetCamera;
    }

    private void OnDisable()
    {
        PhaseManager.OnPhase2Started -= ExpandView;
        SnakeMovement.OnSnakeReset -= ResetCamera;
    }


    private void ExpandView()
    {
        if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothZoom(phase2OrthoSize));

        UpdateItemSpawnBounds(newGridMin, newGridMax);
    }

    private void ResetCamera()
    {
        if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        if(mainCam != null)
        {
            mainCam.orthographicSize = initialOrthoSize;
        }

        UpdateItemSpawnBounds(initialGridMin,initialGridMax);
    }

    private void UpdateItemSpawnBounds(Vector2Int min,Vector2Int max)
    {
        if(currentItem != null)
        {
            currentItem.SetBounds(min,max);
        }
    }

    private IEnumerator SmoothZoom(float targetSize)
    {
        float startSize = mainCam.orthographicSize;
        float elapsed = 0f;

        while(elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            mainCam.orthographicSize = Mathf.Lerp(startSize,targetSize,elapsed / zoomDuration);
            yield return null ;
        }

        mainCam.orthographicSize = targetSize;
        zoomCoroutine  = null;
    }
}

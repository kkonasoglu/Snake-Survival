using UnityEngine;
using System.Collections;

public class CameraExpender : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float initialOrthoSize = 7.5f;
    [SerializeField] private float phase2OrthoSize  = 12f;
    [SerializeField] private float zoomDuration = 1.2f;
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

       
    }

    private void ResetCamera()
    {
        if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        if(mainCam != null)
        {
            mainCam.orthographicSize = initialOrthoSize;
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

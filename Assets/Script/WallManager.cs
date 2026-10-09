using Unity.VisualScripting;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [Header("Wall to destroy in phase 2")]
    [SerializeField] private GameObject innerWall;

    [Header("Wall to destroy in phase3")]
    [SerializeField] private GameObject outWall;

    private void Awake()
    {
        if(outWall == null)
        {
            outWall = GameObject.Find("out border");
        }
    }

    private void OnEnable()
    {
        PhaseManager.OnPhase2Started += HandlePhase2Started;
        SnakeMovement.OnSnakeReset += RestoreWalls;
    }

    private void OnDisable()
    {
        PhaseManager.OnPhase2Started -= HandlePhase2Started;
        SnakeMovement.OnSnakeReset -= RestoreWalls;
    }

    private void HandlePhase2Started()
    {
        if(innerWall != null)
        {
            innerWall.SetActive(false);
            Debug.Log("[WallManager] ilk duvar yıkıldı alan genişledi");
        }
    }

    private void RestoreWalls()
    {
        if(innerWall != null)
        {
            innerWall.SetActive(true);
            Debug.Log("[WallManager] duvarlar geri yüklendi");
        }
    }
}

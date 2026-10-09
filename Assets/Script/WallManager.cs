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
            outWall = GameObject.Find("out Border");
        }
    }

    private void OnEnable()
    {
        PhaseManager.OnPhase2Started += HandlePhase2Started;
        PhaseManager.OnPhase3Started += HandlePhase3Started;
        SnakeMovement.OnSnakeReset += RestoreWalls;
    }

    private void OnDisable()
    {
        PhaseManager.OnPhase2Started -= HandlePhase2Started;
        PhaseManager.OnPhase3Started -= HandlePhase3Started;
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

    private void HandlePhase3Started()
    {
        if(outWall != null)
        {
            outWall.SetActive(false);
            Debug.Log("<color = orange>[WallManager] SON DUVAR YIKILDI! sonsuz alan açıldı</color>");
        }
    }

    private void RestoreWalls()
    {
        if(innerWall != null)
        {
            innerWall.SetActive(true);
           
        }

        if(outWall != null)
        {
            outWall.SetActive(true);
            
        }
        Debug.Log("[WallManager] duvarlar geri yüklendi");
    }
}

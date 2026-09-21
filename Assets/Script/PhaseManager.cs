using UnityEngine;
using System;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance {get; private set;}
    public static event Action OnPhase2Started;

    [Header("Phase goal")]
    [SerializeField] private int pahse1Target = 20;

    private int collectedApples = 0;
    private int currentPhase =1;

    public int CollectedApples =>collectedApples;
    public int CurrentPhase => currentPhase;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SnakeMovement.OnSnakeReset += ResetProgress;
    }
    private void OnDisable()
    {
        SnakeMovement.OnSnakeReset -= ResetProgress;
    }

    public void RegisterItemCollected(Item item)
    {
        if(currentPhase == 1)
        {
            collectedApples++;
            Debug.Log($"[PhaseManager] elma toplandı : {CollectedApples} / {pahse1Target}");

            if(collectedApples >= pahse1Target)
            {
                StartPhase2();
            }
        }
    }

     private void StartPhase2()
    {
        currentPhase = 2;
       Debug.Log("<color=green><b>[PhaseManager] TEBRİKLER! FAZ 2 BAŞLADI! Duvarlar yıkılıyor...</b></color>");
       OnPhase2Started?.Invoke();     
    }

    private void ResetProgress()
    {
        collectedApples = 0;
        currentPhase = 1;
        Debug.Log("[PhaseManager] Yılan öldü, faz ilerlemesi sıfırlandı.");
    }

}

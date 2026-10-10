using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Assemblies;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance { get; private set; }
    public static event Action OnPhase2Started;
    public static event Action OnPhase3Started;

    [Header("Phase goal (Score)")]
    [SerializeField] private int phase1Target = 20;
    [SerializeField] private int phase3Target = 50;

    [Header("Phase 2 Items")]
    [SerializeField] private GameObject bananaObject;

    [Header("Phase 3 Items")]
    [SerializeField] private GameObject bombObject;

    public int currentScore = 0;
    public int currentPhase = 1;

    public int CurrentScore => currentScore;
    public int CurrentPhase => currentPhase;
    public int CollectedApples =>currentScore; //it is for if have a old references

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (bananaObject == null)
        {
            BananaItem banana = FindAnyObjectByType<BananaItem>(FindObjectsInactive.Include);
            if (banana != null)
            {
                bananaObject = banana.gameObject;
            }
        }

        if(bombObject == null)
        {
            BombItem bomb = FindAnyObjectByType<BombItem>(FindObjectsInactive.Include);
            if(bananaObject != null)
            {
                bombObject = bomb.gameObject;
            }
        }
    }

    private void Start()
    {
        if (bananaObject != null)
        {
            bananaObject.SetActive(false);
        }

        if(bombObject != null)
        {
            bananaObject.SetActive(false);
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
        int points = item != null ? item.ScoreValue : 1;
        currentScore += points;
        Debug.Log($"<color = cyan>[PhaseManager] + [points] Puan! Toplam Skor = {currentScore}</color>");

        if(currentPhase == 1&& currentScore >= phase1Target)
        {
            StartPhase2();
        }

        else if(currentPhase == 2 && currentScore >= phase3Target)
        {
            StartPhase3();
        }
    }

    private void StartPhase2()
    {
        currentPhase = 2;
        Debug.Log("<color=green><b>[PhaseManager] TEBRİKLER! FAZ 2 BAŞLADI! Duvarlar yıkılıyor...</b></color>");
        OnPhase2Started?.Invoke();

        if (bananaObject != null)
        {
            bananaObject.SetActive(true);
            if (bananaObject.TryGetComponent<Item>(out var item))
            {
                item.Respawn();
            }
        }
    }

    private void StartPhase3()
    {
        currentPhase = 3;
        Debug.Log("<color=yellow><b>[PhaseManager] TEBRİKLER! FAZ 3 BAŞLADI (50 Puan)! Sonsuz Mod devrede...</b></color>");
        OnPhase3Started?.Invoke();
        if(bombObject != null)
        {
            bombObject.SetActive(true);
            if(bombObject.TryGetComponent<Item>(out var item))
            {
                item.Respawn();
            }
        }
    }

    private void ResetProgress()
    {
        currentScore = 0;
        currentPhase = 1;
        Debug.Log("[PhaseManager] Yılan öldü, faz ilerlemesi sıfırlandı.");

        if (bananaObject != null)
        {
            bananaObject.SetActive(false);
        }

        if(bombObject != null)
        {
            bombObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

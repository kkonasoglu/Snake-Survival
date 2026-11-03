using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("HUD (Canlı Değerler)")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI phaseText;
    [Header("Faz Geçiş Banner'ı")]
    [SerializeField] private GameObject phaseBannerPanel;
    [SerializeField] private TextMeshProUGUI phaseBannerText;
    [SerializeField] private Image phaseBannerImage; 
    [SerializeField] private float bannerDuration = 2.5f;
    [Header("Game Over Ekranı")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private GameObject newRecordBadge; 
    private int highScore = 0;
    private Coroutine bannerCoroutine;
    private void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    private void Start()
    {
        UpdateHighScoreDisplay();
        UpdateScoreDisplay(0);
        UpdatePhaseDisplay(1);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (phaseBannerPanel != null) phaseBannerPanel.SetActive(false);
    }
    private void OnEnable()
    {
        PhaseManager.OnScoreChanged += HandleScoreChanged;
        PhaseManager.OnPhase2Started += HandlePhase2;
        PhaseManager.OnPhase3Started += HandlePhase3;
        SnakeMovement.OnSnakeDied += HandleSnakeDied;
        SnakeMovement.OnSnakeReset += HandleSnakeReset;
    }
    private void OnDisable()
    {
        PhaseManager.OnScoreChanged -= HandleScoreChanged;
        PhaseManager.OnPhase2Started -= HandlePhase2;
        PhaseManager.OnPhase3Started -= HandlePhase3;
        SnakeMovement.OnSnakeDied -= HandleSnakeDied;
        SnakeMovement.OnSnakeReset -= HandleSnakeReset;
    }
    private void Update()
    {
        
        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnRestartButtonClicked();
            }
        }
    }
    private void HandleScoreChanged(int newScore)
    {
        UpdateScoreDisplay(newScore);
        if (newScore > highScore)
        {
            highScore = newScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
        }
    }
    private void HandlePhase2()
    {
        UpdatePhaseDisplay(2);
        ShowPhaseBanner("FAZ 2: DUVARLAR YIKILDI!");
    }
    private void HandlePhase3()
    {
        UpdatePhaseDisplay(3);
        ShowPhaseBanner("FAZ 3: SONSUZ ALAN & KAOS!");
    }
    private void HandleSnakeDied()
    {
        if (gameOverPanel == null) return;
        int finalScore = PhaseManager.Instance != null ? PhaseManager.Instance.CurrentScore : 0;
        
        if (finalScoreText != null) finalScoreText.text = "SKOR: " + finalScore;
        if (bestScoreText != null) bestScoreText.text = "EN YÜKSEK: " + highScore;
        // Yeni rekor kırıldıysa rozeti/yazıyı aç
        if (newRecordBadge != null)
        {
            newRecordBadge.SetActive(finalScore >= highScore && finalScore > 0);
        }
        gameOverPanel.SetActive(true);
    }
    private void HandleSnakeReset()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdatePhaseDisplay(1);
        UpdateScoreDisplay(0);
    }
    public void OnRestartButtonClicked()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (SnakeMovement.Instance != null)
        {
            SnakeMovement.Instance.RestartGame();
        }
    }
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null) scoreText.text = "SKOR: " + score;
    }
    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null) highScoreText.text = "REKOR: " + highScore;
    }
    private void UpdatePhaseDisplay(int phase)
    {
        if (phaseText != null) phaseText.text = "FAZ " + phase;
    }
    public void ShowPhaseBanner(string message)
    {
        if (phaseBannerPanel == null) return;
        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(PhaseBannerRoutine(message));
    }
    private IEnumerator PhaseBannerRoutine(string message)
    {
        phaseBannerPanel.SetActive(true);
        if (phaseBannerText != null) phaseBannerText.text = message;
        yield return new WaitForSeconds(bannerDuration);
        phaseBannerPanel.SetActive(false);
        bannerCoroutine = null;
    }
}
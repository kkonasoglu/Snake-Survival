using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    [SerializeField] private GameObject howToPlayPanel;

    [Header("En Yüksek Skor ")]
    [SerializeField]private TextMeshProUGUI highScoreText;

    [Header("Oyun Sahnesinin Adı")]
    [SerializeField]private string gameSceneName = "SampleScene";

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = -1;
    }

    void Start()
    {
        Time.timeScale=1f;
        if(howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        if(highScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt("HighScore",0);
            highScoreText.text = "BEST SCORE: " + bestScore;
        }  
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenHowToPlay()
    {
        if(howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if(howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    
}

using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI deltaText;
    public TextMeshProUGUI timerText;
    public GameObject endScreen;
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScore(int totalScore, int delta)
    {
        if(scoreText!=null) scoreText.text = $"Score: {totalScore}";
        if(deltaText!=null) deltaText.text = delta>=0? $"+{delta}":"-";
    }

    public void UpdateTimer(float time)
    {
        if(timerText!=null) timerText.text = $"Time: {Mathf.CeilToInt(time)}s";
    }

    public void ShowFinalScore(int finalScore)
    {
        if(endScreen!=null) endScreen.SetActive(true);
        if(finalScoreText!=null) finalScoreText.text = $"Final Score: {finalScore}";
    }
}

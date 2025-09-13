using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Managers")]
    public PatientManager patientManager;
    public PatientVisualManager visualManager;
    public PatientUI patientUI;

    [Header("Timer")]
    public float totalTime = 60f;
    private float remainingTime;
    private bool gameEnded = false;

    [Header("Score")]
    private int score = 0;
    private Patient currentPatient;
    private float patientStartTime;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text playerNameText;
    public TMP_Text finalScoreText;
    public TMP_Text rankText;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public Button backToMenuButton;

    private string playerInitials = "AAA"; // default, bisa di-set dari menu

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); // tetap ada saat pindah scene
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        remainingTime = totalTime;
        ScoreManager.Instance.UpdateScore(score, 0);
        StartCoroutine(TimerCoroutine());
        SpawnNextPatient();

        // setup tombol
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"));

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

private IEnumerator TimerCoroutine()
{
    while (remainingTime > 0f)
    {
        remainingTime -= Time.deltaTime;
        ScoreManager.Instance.UpdateTimer(remainingTime);
        yield return null;
    }

    remainingTime = 0f;
    ScoreManager.Instance.UpdateTimer(remainingTime);

    gameEnded = true; // hentikan input & spawn pasien
    EndGame();        // langsung tampilkan popup
}


private void EndGame()
{
    if (gameEnded) return; // mencegah pemanggilan ganda
    gameEnded = true;
    remainingTime = 0f;

    if (ScoreManager.Instance != null)
        ScoreManager.Instance.UpdateTimer(remainingTime);

    if (LeaderboardManager.Instance != null)
        LeaderboardManager.Instance.AddScore(playerInitials, score);

    // Tampilkan panel
    if (gameOverPanel != null)
    {
        gameOverPanel.SetActive(true);

        if (playerNameText != null)
            playerNameText.text = $"Player: {playerInitials}";

        if (finalScoreText != null)
            finalScoreText.text = $"Score: {score}";

        // Ranking
        if (LeaderboardManager.Instance != null)
        {
            var leaderboard = LeaderboardManager.Instance.GetLeaderboard();
            int rank = leaderboard.FindIndex(e => e.initials == playerInitials && e.score == score) + 1;

            if (rankText != null)
                rankText.text = rank > 0 ? $"Rank: {rank}" : "Rank: N/A";

            if (leaderboardContent != null && leaderboardEntryPrefab != null)
            {
                foreach (Transform child in leaderboardContent)
                    Destroy(child.gameObject);

                foreach (var entry in leaderboard)
                {
                    GameObject go = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                    TMP_Text text = go.GetComponent<TMP_Text>();
                    if (text != null)
                        text.text = $"{entry.initials} - {entry.score}";
                }
            }
        }
    }
}


    public void SubmitAnswer(int submittedScore)
    {
        if (currentPatient == null || gameEnded) return;

        int patientScore = currentPatient.GetTotalScore();
        int deviation = Mathf.Abs(patientScore - submittedScore);
        float timeUsed = Time.time - patientStartTime;
        int delta = 0;
        int baseScore = 400;

        if (submittedScore == patientScore)
        {
            float timeFactor = Mathf.Clamp01((30f - timeUsed) / 20f);
            int timeBonus = Mathf.RoundToInt(timeFactor * 100f);
            delta = baseScore + timeBonus;
        }
        else
        {
            delta = baseScore - 100 * deviation;
            float timeFactor = Mathf.Clamp01((30f - timeUsed) / 20f);
            int timeAdjustment = Mathf.RoundToInt(timeFactor * 100f);
            delta += timeAdjustment;
        }

        delta = Mathf.Clamp(delta, -400, 500);
        score += delta;
        ScoreManager.Instance.UpdateScore(score, delta);

        StartCoroutine(CameraShake(0.15f, 0.05f));
        if (gameEnded) return; // jangan proses pasien baru atau input
        SpawnNextPatient();
    }

    private void SpawnNextPatient()
    {
        currentPatient = patientManager.GeneratePatient();
        patientStartTime = Time.time;

        if (visualManager != null)
            visualManager.SetupPatient(currentPatient);
        if (patientUI != null)
            patientUI.ShowPatient(currentPatient);
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;

        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    public void SetPlayerInitials(string initials)
    {
        if (!string.IsNullOrEmpty(initials))
            playerInitials = initials.ToUpper();
    }

    public int GetScore() => score;
    public string GetPlayerInitials() => playerInitials;
}

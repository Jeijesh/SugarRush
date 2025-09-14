using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Referensi UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Floating Texts")]
    public TextMeshProUGUI deltaScoreText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI badText;
    public TextMeshProUGUI awfulText;

    [Header("Audio Clips")]
    public AudioClip perfectSFX;
    public AudioClip goodSFX;
    public AudioClip badSFX;
    public AudioClip awfulSFX;

    private AudioSource audioSource;

    [Header("Pengaturan Animasi")]
    public float floatDistance = 50f;
    public float floatDuration = 1f;

    private Vector3 deltaStartPos;
    private float deltaTimer = 0f;
    private bool deltaActive = false;

    private TextMeshProUGUI currentFeedback;
    private float feedbackTimer = 0f;
    private float currentFeedbackDuration = 0f;

    private bool firstFeedbackSkipped = true;

    private int totalScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();

        deltaScoreText.gameObject.SetActive(false);
        perfectText.gameObject.SetActive(false);
        goodText.gameObject.SetActive(false);
        badText.gameObject.SetActive(false);
        awfulText.gameObject.SetActive(false);

        deltaStartPos = deltaScoreText.transform.position;
    }

    private void Update()
    {
        if (deltaActive)
        {
            deltaTimer += Time.deltaTime;
            float t = Mathf.Clamp01(deltaTimer / floatDuration);
            deltaScoreText.transform.position = deltaStartPos + Vector3.up * floatDistance * t;
            deltaScoreText.alpha = 1 - t;

            if (t >= 1f)
            {
                deltaScoreText.gameObject.SetActive(false);
                deltaScoreText.transform.position = deltaStartPos;
                deltaScoreText.alpha = 1f;
                deltaActive = false;
            }
        }

        if (currentFeedback != null)
        {
            feedbackTimer += Time.deltaTime;
            if (feedbackTimer >= currentFeedbackDuration)
            {
                currentFeedback.gameObject.SetActive(false);
                currentFeedback = null;
            }
        }
    }

    public void UpdateScore(int newScore, int delta)
    {
        totalScore = newScore;

        if (scoreText != null)
            scoreText.text = $"Skor: {totalScore}";

        if (delta != 0 && deltaScoreText != null)
        {
            deltaScoreText.text = delta > 0 ? $"+{delta}" : $"{delta}";
            deltaScoreText.color = delta > 0 ? Color.green : Color.red;
            deltaScoreText.transform.position = deltaStartPos;
            deltaScoreText.alpha = 1f;
            deltaScoreText.gameObject.SetActive(true);
            deltaTimer = 0f;
            deltaActive = true;
        }

        ShowFeedback(delta);
    }

    private void ShowFeedback(int delta)
    {
        perfectText.gameObject.SetActive(false);
        goodText.gameObject.SetActive(false);
        badText.gameObject.SetActive(false);
        awfulText.gameObject.SetActive(false);

        if (firstFeedbackSkipped)
        {
            firstFeedbackSkipped = false;
            return;
        }

        AudioClip clipToPlay = null;

        if (delta > 400) { currentFeedback = perfectText; clipToPlay = perfectSFX; }
        else if (delta > 0) { currentFeedback = goodText; clipToPlay = goodSFX; }
        else if (delta >= -400) { currentFeedback = badText; clipToPlay = badSFX; }
        else { currentFeedback = awfulText; clipToPlay = awfulSFX; }

        if (currentFeedback != null)
        {
            currentFeedback.gameObject.SetActive(true);
            feedbackTimer = 0f;
            currentFeedbackDuration = clipToPlay != null ? clipToPlay.length : 0.7f;

            if (audioSource != null && clipToPlay != null)
                audioSource.PlayOneShot(clipToPlay);
        }
    }

    public void UpdateTimer(float time)
    {
        if (timerText != null)
            timerText.text = time <= 0f ? "Waktu Habis!" : $"Waktu: {Mathf.CeilToInt(time)}s";
    }

    // **Method baru**: menerima delta int dari GameManager
    public void AddGameResult(int delta)
    {
        totalScore += delta;
        UpdateScore(totalScore, delta);
    }
public void AddGameResult(bool success)
{
    int delta = success ? 50 : -50;
    AddGameResult(delta);
}

    public int GetTotalScore() => totalScore;
}

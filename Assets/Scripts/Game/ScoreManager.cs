using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
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

    [Header("Animation Settings")]
    public float floatDistance = 50f;
    public float floatDuration = 1f;

    private Vector3 deltaStartPos;
    private float deltaTimer = 0f;
    private bool deltaActive = false;

    private TextMeshProUGUI currentFeedback;
    private float feedbackTimer = 0f;
    private float currentFeedbackDuration = 0f;

    private bool firstFeedbackSkipped = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();

        // Default: semua feedback & delta mati
        deltaScoreText.gameObject.SetActive(false);
        perfectText.gameObject.SetActive(false);
        goodText.gameObject.SetActive(false);
        badText.gameObject.SetActive(false);
        awfulText.gameObject.SetActive(false);

        deltaStartPos = deltaScoreText.transform.position;
    }

    private void Update()
    {
        // Floating delta
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

        // Feedback timer berdasarkan durasi audio
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
public void UpdateScore(int totalScore, int delta)
{
    if (scoreText != null)
        scoreText.text = $"Score: {totalScore}";

    // Spawn delta hanya jika delta ≠ 0
    if (delta != 0 && deltaScoreText != null)
    {
        deltaScoreText.text = delta > 0 ? $"+{delta}" : $"{delta}";
        deltaScoreText.color = delta > 0 ? Color.green : Color.red;  // ⬅ Warna hijau/merah
        deltaScoreText.transform.position = deltaStartPos;
        deltaScoreText.alpha = 1f;
        deltaScoreText.gameObject.SetActive(true);
        deltaTimer = 0f;
        deltaActive = true;
    }

    // Spawn feedback
    ShowFeedback(delta);
}


    private void ShowFeedback(int delta)
    {
        // Nonaktifkan dulu semua
        perfectText.gameObject.SetActive(false);
        goodText.gameObject.SetActive(false);
        badText.gameObject.SetActive(false);
        awfulText.gameObject.SetActive(false);

        // Skip feedback pertama kali muncul
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

            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
                currentFeedbackDuration = clipToPlay.length;
            }
            else
            {
                currentFeedbackDuration = 0.7f; // default durasi
            }
        }
    }

    public void UpdateTimer(float time)
    {
        if (timerText != null)
            timerText.text = time <= 0f ? "Times Up!" : $"Time: {Mathf.CeilToInt(time)}s";
    }

    public void AddGameResult(bool success)
{
    int delta = success ? 50 : -50;

    // Ambil score saat ini dari UI
    int totalScore = 0;
    if (int.TryParse(scoreText.text.Replace("Score: ", ""), out int parsed))
        totalScore = parsed;

    totalScore += delta;

    // Update UI + feedback
    UpdateScore(totalScore, delta);
}

}

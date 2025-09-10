using UnityEngine;
using System.Collections;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        remainingTime = totalTime;
        ScoreManager.Instance.UpdateScore(score, 0);
        StartCoroutine(TimerCoroutine());
        SpawnNextPatient();
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            ScoreManager.Instance.UpdateTimer(remainingTime);
            yield return null;
        }
        EndGame();
    }

    private void EndGame()
    {
        gameEnded = true;
        remainingTime = 0f;
        ScoreManager.Instance.UpdateTimer(remainingTime);
    }

    public void SubmitAnswer(int submittedScore)
    {
        if (currentPatient == null || gameEnded) return;

        int patientScore = currentPatient.GetTotalScore();
        int deviation = Mathf.Abs(patientScore - submittedScore);

        float timeUsed = Time.time - patientStartTime;
        int delta = 0;

        // Dasar skor
        int baseScore = 400;

        if (submittedScore == patientScore) // jawaban benar
        {
            // bonus waktu linear: 10 detik awal = +100, 30 detik = +0
            float timeFactor = Mathf.Clamp01((30f - timeUsed) / 20f);
            int timeBonus = Mathf.RoundToInt(timeFactor * 100f);
            delta = baseScore + timeBonus;
        }
        else // jawaban salah
        {
            delta = baseScore - 75 * deviation;

            float timeFactor = Mathf.Clamp01((30f - timeUsed) / 20f);
            int timeAdjustment = Mathf.RoundToInt(timeFactor * 100f);
            delta += timeAdjustment;
        }

        delta = Mathf.Clamp(delta, -400, 500);

        score += delta;
        ScoreManager.Instance.UpdateScore(score, delta);

        // 🔥 efek shake kamera
        StartCoroutine(CameraShake(0.15f, 0.05f));

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

    public int GetScore() => score;

    // 🔥 fungsi camera shake
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
}

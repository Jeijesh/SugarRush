using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Manager")]
    public PatientManager patientManager;
    public PatientVisualManager visualManager;
    public PatientUI patientUI;

    [Header("Timer")]
    public float totalTime = 60f;
    private float remainingTime;
    private bool gameEnded = false;
    private bool overtime = false; // true saat timer habis tapi pasien terakhir belum selesai

    [Header("Skor")]
    private int skor = 0;
    private Patient currentPatient;
    private float patientStartTime;

    [Header("UI Game Over")]
    public GameObject panelGameOver;
    public TMP_Text namaPemainText;
    public TMP_Text skorAkhirText;
    public TMP_Text peringkatText;
    public Button tombolKembaliMenu;

    [Header("UI Timer")]
    public TMP_Text timerText;

    private string inisialPemain = "AAA";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        inisialPemain = PlayerPrefs.GetString("PlayerInitials", "Anonymous");
    }

    private void Start()
    {
        remainingTime = totalTime;
        skor = 0;
        gameEnded = false;
        overtime = false;

        StartCoroutine(TimerCoroutine());
        SpawnNextPatient();

        if (tombolKembaliMenu != null)
        {
            tombolKembaliMenu.onClick.RemoveAllListeners();
            tombolKembaliMenu.onClick.AddListener(() =>
            {
                StopAllCoroutines();
                Destroy(gameObject);
                SceneManager.LoadScene("MainMenu");
            });
        }

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0f && !gameEnded)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (!gameEnded)
        {
            remainingTime = 0f;
            overtime = true;
            UpdateTimerUI();
            Debug.Log("OVERTIME: Selesaikan pasien terakhir!");
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        if (!overtime)
        {
            timerText.color = Color.white;
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();
        }
        else
        {
            timerText.color = Color.red;
            timerText.text = "OVERTIME";
        }
    }

private void EndGame()
{
    gameEnded = true;

    int finalScore = ScoreManager.Instance.GetTotalScore(); // skor aktual

    // Tambah ke leaderboard
    if (LeaderboardManager.Instance != null)
    {
        LeaderboardManager.Instance.AddScore(inisialPemain, finalScore);
        LeaderboardManager.Instance.SaveLeaderboard();
    }

    if (panelGameOver != null)
    {
        panelGameOver.SetActive(true);
        if (namaPemainText != null) namaPemainText.text = $"Pemain: {inisialPemain}";
        if (skorAkhirText != null) skorAkhirText.text = $"Skor\t: {finalScore}";

        if (LeaderboardManager.Instance != null)
        {
            var leaderboard = LeaderboardManager.Instance.GetLeaderboard();
            int rank = leaderboard.FindIndex(e => e.initials == inisialPemain && e.score == finalScore) + 1;
            if (peringkatText != null)
                peringkatText.text = rank > 0 ? $"Peringkat: {rank}" : "Peringkat: N/A";
        }
    }
}


public void SubmitAnswer(int submittedScore)
{
    if (currentPatient == null || gameEnded) return;

    int patientScore = currentPatient.GetTotalScore();
    int deviasi = Mathf.Abs(patientScore - submittedScore);
    float waktuDipakai = Time.time - patientStartTime;
    int delta = 0;
    int skorDasar = 400;

    if (submittedScore == patientScore)
    {
        float faktorWaktu = Mathf.Clamp01((30f - waktuDipakai) / 20f);
        int bonusWaktu = Mathf.RoundToInt(faktorWaktu * 100f);
        delta = skorDasar + bonusWaktu;
    }
    else
    {
        delta = skorDasar - 100 * deviasi;
        float faktorWaktu = Mathf.Clamp01((30f - waktuDipakai) / 20f);
        int penyesuaianWaktu = Mathf.RoundToInt(faktorWaktu * 100f);
        delta += penyesuaianWaktu;
    }

    delta = Mathf.Clamp(delta, -400, 500);

    // Update skor internal & UI
    ScoreManager.Instance.AddGameResult(delta);

    StartCoroutine(CameraShake(0.15f, 0.05f));

    if (!gameEnded && !overtime)
        SpawnNextPatient();
    else if (overtime)
    {
        currentPatient = null;
        EndGame();
    }
}



    private void SpawnNextPatient()
    {
        if (gameEnded) return;
        if (overtime)
        {
            currentPatient = null;
            return;
        }

        currentPatient = patientManager.GeneratePatient();
        patientStartTime = Time.time;

        if (visualManager != null) visualManager.SetupPatient(currentPatient);
        if (patientUI != null) patientUI.ShowPatient(currentPatient);
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;

        Vector3 posisiAwal = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = posisiAwal + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = posisiAwal;
    }

    public void SetPlayerInitials(string initials)
    {
        if (!string.IsNullOrEmpty(initials))
        {
            inisialPemain = initials.ToUpper();
            PlayerPrefs.SetString("PlayerInitials", inisialPemain);
            PlayerPrefs.Save();
        }
    }

    public int GetScore() => skor;
    public string GetPlayerInitials() => inisialPemain;
}

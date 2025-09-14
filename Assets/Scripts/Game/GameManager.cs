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

[Header("UI Pause")]
public Button tombolPause;      // tombol kecil di pojok kanan atas
public GameObject panelPause;   // panel overlay pause
public Button tombolLanjut;     // tombol lanjut di panel pause
public Button tombolKeluar;     // tombol keluar ke menu di panel pause

private bool isPaused = false;


    [Header("Quotes Diabetes")]
public TMP_Text quotesText;


    private string inisialPemain = "Anonymous";

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
public void TogglePause()
{
    isPaused = !isPaused;
    Time.timeScale = isPaused ? 0f : 1f; // pause / resume game
    if (panelPause != null)
        panelPause.SetActive(isPaused);
}

private void Start()
{
    // Inisialisasi game
    remainingTime = totalTime;
    skor = 0;
    gameEnded = false;
    overtime = false;

    // Mulai timer dan spawn pasien pertama
    StartCoroutine(TimerCoroutine());
    SpawnNextPatient();

    // Tombol kembali ke menu (di Game Over)
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

    // Tombol pause kecil di pojok kanan atas
    if (tombolPause != null)
    {
        tombolPause.onClick.RemoveAllListeners();
        tombolPause.onClick.AddListener(TogglePause);
    }

    // Panel pause awalnya disembunyikan
    if (panelPause != null)
        panelPause.SetActive(false);

    // Tombol Lanjut di panel pause
    if (tombolLanjut != null)
    {
        tombolLanjut.onClick.RemoveAllListeners();
        tombolLanjut.onClick.AddListener(TogglePause); // Resume game
    }

    // Tombol Keluar di panel pause
    if (tombolKeluar != null)
    {
        tombolKeluar.onClick.RemoveAllListeners();
        tombolKeluar.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // pastikan game berjalan normal sebelum load menu
            SceneManager.LoadScene("MainMenu");
        });
    }

    // Panel Game Over awalnya disembunyikan
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
        timerText.text = $"Waktu: {Mathf.CeilToInt(remainingTime)}d";
    }
    else
    {
        timerText.color = Color.red;
        timerText.text = "DETIK TERAKHIR!";
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
        if (quotesText != null && diabetesQuotes.Length > 0)
{
    int randomIndex = Random.Range(0, diabetesQuotes.Length);
    quotesText.text = $"\"{diabetesQuotes[randomIndex]}\"";
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

private string[] diabetesQuotes = new string[]
{
    "Diabetes adalah sebuah perjalanan, bukan perlombaan.",
    "Langkah kecil setiap hari bisa membuat perbedaan besar dalam mengelola diabetes.",
    "Hidup sehat adalah obat terbaik untuk diabetes.",
    "Gula darahmu bukan identitasmu; jaga dirimu dengan baik.",
    "Mengelola diabetes tentang keseimbangan, bukan kesempurnaan."
};


    public int GetScore() => skor;
    public string GetPlayerInitials() => inisialPemain;
}

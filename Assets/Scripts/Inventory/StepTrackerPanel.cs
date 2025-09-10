using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityMinigame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform patient;
    public RectTransform finishLine;
    public Button toggleButton;     // gabungan start/stop
    public Button submitButton;     // reset manual
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    public float countdownTime = 3f;
    public float trackLength = 1200f;

    private Vector2 startPos;
    private bool isMoving = false;
    private bool isStopped = false;
    private float timer;
    private float moveSpeed;
    private int toggleClicks = 0;

    private Patient lastPatient;

    private void Awake()
    {
        if (patient != null)
            startPos = patient.anchoredPosition;
    }

    private void Start()
    {
        ResetMinigame();

        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggle);

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
    }

    private void Update()
    {
        // Deteksi pasien baru via PatientUI
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient current = PatientUI.Instance.currentPatient;
            if (current != lastPatient)
            {
                lastPatient = current;
                ResetMinigame(current.activity);
            }
        }

        if (!isMoving || isStopped) return;

        // Timer jalan terus (boleh negatif)
        timer -= Time.deltaTime;
        UpdateTimerUI();

        // Move patient
        patient.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0f);

        // Clamp posisi pasien
        if (patient.anchoredPosition.x >= startPos.x + trackLength)
        {
            patient.anchoredPosition = startPos + new Vector2(trackLength, 0f);
            StopMinigame();
        }
    }

    private void OnToggle()
    {
        toggleClicks++;

        if (toggleClicks == 1)
            StartMinigame(lastPatient != null ? lastPatient.activity : 1);
        else if (toggleClicks == 2)
        {
            StopMinigame();
            if (toggleButton != null) toggleButton.gameObject.SetActive(false);
            if (patient != null) patient.gameObject.SetActive(false);
        }
    }

    private void StartMinigame(int activity)
    {
        isMoving = true;
        isStopped = false;

        float baseSpeed = trackLength / countdownTime;
        moveSpeed = (activity == 1)
            ? Random.Range(baseSpeed * 1.0f, baseSpeed * 1.2f)
            : Random.Range(baseSpeed * 0.4f, baseSpeed * 0.8f);

        timer = countdownTime;

        if (feedbackText != null)
            feedbackText.text = "Minigame Started!";
    }

    private void StopMinigame()
    {
        if (!isMoving || isStopped) return;

        isStopped = true;

        float finishX = finishLine != null ? finishLine.anchoredPosition.x : startPos.x + trackLength;
        float patientX = patient != null ? patient.anchoredPosition.x : startPos.x;

        if (feedbackText != null)
        {
            if (patientX >= finishX && timer >= 0f)
                feedbackText.text = "Active!";
            else
                feedbackText.text = "Failed!";
        }

        isMoving = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {timer:F1}s";
    }

    // Reset minigame, patient muncul, tombol toggle muncul
    private void ResetMinigame(int activity = 1)
    {
        if (patient != null)
        {
            patient.anchoredPosition = startPos;
            patient.gameObject.SetActive(true);
        }

        if (toggleButton != null)
            toggleButton.gameObject.SetActive(true);

        timer = countdownTime;
        isMoving = false;
        isStopped = false;
        toggleClicks = 0;

        if (feedbackText != null)
            feedbackText.text = "Press Start to play!";

        UpdateTimerUI();
    }

    private void OnSubmit()
    {
        ResetMinigame(lastPatient != null ? lastPatient.activity : 1);
    }
}

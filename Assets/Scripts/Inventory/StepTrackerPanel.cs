using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ActivityMinigame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform patient;   // container patient (UI)
    public Image patientImage;      // UI Image patient
    public RectTransform finishLine;
    public Button toggleButton;     // gabungan start/stop
    public TextMeshProUGUI toggleButtonText; // teks pada toggle button
    public Button submitButton;     // reset manual
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    public float countdownTime = 3f;
    public float trackLength = 1200f;

    [Header("Animation")]
    public List<Sprite> runSprites; // array/list animasi lari
    public float frameRate = 0.1f;  // durasi per frame
    public Sprite idleSprite;       // sprite idle (opsional)

    private Vector2 startPos;
    private bool isMoving = false;
    private bool isStopped = false;
    private float timer;
    private float moveSpeed;
    private int toggleClicks = 0;

    private float animTimer = 0f;
    private int animIndex = 0;

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
    // 🔹 Deteksi pasien baru via PatientUI
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

    // Timer berjalan terus (bisa ke minus)
    timer -= Time.deltaTime;
    UpdateTimerUI();

    // Gerak patient
    patient.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0f);

    // 🔹 Animasi sprite berjalan
    AnimateRun();

    // 🔹 Kalau pasien melewati trackLength → langsung gagal
    if (patient.anchoredPosition.x >= startPos.x + trackLength)
    {
        patient.anchoredPosition = startPos + new Vector2(trackLength, 0f);
        ForceFail();
    }
}

    private void AnimateRun()
    {
        if (patientImage == null || runSprites.Count == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer = 0f;
            animIndex = (animIndex + 1) % runSprites.Count;
            patientImage.sprite = runSprites[animIndex];
        }
    }

    private void OnToggle()
    {
        toggleClicks++;

        if (toggleClicks == 1)
        {
            StartMinigame(lastPatient != null ? lastPatient.activity : 1);
            if (toggleButtonText != null) toggleButtonText.text = "Stop";
        }
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
            : Random.Range(baseSpeed * 0.4f, baseSpeed * 0.6f);

        timer = countdownTime;
        animIndex = 0;
        animTimer = 0f;

        if (runSprites.Count > 0 && patientImage != null)
            patientImage.sprite = runSprites[0];

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
        bool passedFinish = patientX >= finishX;
        bool withinTolerance = (timer > -0.3f && timer < 0.3f);

        if (passedFinish || (!passedFinish && withinTolerance))
        {
            // ✅ success → tampilkan sesuai kondisi pasien
            if (lastPatient != null)
            {
                if (lastPatient.activity == 1)
                    feedbackText.text = "Active!";
                else
                    feedbackText.text = "Inactive!";
            }
            else
            {
                feedbackText.text = "Success!";
            }
        }
        else
        {
            feedbackText.text = "Failed!";
        }
    }

    // 🔹 Hentikan animasi (sprite diam di tempat)
    isMoving = false;

    if (idleSprite != null && patientImage != null)
        patientImage.sprite = idleSprite;

    if (toggleButtonText != null) toggleButtonText.text = "Start";
}



private void ForceFail()
{
    isStopped = true;
    isMoving = false;

    if (feedbackText != null)
        feedbackText.text = "Failed! (Too late)";

    if (idleSprite != null && patientImage != null)
        patientImage.sprite = idleSprite;

    if (toggleButtonText != null) toggleButtonText.text = "Start";
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

        if (toggleButtonText != null)
            toggleButtonText.text = "Start";

        timer = countdownTime;
        isMoving = false;
        isStopped = false;
        toggleClicks = 0;

        animIndex = 0;
        animTimer = 0f;

        if (idleSprite != null && patientImage != null)
            patientImage.sprite = idleSprite;
        else if (runSprites.Count > 0 && patientImage != null)
            patientImage.sprite = runSprites[0];

        if (feedbackText != null)
            feedbackText.text = "Press Start to play!";

        UpdateTimerUI();
    }

    private void OnSubmit()
    {
        ResetMinigame(lastPatient != null ? lastPatient.activity : 1);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ActivityMinigame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform patient;   
    public Image patientImage;      
    public RectTransform finishLine;
    public Button toggleButton;     
    public TextMeshProUGUI toggleButtonText; 
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failText;
    public Button closeButton;

    [Header("Settings")]
    public float countdownTime = 3f;
    public float trackLength = 1200f;

    [Header("Animation")]
    public List<Sprite> runSprites; 
    public float frameRate = 0.1f;  
    public Sprite idleSprite;       

    [Header("Audio")]
    public AudioSource runAudio; // drag SFX running di sini

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
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMinigame);
    }

    private void CloseMinigame()
    {
        gameObject.SetActive(false);
        ResetMinigame();
        if (runAudio != null && runAudio.isPlaying) runAudio.Stop();
    }

    private void Update()
    {
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient current = PatientUI.Instance.currentPatient;
            if (current != lastPatient)
            {
                lastPatient = current;
                ResetMinigame(current.activity);
            }
        }

        if (!isMoving || isStopped) 
        {
            if (runAudio != null && runAudio.isPlaying)
                runAudio.Stop();
            return;
        }

        // play running SFX
        if (runAudio != null && !runAudio.isPlaying)
            runAudio.Play();

        // Update timer
        timer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = $"Time: {timer:F1}s";

        // Move patient
        patient.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0f);

        AnimateRun();

        // Update feedback text
        if (feedbackText != null && lastPatient != null)
        {
            bool passedFinish = patient.anchoredPosition.x >= finishLine.anchoredPosition.x;
            feedbackText.text = $"Current patient's activity level: {(passedFinish ? "Active" : "Inactive")}";
        }

        // Auto-fail jika patient melewati end track
        if (patient.anchoredPosition.x >= startPos.x + trackLength + 50f)
        {
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

        if (feedbackText != null && lastPatient != null)
            feedbackText.text = $"Current patient's activity level: {(lastPatient.activity == 1 ? "Active" : "Inactive")}";

        if (successText != null) successText.gameObject.SetActive(false);
        if (failText != null) failText.gameObject.SetActive(false);
    }

    private void StopMinigame()
    {
        if (!isMoving || isStopped) return;

        isStopped = true;

        if (runAudio != null && runAudio.isPlaying) runAudio.Stop();

        float finishX = finishLine != null ? finishLine.anchoredPosition.x : startPos.x + trackLength;
        float patientX = patient != null ? patient.anchoredPosition.x : startPos.x;

        bool passedFinish = patientX >= finishX;
        bool success = false;

        if (lastPatient != null)
        {
            if (lastPatient.activity == 1)
                success = passedFinish;
            else
                success = (timer > -0.3f && timer < 0.3f && !passedFinish);

            PatientUI.Instance.currentPatient.activity = passedFinish ? 1 : 0;
            PatientUI.Instance.FillField("Activity");
            PatientUI.Instance.RefreshDropdowns(PatientUI.Instance.currentPatient);
        }

        UpdateFeedback(success);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(success);

        isMoving = false;

        if (idleSprite != null && patientImage != null)
            patientImage.sprite = idleSprite;

        if (toggleButtonText != null)
            toggleButtonText.text = "Start";
    }

    private void ForceFail()
    {
        isStopped = true;
        isMoving = false;

        if (runAudio != null && runAudio.isPlaying) runAudio.Stop();

        if (feedbackText != null && lastPatient != null)
            feedbackText.text = $"Current patient's activity level: Active";

        if (lastPatient != null)
        {
            PatientUI.Instance.currentPatient.activity = 1;
            PatientUI.Instance.FillField("Activity");
            PatientUI.Instance.RefreshDropdowns(PatientUI.Instance.currentPatient);
        }

        if (failText != null) failText.gameObject.SetActive(true);
        if (successText != null) successText.gameObject.SetActive(false);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(false);

        if (idleSprite != null && patientImage != null)
            patientImage.sprite = idleSprite;

        if (toggleButtonText != null)
            toggleButtonText.text = "Start";
    }

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
            feedbackText.text = "Current patient's activity level: Inactive";

        if (successText != null) successText.gameObject.SetActive(false);
        if (failText != null) failText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.text = $"Time: {timer:F1}s";
    }

    private void UpdateFeedback(bool success)
    {
        if (success)
        {
            if (successText != null) successText.gameObject.SetActive(true);
            if (failText != null) failText.gameObject.SetActive(false);
        }
        else
        {
            if (failText != null) failText.gameObject.SetActive(true);
            if (successText != null) successText.gameObject.SetActive(false);
        }
    }
}

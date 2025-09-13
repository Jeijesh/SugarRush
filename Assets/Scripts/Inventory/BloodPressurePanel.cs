using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BloodPressureMinigame : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statusText;   // Status Normal/High
    public TextMeshProUGUI timerText;    // Timer
    public TextMeshProUGUI counterText;  // Pumps
    public TextMeshProUGUI bpText;       // 110/80 dll
    public Image pumpImage;               // Pump UI
    public Button exitButton;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pumpSfx;

    [Header("Settings")]
    public float pumpDuration = 5f;      // durasi game
    public int targetClicks = 10;        // jumlah klik ideal
    public float clickEffectRatio = 0.05f;
    public float pumpAnimScale = 1.05f;

    private Patient lastPatient;
    private bool running = false;
    private float timer;
    private int clickCount;
    private Vector3 originalScale;

    private int systolic;
    private int diastolic;
    private int patientSystolic;
    private int patientDiastolic;

    private Coroutine pumpCoroutine;

    private void Awake()
    {
        if (pumpImage != null)
            originalScale = pumpImage.rectTransform.localScale;

        if (exitButton != null)
            exitButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void Update()
    {
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient)
            {
                SetupGame(p);
                lastPatient = p;
            }
        }

        if (!running) return;

        timer -= Time.deltaTime;
        UpdateUI();

        if (timer <= 0f)
        {
            EndGame(clickCount >= targetClicks);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            ApplyClickEffect();
            PlayPumpAnimation();
            PlayPumpAudio();

            if (clickCount >= targetClicks)
            {
                EndGame(true);
            }
        }
    }

    private void SetupGame(Patient p)
    {
        clickCount = 0;
        timer = pumpDuration;
        running = true;

        systolic = 0;
        diastolic = 0;

        lastPatient = p;

        // Ambil nilai pasien
        if (p.bp == 0) // Normal
        {
            patientSystolic = Random.Range(110, 121);
            patientDiastolic = Random.Range(70, 81);
        }
        else // High
        {
            patientSystolic = Random.Range(130, 151);
            patientDiastolic = Random.Range(85, 96);
        }

        // Reset UI
        if (counterText != null) counterText.text = $"Pumps: 0/{targetClicks}";
        if (bpText != null) bpText.text = $"{systolic}/{diastolic}";
        UpdateStatusText();

        if (pumpImage != null) pumpImage.rectTransform.localScale = originalScale;
        if (successText != null) successText.gameObject.SetActive(false);
        if (failText != null) failText.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (bpText != null) bpText.text = $"{systolic}/{diastolic}";
        UpdateStatusText();

        if (counterText != null) counterText.text = $"Pumps: {clickCount}/{targetClicks}";
        if (timerText != null) timerText.text = $"Time: {timer:F1}s";
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            string status = (systolic >= 130 || diastolic >= 85) ? "High" : "Normal";
            statusText.text = $"Status: {status}";
        }
    }

    private void ApplyClickEffect()
    {
        if (targetClicks <= 0) return;

        int systolicStep = Mathf.CeilToInt((patientSystolic - systolic) / (float)(targetClicks - clickCount + 1));
        int diastolicStep = Mathf.CeilToInt((patientDiastolic - diastolic) / (float)(targetClicks - clickCount + 1));

        systolic += systolicStep;
        diastolic += diastolicStep;

        systolic = Mathf.Min(systolic, patientSystolic);
        diastolic = Mathf.Min(diastolic, patientDiastolic);
    }

    private void PlayPumpAnimation()
    {
        if (pumpCoroutine != null)
            StopCoroutine(pumpCoroutine);

        if (pumpImage != null)
            pumpCoroutine = StartCoroutine(PumpAnimCoroutine());
    }

    private IEnumerator PumpAnimCoroutine()
    {
        if (pumpImage == null) yield break;

        RectTransform rt = pumpImage.rectTransform;
        Vector3 targetScale = originalScale * pumpAnimScale;

        float t = 0f;
        while (t < 0.1f)
        {
            if (rt == null) yield break;
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, targetScale, t / 0.1f);
            yield return null;
        }

        t = 0f;
        while (t < 0.1f)
        {
            if (rt == null) yield break;
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(targetScale, originalScale, t / 0.1f);
            yield return null;
        }

        if (rt != null) rt.localScale = originalScale;
    }

    private void PlayPumpAudio()
    {
        if (audioSource != null && pumpSfx != null)
            audioSource.PlayOneShot(pumpSfx);
    }

    private void EndGame(bool success)
    {
        running = false;
        if (lastPatient == null) return;

        // Show feedback
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

        // Update patient BP value
        lastPatient.bp = (systolic >= 130 || diastolic >= 85) ? 1 : 0;

        // Update UI
        if (PatientUI.Instance != null)
        {
            PatientUI.Instance.FillField("BP");
            PatientUI.Instance.RefreshDropdowns(lastPatient);
        }

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(success);

        UpdateUI();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BloodPressureMinigame : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI bpText;       // Text untuk menampilkan BP / status
    public Image pumpImage;

    [Header("Settings")]
    public float pumpDuration = 3f;      // durasi game
    public int targetClicks = 10;        // klik ideal
    public float clickEffectRatio = 0.05f; // proporsi naik per klik

    private Patient lastPatient;
    private bool running = false;
    private float timer;
    private int clickCount;
    private Vector3 originalScale;

    private int systolic;
    private int diastolic;
    private int patientSystolic;  // nilai asli pasien
    private int patientDiastolic;

    private void Awake()
    {
        if (pumpImage != null)
            originalScale = pumpImage.rectTransform.localScale;
    }

    private void Update()
    {
        // 🔹 Deteksi pasien baru secara real-time
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

        // 🔹 Hitung klik pemain
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            if (counterText != null)
                counterText.text = $"Pumps: {clickCount}";

            ApplyClickEffect();
            UpdateBPText();

            // 🔹 Animasi pompa membesar → mengecil
            if (pumpImage != null)
            {
                StopAllCoroutines();
                StartCoroutine(PumpAnimation());
            }

            // 🔹 Cek target klik tercapai
            if (clickCount >= targetClicks)
            {
                FinalizeBP();
            }
        }

        // 🔹 Timer berjalan
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndGame();
        }
    }

    private void SetupGame(Patient p)
    {
        clickCount = 0;
        timer = pumpDuration;
        running = true;

        // Start dari 0/0
        systolic = 0;
        diastolic = 0;

        // Simpan nilai asli pasien
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

        if (feedbackText != null)
            feedbackText.text = "Pump the cuff! (Click fast)";
        if (counterText != null)
            counterText.text = "Pumps: 0";

        if (pumpImage != null)
            pumpImage.rectTransform.localScale = originalScale;

        UpdateBPText();
    }

    private void ApplyClickEffect()
    {
        // Naikkan tekanan per klik secara dinamis menuju pasien
        systolic += Mathf.CeilToInt((patientSystolic * clickEffectRatio));
        diastolic += Mathf.CeilToInt((patientDiastolic * clickEffectRatio));

        // Jangan melebihi pasien
        systolic = Mathf.Min(systolic, patientSystolic);
        diastolic = Mathf.Min(diastolic, patientDiastolic);
    }

    private void FinalizeBP()
    {
        // Setelah target klik tercapai → langsung set ke nilai pasien
        systolic = patientSystolic;
        diastolic = patientDiastolic;

        if (feedbackText != null)
            feedbackText.text = "Target reached!";

        UpdateBPText();
    }

private void UpdateBPText()
{
    if (bpText == null) return;

    string status;

    if (systolic >= 130 || diastolic >= 85) status = "High";
    else if (systolic >= 110 && systolic <= 120 && diastolic >= 70 && diastolic <= 80) status = "Normal";
    else status = "Low";

    bpText.text = $"BP: {systolic}/{diastolic}\nStatus: {status}";
}


    private void EndGame()
    {
        running = false;

        if (lastPatient == null) return;

        if (feedbackText != null)
            feedbackText.text += "\nGame Over";

        UpdateBPText();
    }

    private IEnumerator PumpAnimation()
    {
        if (pumpImage == null) yield break;

        RectTransform rt = pumpImage.rectTransform;
        Vector3 targetScale = originalScale * 1.05f;

        float t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, targetScale, t / 0.1f);
            yield return null;
        }

        t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(targetScale, originalScale, t / 0.1f);
            yield return null;
        }

        rt.localScale = originalScale;
    }
}

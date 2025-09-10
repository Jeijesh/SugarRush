using UnityEngine;
using TMPro;

public class BloodPressureMinigame : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI counterText;

    [Header("Settings")]
    public float pumpDuration = 2f; // durasi klik (detik)

    private Patient lastPatient;
    private bool running = false;
    private float timer;
    private int clickCount;

    private void Update()
    {
        // Reset kalau pasien berganti
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

        // Hitung klik
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            if (counterText != null) counterText.text = $"Pumps: {clickCount}";
        }

        // Timer berjalan
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndGame();
        }
    }

    private void SetupGame(Patient p)
    {
        // Reset state
        clickCount = 0;
        timer = pumpDuration;
        running = true;

        if (feedbackText != null) feedbackText.text = "Pump the cuff! (Click fast)";
        if (counterText != null) counterText.text = "Pumps: 0";
    }

    private void EndGame()
    {
        running = false;

        if (lastPatient == null) return;

        // Ambil hasil dari p.bp
        string result = "Unknown";
        if (lastPatient.bp == 0) result = "Normal";
        else if (lastPatient.bp == 1) result = "High";

        if (feedbackText != null) feedbackText.text = $"Result: {result}";
    }
}

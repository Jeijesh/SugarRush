using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeasuringTapeAutoPingPong : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform tapeBar;
    public RectTransform targetZone;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI feedbackText;
    public Button exitButton;

    [Header("Settings")]
    public float growSpeed = 200f;
    public float minWaist = 60f;
    public float maxWaist = 120f;
    public float barMaxWidth = 500f;

    private float targetValue;
    private float currentValue;
    private bool stopped = false;
    private bool growingRight = true;
    private float startWidth;
    private Vector2 startPos;
    private Patient lastPatient;

private void Start()
{
    if (tapeBar != null)
    {
        tapeBar.pivot = new Vector2(0f, tapeBar.pivot.y); // pivot kiri
        startPos = tapeBar.anchoredPosition;
        startWidth = 0f; // mulai dari 0 cm
        tapeBar.sizeDelta = new Vector2(startWidth, tapeBar.sizeDelta.y);
    }

    ResetTape();
    if (exitButton != null)
        exitButton.onClick.AddListener(() => gameObject.SetActive(false));
}



    private void Update()
    {
        // Deteksi pasien baru
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient)
            {
                lastPatient = p;
                targetValue = Mathf.Clamp(p.waist, minWaist, maxWaist);

                // simpan posisi & ukuran awal
                startPos = tapeBar != null ? tapeBar.anchoredPosition : Vector2.zero;
                startWidth = tapeBar != null ? tapeBar.sizeDelta.x : 0f;

                SetTargetZone(targetValue);
                ResetTape();
            }
        }

        if (stopped || tapeBar == null) return;

        if (Input.GetKey(KeyCode.Space))
        {
            float delta = growSpeed * Time.deltaTime;
            // maju atau mundur
            if (growingRight)
                tapeBar.sizeDelta = new Vector2(tapeBar.sizeDelta.x + delta, tapeBar.sizeDelta.y);
            else
                tapeBar.sizeDelta = new Vector2(tapeBar.sizeDelta.x - delta, tapeBar.sizeDelta.y);

            // cek batas & balik arah
            if (tapeBar.sizeDelta.x >= startWidth + barMaxWidth) growingRight = false;
            if (tapeBar.sizeDelta.x <= startWidth) growingRight = true;

            // update nilai normal
            float normalized = (tapeBar.sizeDelta.x - startWidth) / barMaxWidth;
            currentValue = Mathf.Lerp(minWaist, maxWaist, normalized);

            // cek target zone
            float barEnd = tapeBar.anchoredPosition.x + tapeBar.sizeDelta.x;
            float zoneLeft = targetZone.anchoredPosition.x - targetZone.rect.width / 2f;
            float zoneRight = targetZone.anchoredPosition.x + targetZone.rect.width / 2f;
            if (barEnd >= zoneLeft && barEnd <= zoneRight)
                currentValue = targetValue;

            if (valueText != null)
                valueText.text = $"{currentValue:F1} cm";
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stopped = true;
            CheckResult(currentValue);
        }
    }

    private void CheckResult(float value)
    {
        if (Mathf.Abs(value - targetValue) <= 2f)
        {
            if (feedbackText != null) feedbackText.text = "Perfect!";
            if (valueText != null) valueText.text = targetValue.ToString("F1");
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "Miss!";
            if (valueText != null) valueText.text = value.ToString("F1");
        }
    }

    // Reset otomatis seperti ScalePanel / ActivityMinigame
private void ResetTape()
{
    stopped = false;
    growingRight = true;

    if (tapeBar != null)
    {
        tapeBar.anchoredPosition = startPos;

        // kasih lebar minimal biar kepala tape kelihatan
        float minHeadWidth = 80f;
        tapeBar.sizeDelta = new Vector2(minHeadWidth, tapeBar.sizeDelta.y);

        tapeBar.gameObject.SetActive(true);
    }

    if (targetZone != null)
        targetZone.gameObject.SetActive(true);

    if (feedbackText != null)
        feedbackText.text = "Hold Space to measure!";
    if (valueText != null)
        valueText.text = $"{minWaist:F1} cm"; // mulai dari batas min
}



    private void SetTargetZone(float target)
    {
        if (targetZone != null)
        {
            float normalized = (target - minWaist) / (maxWaist - minWaist);
            targetZone.anchoredPosition = new Vector2(
                startPos.x + Mathf.Lerp(0, barMaxWidth, normalized),
                targetZone.anchoredPosition.y
            );
        }
    }
}

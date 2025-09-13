using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeasuringTapeAutoPingPong : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform tapeBar;
    public RectTransform targetZone;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failedText;
    public Button exitButton;

    [Header("Settings")]
    public float growSpeed = 200f;
    public float minWaist = 60f;
    public float maxWaist = 120f;
    public float barMaxWidth = 500f;

    [Header("Audio")]
    public AudioSource tapeAudio; // drag SFX disini

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
            tapeBar.pivot = new Vector2(0f, tapeBar.pivot.y);
            startPos = tapeBar.anchoredPosition;
        }

        ResetTape();

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
                lastPatient = p;
                targetValue = Mathf.Clamp(p.waist, minWaist, maxWaist);

                if (tapeBar != null)
                {
                    tapeBar.pivot = new Vector2(0f, tapeBar.pivot.y);
                    startPos = tapeBar.anchoredPosition;
                    startWidth = 80f;
                    tapeBar.sizeDelta = new Vector2(startWidth, tapeBar.sizeDelta.y);
                }

                SetTargetZone(targetValue);
                ResetTape();
            }
        }

        if (stopped || tapeBar == null) 
        {
            if (tapeAudio != null && tapeAudio.isPlaying) tapeAudio.Stop();
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            float delta = growSpeed * Time.deltaTime;
            tapeBar.sizeDelta = new Vector2(
                growingRight ? tapeBar.sizeDelta.x + delta : tapeBar.sizeDelta.x - delta,
                tapeBar.sizeDelta.y
            );

            if (tapeBar.sizeDelta.x >= startWidth + barMaxWidth) growingRight = false;
            if (tapeBar.sizeDelta.x <= startWidth) growingRight = true;

            float normalized = (tapeBar.sizeDelta.x - startWidth) / barMaxWidth;
            currentValue = Mathf.Lerp(minWaist, maxWaist, normalized);

            float barEnd = tapeBar.anchoredPosition.x + tapeBar.sizeDelta.x;
            float zoneLeft = targetZone.anchoredPosition.x - targetZone.rect.width / 2f;
            float zoneRight = targetZone.anchoredPosition.x + targetZone.rect.width / 2f;
            if (barEnd >= zoneLeft && barEnd <= zoneRight)
                currentValue = targetValue;

            if (valueText != null)
                valueText.text = $"{currentValue:F1} cm";

            // play audio saat bar bergerak
            if (tapeAudio != null && !tapeAudio.isPlaying)
                tapeAudio.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stopped = true;
            if (tapeAudio != null && tapeAudio.isPlaying)
                tapeAudio.Stop();

            CheckResult(currentValue);
        }
    }

    private void CheckResult(float value)
    {
        bool success = Mathf.Abs(value - targetValue) <= 2f;

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);

        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;

            if (success)
            {
                if (successText != null) successText.gameObject.SetActive(true);
                p.waist = targetValue;
            }
            else
            {
                if (failedText != null) failedText.gameObject.SetActive(true);
                p.waist = value;
            }

            PatientUI.Instance.FillField("Waist");
            PatientUI.Instance.RefreshDropdowns(p);
        }

        if (valueText != null)
            valueText.text = $"{(success ? targetValue : value):F1} cm";

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(success);
    }

    private void ResetTape()
    {
        stopped = false;
        growingRight = true;

        if (tapeBar != null)
        {
            tapeBar.anchoredPosition = startPos;
            tapeBar.sizeDelta = new Vector2(startWidth, tapeBar.sizeDelta.y);
            tapeBar.gameObject.SetActive(true);
        }

        if (targetZone != null)
            targetZone.gameObject.SetActive(true);

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);

        if (valueText != null)
            valueText.text = $"{minWaist:F1} cm";
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

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScalePanel : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform track;         
    public RectTransform movingBar;     
    public RectTransform targetZone;    
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI valueText;
    public Button exitButton;

    [Header("Settings")]
    public float movingSpeed = 200f;
    public int minValue = 15;
    public int maxValue = 40;

    private bool movingRight = true;
    private float leftLimit = -475f;
    private float rightLimit = 475f;
    private float fixedY = -250f;
    private bool barStopped = false;
    private Patient lastPatient;
    private float targetBMI;   // simpan BMI asli pasien

    private void Start()
    {
        ResetPanel();
        if (exitButton != null) 
            exitButton.onClick.AddListener(ClosePanel);

        if (track != null)
            track.localPosition = new Vector3(0f, fixedY, 0f);
    }

    private void Update()
    {
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient) // pasien baru
            {
                ResetPanel();
                targetBMI = Mathf.Clamp(p.bmi, minValue, maxValue);
                SetTargetByBMI(targetBMI);
                lastPatient = p;
            }
        }

        if (barStopped || movingBar == null || !movingBar.gameObject.activeSelf) return;

        float step = movingSpeed * Time.deltaTime * (movingRight ? 1f : -1f);
        Vector3 pos = movingBar.localPosition;
        pos.x += step;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        pos.y = fixedY;
        movingBar.localPosition = pos;

        if (pos.x >= rightLimit) movingRight = false;
        else if (pos.x <= leftLimit) movingRight = true;

        float normalized = (pos.x - leftLimit) / (rightLimit - leftLimit);
        float value = Mathf.Lerp(minValue, maxValue, normalized);

        if (valueText != null) 
            valueText.text = value.ToString("F1");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            barStopped = true;
            SetVisuals(false); // matikan semua visual
            CheckHit(value);
        }
    }

    private void CheckHit(float value)
    {
        float targetLeft = targetZone.localPosition.x - targetZone.rect.width / 2;
        float targetRight = targetZone.localPosition.x + targetZone.rect.width / 2;
        float barCenter = movingBar.localPosition.x;

        if (barCenter >= targetLeft && barCenter <= targetRight)
        {
            if (feedbackText != null) 
                feedbackText.text = "Perfect!";
            if (valueText != null) 
                valueText.text = targetBMI.ToString("F1");
        }
        else
        {
            if (feedbackText != null) 
                feedbackText.text = "Miss!";
            if (valueText != null) 
                valueText.text = value.ToString("F1");
        }
    }

    public void ResetPanel()
    {
        SetVisuals(true); // nyalakan lagi semua visual
        if (movingBar != null)
            movingBar.localPosition = new Vector3(leftLimit, fixedY, 0f);

        if (feedbackText != null) feedbackText.text = "Press Space!";
        if (valueText != null) valueText.text = minValue.ToString("F1");

        movingRight = true;
        barStopped = false;
    }

    private void SetTargetByBMI(float bmi)
    {
        bmi = Mathf.Clamp(bmi, minValue, maxValue);
        float normalized = (bmi - minValue) / (float)(maxValue - minValue);
        if (targetZone != null) 
            targetZone.localPosition = new Vector3(Mathf.Lerp(leftLimit, rightLimit, normalized), fixedY, 0f);
    }

    private void SetVisuals(bool state)
    {
        if (track != null) track.gameObject.SetActive(state);
        if (movingBar != null) movingBar.gameObject.SetActive(state);
        if (targetZone != null) targetZone.gameObject.SetActive(state);
    }

    private void ClosePanel() => gameObject.SetActive(false);
}

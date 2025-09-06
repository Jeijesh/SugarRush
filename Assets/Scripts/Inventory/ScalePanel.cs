using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScalePanel : MonoBehaviour
{
    [Header("UI References")]
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

    private void Start()
    {
        ResetPanel();
        if(exitButton!=null) exitButton.onClick.AddListener(ClosePanel);
    }

    private void Update()
    {
        if(PatientUI.Instance!=null && PatientUI.Instance.currentPatient!=null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if(p!=lastPatient)
            {
                ResetPanel();
                SetTargetByBMI(Mathf.RoundToInt(p.bmi));
                lastPatient = p;
            }
        }

        if(barStopped || movingBar==null) return;

        float step = movingSpeed * Time.deltaTime * (movingRight?1f:-1f);
        Vector3 pos = movingBar.localPosition;
        pos.x += step;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        pos.y = fixedY;
        movingBar.localPosition = pos;

        if(pos.x>=rightLimit) movingRight=false;
        else if(pos.x<=leftLimit) movingRight=true;

        float normalized = (pos.x-leftLimit)/(rightLimit-leftLimit);
        int value = Mathf.RoundToInt(Mathf.Lerp(minValue,maxValue,normalized));
        if(valueText!=null) valueText.text = value.ToString();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            barStopped=true;
            movingBar.gameObject.SetActive(false);
            CheckHit(value);
        }
    }

    private void CheckHit(int value)
    {
        float targetLeft = targetZone.localPosition.x - targetZone.rect.width/2;
        float targetRight = targetZone.localPosition.x + targetZone.rect.width/2;
        float barCenter = movingBar.localPosition.x;

        if(barCenter>=targetLeft && barCenter<=targetRight)
            if(feedbackText!=null) feedbackText.text=$"Perfect! Value: {value}";
        else
            if(feedbackText!=null) feedbackText.text=$"Miss! Value: {value}";
    }

    public void ResetPanel()
    {
        if(movingBar!=null)
        {
            movingBar.gameObject.SetActive(true);
            movingBar.localPosition=new Vector3(leftLimit,fixedY,0f);
        }
        if(feedbackText!=null) feedbackText.text="Press Space!";
        if(valueText!=null) valueText.text=minValue.ToString();
        movingRight=true;
        barStopped=false;
    }

    private void SetTargetByBMI(int bmi)
    {
        bmi=Mathf.Clamp(bmi,minValue,maxValue);
        float normalized=(bmi-minValue)/(float)(maxValue-minValue);
        if(targetZone!=null) targetZone.localPosition=new Vector3(Mathf.Lerp(leftLimit,rightLimit,normalized),fixedY,0f);
    }

    private void ClosePanel() => gameObject.SetActive(false);
}

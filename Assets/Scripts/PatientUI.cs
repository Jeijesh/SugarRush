using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PatientUI : MonoBehaviour
{
    public static PatientUI Instance;

    private bool muteSound = false;

    [Header("UI References")]
    public TextMeshProUGUI idText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI riskLevelText;
    public TextMeshProUGUI riskScoreText;
    public TextMeshProUGUI patientInfoText;
    public TextMeshProUGUI genderText;

    [Header("Dropdowns")]
    public TMP_Dropdown ageDropdown, bmiDropdown, waistDropdown,
                        activityDropdown, fruitDropdown, bpDropdown,
                        glucoseDropdown, familyDropdown;

    [Header("Button")]
    public Button submitButton;

    [Header("Audio")]
    public AudioSource audioSource;    // drag di inspector (Canvas/objek PatientUI)
    public AudioClip dropdownSfx;      // drag file audio di inspector

    [HideInInspector] public Patient currentPatient;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetupDropdowns();
        SetupListeners();
    }

    private void SetupDropdowns()
    {
        ageDropdown.ClearOptions();
        ageDropdown.AddOptions(new List<string> { "<45", "45-54", "55-64", ">64" });

        bmiDropdown.ClearOptions();
        bmiDropdown.AddOptions(new List<string> { "<25", "25-30", ">30" });

        waistDropdown.ClearOptions();
        waistDropdown.AddOptions(new List<string> { "<94", "94-102", ">102" });

        activityDropdown.ClearOptions();
        activityDropdown.AddOptions(new List<string> { "Active", "Inactive" });

        fruitDropdown.ClearOptions();
        fruitDropdown.AddOptions(new List<string> { "Sufficient", "Insufficient" });

        bpDropdown.ClearOptions();
        bpDropdown.AddOptions(new List<string> { "Normal", "High" });

        glucoseDropdown.ClearOptions();
        glucoseDropdown.AddOptions(new List<string> { "Normal", "High" });

        familyDropdown.ClearOptions();
        familyDropdown.AddOptions(new List<string> { "None", "Level1", "Level2" });
    }

    private void SetupListeners()
    {
        // dropdown → update risk + SFX
        ageDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        bmiDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        waistDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        activityDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        fruitDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        bpDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        glucoseDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        familyDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());

        // button → submit tanpa bunyi
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnDropdownChanged()
    {
        UpdateRisk();

        // skip SFX saat mute (misalnya pas ResetDropdowns)
        if (muteSound) return;

        if (audioSource != null && dropdownSfx != null)
        {
            audioSource.PlayOneShot(dropdownSfx);
        }
    }

    public void ShowPatient(Patient p)
    {
        currentPatient = p;
        int patientTotalScore = p.GetTotalScore();
        string patientRiskText = p.riskLevel;

        if (idText != null) idText.text = $"ID\t: {p.patientID}";
        if (nameText != null) nameText.text = $"Name\t: {p.patientName}";
        if (riskLevelText != null) riskLevelText.text = patientRiskText;
        if (riskScoreText != null) riskScoreText.text = patientTotalScore.ToString();
        if (genderText != null)
            genderText.text = $"Sex\t: {(p.gender == 0 ? "Female" : "Male")}";

        string info =
            $"ID: {p.patientID}\n" +
            $"Name: {p.patientName}\n" +
            $"Age: {p.age}\n" +
            $"BMI: {p.bmi:F1}\n" +
            $"Waist: {p.waist:F1}\n" +
            $"Activity: {(p.activity == 1 ? "Active" : "Inactive")}\n" +
            $"Fruit: {(p.fruit == 1 ? "Sufficient" : "Insufficient")}\n" +
            $"BP: {(p.bp == 1 ? "High" : "Normal")}\n" +
            $"Glucose: {(p.glucose == 1 ? "High" : "Normal")}\n" +
            $"Family: {(p.family == 0 ? "None" : p.family == 1 ? "Level1" : "Level2")}\n" +
            $"Gender: {(p.gender == 0 ? "Female" : "Male")}\n" +
            $"BodyType: {(p.bodyType == 0 ? "Normal" : "Overweight")}\n" +
            $"Skin: #{ColorUtility.ToHtmlStringRGB(p.skinColor)}\n" +
            $"Risk: {p.riskLevel}";

        if (patientInfoText != null) patientInfoText.text = info;

        ResetDropdowns();
        UpdateRisk();
    }

    private void ResetDropdowns()
    {
        muteSound = true;

        ageDropdown.value = bmiDropdown.value = waistDropdown.value =
        activityDropdown.value = fruitDropdown.value = bpDropdown.value =
        glucoseDropdown.value = familyDropdown.value = 0;

        muteSound = false;
    }

    private void OnSubmit()
    {
        if (currentPatient == null) return;

        int userScore = CalculateRisk();
        GameManager.Instance.SubmitAnswer(userScore);
    }

    private void UpdateRisk()
    {
        if (currentPatient == null) return;

        int currentScore = CalculateRisk();
        string currentRiskTextStr = Patient.CalculateRiskLevel(currentScore);

        if (riskLevelText != null)
        {
            riskLevelText.text = currentRiskTextStr;
            riskLevelText.fontSize = currentRiskTextStr == "Slightly Elevated" ? 14 : 20;
        }

        if (riskScoreText != null)
        {
            riskScoreText.text = currentScore.ToString();
            riskScoreText.color = GetRiskColor(currentRiskTextStr);
        }
    }

    private Color GetRiskColor(string riskLevel)
    {
        return riskLevel switch
        {
            "Low"              => new Color(0.0f, 0.6f, 0.0f),
            "Slightly Elevated"=> new Color(0.8f, 0.6f, 0.0f),
            "Moderate"         => new Color(1.0f, 0.4f, 0.0f),
            "High"             => new Color(0.8f, 0.0f, 0.0f),
            "Very High"        => new Color(0.5f, 0.0f, 0.0f),
            _                  => Color.white
        };
    }

    private int CalculateRisk()
    {
        int score = 0;
        score += ageDropdown.value switch { 0 => 0, 1 => 2, 2 => 3, 3 => 4, _ => 0 };
        score += bmiDropdown.value switch { 0 => 0, 1 => 1, 2 => 3, _ => 0 };
        score += waistDropdown.value switch { 0 => 0, 1 => 3, 2 => 4, _ => 0 };
        score += activityDropdown.value == 1 ? 2 : 0;
        score += fruitDropdown.value == 1 ? 1 : 0;
        score += bpDropdown.value == 1 ? 2 : 0;
        score += glucoseDropdown.value == 1 ? 5 : 0;
        score += familyDropdown.value switch { 1 => 3, 2 => 5, _ => 0 };
        return score;
    }
}

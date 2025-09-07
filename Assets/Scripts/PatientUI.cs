using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PatientUI : MonoBehaviour
{
    public static PatientUI Instance;

    [Header("UI References")]
    public TextMeshProUGUI idText;           // ID pasien
    public TextMeshProUGUI nameText;         // Nama pasien
    public TextMeshProUGUI riskLevelText;    // Risk level teks (Low, Slightly Elevated, ...)
    public TextMeshProUGUI riskScoreText;    // Risk score angka
    public TextMeshProUGUI patientInfoText;  // Info tambahan
    public TextMeshProUGUI genderText; // Gender pasien

    [Header("Dropdowns")]
    public TMP_Dropdown ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown, bpDropdown, glucoseDropdown, familyDropdown;

    [Header("Button")]
    public Button submitButton;

    [HideInInspector] public Patient currentPatient;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetupDropdowns();
        SetupListeners();
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void SetupDropdowns()
    {
        ageDropdown.ClearOptions();
        ageDropdown.AddOptions(new List<string>{"<45","45-54","55-64",">64"});

        bmiDropdown.ClearOptions();
        bmiDropdown.AddOptions(new List<string>{"<25","25-30",">30"});

        waistDropdown.ClearOptions();
        waistDropdown.AddOptions(new List<string>{"<94","94-102",">102"});

        activityDropdown.ClearOptions();
        activityDropdown.AddOptions(new List<string>{"Active","Inactive"});

        fruitDropdown.ClearOptions();
        fruitDropdown.AddOptions(new List<string>{"Sufficient","Insufficient"});

        bpDropdown.ClearOptions();
        bpDropdown.AddOptions(new List<string>{"Normal","High"});

        glucoseDropdown.ClearOptions();
        glucoseDropdown.AddOptions(new List<string>{"Normal","High"});

        familyDropdown.ClearOptions();
        familyDropdown.AddOptions(new List<string>{"None","Level1","Level2"});
    }

    private void SetupListeners()
    {
        ageDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        bmiDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        waistDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        activityDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        fruitDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        bpDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        glucoseDropdown.onValueChanged.AddListener(_=>UpdateRisk());
        familyDropdown.onValueChanged.AddListener(_=>UpdateRisk());
    }

    public void ShowPatient(Patient p)
    {
        currentPatient = p;
        int patientTotalScore = p.GetTotalScore();
        string patientRiskText = p.riskLevel;

        // Update ID & Name terpisah
        if (idText != null) idText.text = $"ID\t: {p.patientID}";
        if (nameText != null) nameText.text = $"Name\t: {p.patientName}";

        // Update Risk teks dan angka terpisah
        if (riskLevelText != null) riskLevelText.text = patientRiskText;
        if (riskScoreText != null) riskScoreText.text = patientTotalScore.ToString();
        if (genderText != null)
            genderText.text = $"Sex\t: {(p.gender == 0 ? "Female" : "Male")}";

        // Info tambahan
        string info = $"ID: {p.patientID}\nName: {p.patientName}\nAge: {p.age}\nBMI: {p.bmi:F1}\nWaist: {p.waist:F1}\n" + $"Activity: {(p.activity==1?"Active":"Inactive")}\nFruit: {(p.fruit==1?"Sufficient":"Insufficient")}\n" + $"BP: {(p.bp==1?"High":"Normal")}\nGlucose: {(p.glucose==1?"High":"Normal")}\nFamily: {(p.family==0?"None":p.family==1?"Level1":"Level2")}\n" + $"Gender: {(p.gender==0?"Female":"Male")}\nBodyType: {(p.bodyType==0?"Normal":"Overweight")}\nSkin: #{ColorUtility.ToHtmlStringRGB(p.skinColor)}\nRisk: {p.riskLevel}";

        if (patientInfoText != null) patientInfoText.text = info;

        ResetDropdowns();
        UpdateRisk();
    }

    private void ResetDropdowns()
    {
        ageDropdown.value = bmiDropdown.value = waistDropdown.value = activityDropdown.value =
        fruitDropdown.value = bpDropdown.value = glucoseDropdown.value = familyDropdown.value = 0;
    }

    private void OnSubmit()
    {
        if (currentPatient == null) return;

        int userScore = CalculateRisk(); // numeric score dari dropdown
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

        // Atur ukuran font
        riskLevelText.fontSize = currentRiskTextStr == "Slightly Elevated" ? 14 : 20;
    }    if (riskScoreText != null) 
    {
        riskScoreText.text = currentScore.ToString();
        riskScoreText.color = GetRiskColor(currentRiskTextStr); // set warna
    }
}

// Fungsi untuk menentukan warna sesuai risk level
private Color GetRiskColor(string riskLevel)
{
    return riskLevel switch
    {
        "Low" => new Color(0.0f, 0.6f, 0.0f),               // hijau gelap
        "Slightly Elevated" => new Color(0.8f, 0.6f, 0.0f), // kuning keemasan
        "Moderate" => new Color(1.0f, 0.4f, 0.0f),          // oranye jenuh
        "High" => new Color(0.8f, 0.0f, 0.0f),              // merah terang tapi jelas
        "Very High" => new Color(0.5f, 0.0f, 0.0f),          // merah gelap
        _ => Color.white
    };
}


    // Return numeric score dari dropdown
    private int CalculateRisk()
    {
        int score = 0;
        score += ageDropdown.value switch {0=>0,1=>2,2=>3,3=>4,_=>0};
        score += bmiDropdown.value switch {0=>0,1=>1,2=>3,_=>0};
        score += waistDropdown.value switch {0=>0,1=>3,2=>4,_=>0};
        score += activityDropdown.value==1?2:0;
        score += fruitDropdown.value==1?1:0;
        score += bpDropdown.value==1?2:0;
        score += glucoseDropdown.value==1?5:0;
        score += familyDropdown.value switch {1=>3,2=>5,_=>0};
        return score;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatientUI : MonoBehaviour
{
    public static PatientUI Instance;

    private bool muteSound = false;
    private int patientIndex = 0;

    [Header("Referensi UI")]
    public TextMeshProUGUI idText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI riskLevelText;
    public TextMeshProUGUI riskScoreText;
    public TextMeshProUGUI genderText;

    [Header("Dropdowns")]
    public TMP_Dropdown ageDropdown, bmiDropdown, waistDropdown,
                        activityDropdown, fruitDropdown, bpDropdown,
                        glucoseDropdown, familyDropdown;

    [Header("Tombol")]
    public Button submitButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dropdownSfx;

    [HideInInspector] public Patient currentPatient;

    private List<TMP_Dropdown> emptyDropdowns = new List<TMP_Dropdown>();
    public List<TMP_Dropdown> EmptyDropdowns => emptyDropdowns;

    public event System.Action OnEmptyDropdownChanged;

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
        activityDropdown.AddOptions(new List<string> { "Aktif", "Tidak Aktif" });

        fruitDropdown.ClearOptions();
        fruitDropdown.AddOptions(new List<string> { "Cukup", "Kurang" });

        bpDropdown.ClearOptions();
        bpDropdown.AddOptions(new List<string> { "Normal", "Tinggi" });

        glucoseDropdown.ClearOptions();
        glucoseDropdown.AddOptions(new List<string> { "Normal", "Tinggi" });

        familyDropdown.ClearOptions();
        familyDropdown.AddOptions(new List<string> { "Tidak Ada", "Level1", "Level2" });
    }

    private void SetupListeners()
    {
        TMP_Dropdown[] dropdowns =
        { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown,
          bpDropdown, glucoseDropdown, familyDropdown };

        foreach (var dd in dropdowns)
        {
            if (dd != null)
                dd.onValueChanged.AddListener(_ => OnDropdownChanged());
        }

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
    }

    public void OnDropdownChanged()
    {
        UpdateRisk();

        if (!muteSound && audioSource != null && dropdownSfx != null)
            audioSource.PlayOneShot(dropdownSfx);

        UpdateSubmitButtonState();
        OnEmptyDropdownChanged?.Invoke();
    }

    public void ShowPatient(Patient p)
    {
        if (p == null) return;

        currentPatient = p;
        patientIndex++;

        if (idText != null) idText.text = $"ID\t  : {p.patientID}";
        if (nameText != null) nameText.text = $"Nama\t  : {p.patientName}";
        if (riskLevelText != null) riskLevelText.text = p.riskLevel;
        if (riskScoreText != null) riskScoreText.text = p.GetTotalScore().ToString();
        if (genderText != null)
            genderText.text = $"Gender: {(p.gender == 0 ? "Perempuan" : "Laki-laki")}";

        ResetDropdownsToPatientData(p);
        SetDropdownInteractable(false);

        emptyDropdowns.Clear();
        int emptyFields = (patientIndex - 1) / 5 + 1;
        if (patientIndex == 1 && emptyFields < 1) emptyFields = 1;
        SetEmptyDropdowns(emptyFields);
        UpdateDropdownVisuals();

        PatientDialogue.Instance?.ShowRandomDialogue();

        OnEmptyDropdownChanged?.Invoke();
    }

    private void ResetDropdownsToPatientData(Patient p)
    {
        muteSound = true;

        ageDropdown.value = MapAgeToDropdown(p.age);
        bmiDropdown.value = MapBmiToDropdown(p.bmi);
        waistDropdown.value = MapWaistToDropdown(p.waist);
        activityDropdown.value = (p.activity == 1) ? 0 : 1;
        fruitDropdown.value = (p.fruit == 1) ? 0 : 1;
        bpDropdown.value = p.bp;
        glucoseDropdown.value = p.glucose;
        familyDropdown.value = p.family;

        muteSound = false;
    }

    private void SetDropdownInteractable(bool state)
    {
        TMP_Dropdown[] dropdowns =
        { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown,
          bpDropdown, glucoseDropdown, familyDropdown };

        foreach (var dd in dropdowns)
            if (dd != null) dd.interactable = state;
    }

    private void SetEmptyDropdowns(int count)
    {
        TMP_Dropdown[] dropdowns =
        { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown,
          bpDropdown, glucoseDropdown, familyDropdown };

        List<int> indices = new List<int>();
        for (int i = 0; i < dropdowns.Length; i++) indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(i, indices.Count);
            int tmp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = tmp;
        }

        count = Mathf.Clamp(count, 0, dropdowns.Length);
        for (int i = 0; i < count; i++)
        {
            TMP_Dropdown dd = dropdowns[indices[i]];
            if (dd != null && !emptyDropdowns.Contains(dd))
                emptyDropdowns.Add(dd);
        }
    }

    public void UpdateDropdownVisuals()
    {
        TMP_Dropdown[] dropdowns =
        { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown,
          bpDropdown, glucoseDropdown, familyDropdown };

        foreach (var dd in dropdowns)
        {
            if (dd == null) continue;

            TMP_Text label = dd.captionText;
            if (label == null) continue;

            if (emptyDropdowns.Contains(dd))
            {
                label.text = "-";
                label.color = Color.red;
                StartCoroutine(ForceSetCaptionNextFrame(dd, "-", Color.red));
            }
            else
            {
                int v = Mathf.Clamp(dd.value, 0, dd.options.Count - 1);
                label.text = dd.options[v].text;
                label.color = Color.black;
            }
        }

        StartCoroutine(UpdateSubmitNextFrame());
        OnEmptyDropdownChanged?.Invoke();
    }

    private IEnumerator ForceSetCaptionNextFrame(TMP_Dropdown dd, string txt, Color col)
    {
        yield return null;
        if (dd == null || !emptyDropdowns.Contains(dd)) yield break;
        dd.captionText.text = txt;
        dd.captionText.color = col;
    }

    private IEnumerator UpdateSubmitNextFrame()
    {
        yield return null;
        UpdateSubmitButtonState();
    }

    private void UpdateSubmitButtonState()
    {
        if (submitButton != null)
            submitButton.interactable = emptyDropdowns.Count == 0;
    }

    public void FillField(string fieldName)
    {
        TMP_Dropdown target = fieldName switch
        {
            "Age" => ageDropdown,
            "BMI" => bmiDropdown,
            "Waist" => waistDropdown,
            "Activity" => activityDropdown,
            "Fruit" => fruitDropdown,
            "BP" => bpDropdown,
            "Glucose" => glucoseDropdown,
            "Family" => familyDropdown,
            _ => null
        };

        if (target != null && emptyDropdowns.Contains(target))
        {
            emptyDropdowns.Remove(target);
            UpdateDropdownVisuals();
            OnEmptyDropdownChanged?.Invoke();
        }
    }

    public void RefreshDropdowns(Patient p)
    {
        ResetDropdownsToPatientData(p);
        UpdateDropdownVisuals();
        UpdateRisk();
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
    string riskEnglish = Patient.CalculateRiskLevel(currentScore);

    // Konversi ke bahasa Indonesia
    string riskIndo = riskEnglish switch
    {
        "Low" => "Rendah",
        "Slightly Elevated" => "Sedikit Meningkat",
        "Moderate" => "Sedang",
        "High" => "Tinggi",
        "Very High" => "Sangat Tinggi",
        _ => riskEnglish
    };

    if (riskLevelText != null)
    {
        riskLevelText.text = riskIndo;
        riskLevelText.fontSize = riskIndo == "Sedikit Meningkat" ? 14 : 20;
    }

    if (riskScoreText != null)
    {
        riskScoreText.text = currentScore.ToString();
        // Gunakan string bahasa Indonesia untuk warna
        riskScoreText.color = GetRiskColor(riskIndo);
    }
}

private Color GetRiskColor(string riskLevel)
{
    return riskLevel switch
    {
        "Rendah" => new Color(0.0f, 0.6f, 0.0f),
        "Sedikit Meningkat" => new Color(0.8f, 0.6f, 0.0f),
        "Sedang" => new Color(1.0f, 0.4f, 0.0f),
        "Tinggi" => new Color(0.8f, 0.0f, 0.0f),
        "Sangat Tinggi" => new Color(0.5f, 0.0f, 0.0f),
        _ => Color.white
    };
}


    private int CalculateRisk()
    {
        int score = 0;

        if (!emptyDropdowns.Contains(ageDropdown))
            score += ageDropdown.value switch { 0 => 0, 1 => 2, 2 => 3, 3 => 4, _ => 0 };

        if (!emptyDropdowns.Contains(bmiDropdown))
            score += bmiDropdown.value switch { 0 => 0, 1 => 1, 2 => 3, _ => 0 };

        if (!emptyDropdowns.Contains(waistDropdown))
            score += waistDropdown.value switch { 0 => 0, 1 => 3, 2 => 4, _ => 0 };

        if (!emptyDropdowns.Contains(activityDropdown))
            score += activityDropdown.value == 1 ? 2 : 0;

        if (!emptyDropdowns.Contains(fruitDropdown))
            score += fruitDropdown.value == 1 ? 1 : 0;

        if (!emptyDropdowns.Contains(bpDropdown))
            score += bpDropdown.value == 1 ? 2 : 0;

        if (!emptyDropdowns.Contains(glucoseDropdown))
            score += glucoseDropdown.value == 1 ? 5 : 0;

        if (!emptyDropdowns.Contains(familyDropdown))
            score += familyDropdown.value switch { 1 => 3, 2 => 5, _ => 0 };

        return score;
    }

    private int MapAgeToDropdown(int age)
    {
        if (age < 45) return 0;
        if (age <= 54) return 1;
        if (age <= 64) return 2;
        return 3;
    }

    private int MapBmiToDropdown(float bmi)
    {
        if (bmi < 25f) return 0;
        if (bmi <= 30f) return 1;
        return 2;
    }

    private int MapWaistToDropdown(float waist)
    {
        if (waist < 94f) return 
0;
        if (waist <= 102f) return 1;
        return 2;
    }
}

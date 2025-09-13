using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatientUI : MonoBehaviour
{
    public static PatientUI Instance;

    private bool muteSound = false;
    private int patientIndex = 0; // hitung pasien yang ditampilkan (dimulai 0)
    private List<TMP_Dropdown> emptyDropdowns = new List<TMP_Dropdown>();

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
    public AudioSource audioSource;
    public AudioClip dropdownSfx;

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

        // NOTE: order here matters for mapping below:
        // index 0 = "Active" (good), index 1 = "Inactive" (bad)
        activityDropdown.ClearOptions();
        activityDropdown.AddOptions(new List<string> { "Active", "Inactive" });

        // index 0 = "Sufficient" (good), index 1 = "Insufficient" (bad)
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
        if (ageDropdown != null) ageDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (bmiDropdown != null) bmiDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (waistDropdown != null) waistDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (activityDropdown != null) activityDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (fruitDropdown != null) fruitDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (bpDropdown != null) bpDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (glucoseDropdown != null) glucoseDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        if (familyDropdown != null) familyDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());

        if (submitButton != null) submitButton.onClick.AddListener(OnSubmit);
    }

    public void OnDropdownChanged()
    {
        UpdateRisk();

        if (muteSound) return;

        if (audioSource != null && dropdownSfx != null)
            audioSource.PlayOneShot(dropdownSfx);
    }

    public void RefreshRiskFromDropdown()
    {
        UpdateRisk();
    }

    // Show pasien: reset dropdowns -> acak field kosong (selalu acak tiap pasien) -> tampilkan visuals
    public void ShowPatient(Patient p)
    {
        if (p == null) return;

        currentPatient = p;
        patientIndex++; // sekarang patientIndex = 1 untuk pasien pertama

        if (idText != null) idText.text = $"ID\t: {p.patientID}";
        if (nameText != null) nameText.text = $"Name\t: {p.patientName}";
        if (riskLevelText != null) riskLevelText.text = p.riskLevel;
        if (riskScoreText != null) riskScoreText.text = p.GetTotalScore().ToString();
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

        // Set semua dropdown sesuai data asli pasien (mapping yang benar)
        ResetDropdownsToPatientData(p);

        // Nonaktifkan dropdown agar pasien tidak bisa ubah
        SetDropdownInteractable(false);

        // --- PENTING: acak ulang field kosong setiap pasien baru ---
        emptyDropdowns.Clear();

        int emptyFields = (patientIndex - 1) / 5 + 1;
        // Pastikan pasien pertama minimal 1 field kosong
        if (patientIndex == 1 && emptyFields < 1) emptyFields = 1;

        SetEmptyDropdowns(emptyFields);

        // Update visual (teks "-" merah atau teks asli)
        UpdateDropdownVisuals();

        // Update risk (berdasarkan dropdown sekarang)
        UpdateRisk();
    }

    private void ResetDropdownsToPatientData(Patient p)
    {
        muteSound = true;

        ageDropdown.value = MapAgeToDropdown(p.age);
        bmiDropdown.value = MapBmiToDropdown(p.bmi);
        waistDropdown.value = MapWaistToDropdown(p.waist);

        // === MAPPING YANG BENAR untuk activity & fruit ===
        // Patient.activity: 0 = Inactive (buruk), 1 = Active (baik)
        // Dropdown order: [0] "Active", [1] "Inactive"
        // Jadi: dropdownIndex = (p.activity == 1) ? 0 : 1
        if (activityDropdown != null)
            activityDropdown.value = (p.activity == 1) ? 0 : 1;

        if (fruitDropdown != null)
            fruitDropdown.value = (p.fruit == 1) ? 0 : 1;

        if (bpDropdown != null) bpDropdown.value = p.bp;
        if (glucoseDropdown != null) glucoseDropdown.value = p.glucose;
        if (familyDropdown != null) familyDropdown.value = p.family;

        muteSound = false;
    }

    private void SetDropdownInteractable(bool state)
    {
        if (ageDropdown != null) ageDropdown.interactable = state;
        if (bmiDropdown != null) bmiDropdown.interactable = state;
        if (waistDropdown != null) waistDropdown.interactable = state;
        if (activityDropdown != null) activityDropdown.interactable = state;
        if (fruitDropdown != null) fruitDropdown.interactable = state;
        if (bpDropdown != null) bpDropdown.interactable = state;
        if (glucoseDropdown != null) glucoseDropdown.interactable = state;
        if (familyDropdown != null) familyDropdown.interactable = state;
    }

    // Pilih 'count' dropdown random untuk dijadikan kosong (unique)
    private void SetEmptyDropdowns(int count)
    {
        TMP_Dropdown[] dropdowns = { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown, bpDropdown, glucoseDropdown, familyDropdown };
        List<int> indices = new List<int>();
        for (int i = 0; i < dropdowns.Length; i++) indices.Add(i);

        // acak
        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(i, indices.Count);
            int tmp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = tmp;
        }

        // pilih pertama 'count' index
        count = Mathf.Clamp(count, 0, dropdowns.Length);
        for (int i = 0; i < count; i++)
        {
            TMP_Dropdown dd = dropdowns[indices[i]];
            if (dd != null && !emptyDropdowns.Contains(dd))
            {
                emptyDropdowns.Add(dd);
            }
        }
    }

    // Update caption text & warna. Untuk mengatasi race-update Unity, kita juga force-set di frame berikutnya.
    public void UpdateDropdownVisuals()
    {
        TMP_Dropdown[] dropdowns =
        {
            ageDropdown, bmiDropdown, waistDropdown, activityDropdown,
            fruitDropdown, bpDropdown, glucoseDropdown, familyDropdown
        };

        foreach (var dd in dropdowns)
        {
            if (dd == null) continue;

            TMP_Text label = dd.captionText;
            if (label == null) continue;

            if (emptyDropdowns.Contains(dd))
            {
                // Force kosong: tampil "-" merah
                label.text = "-";
                label.color = Color.red;

                // Pastikan tidak di-overwrite oleh Unity di frame berikutnya
                StartCoroutine(ForceSetCaptionNextFrame(dd, "-", Color.red));
            }
            else
            {
                // Tampilkan isi asli: option sesuai value, warna hitam
                // Safety: guard index
                int v = Mathf.Clamp(dd.value, 0, dd.options.Count - 1);
                label.text = dd.options[v].text;
                label.color = Color.black;
            }
        }
    }

    private IEnumerator ForceSetCaptionNextFrame(TMP_Dropdown dd, string txt, Color col)
    {
        yield return null; // next frame
        if (dd == null) yield break;
        if (!emptyDropdowns.Contains(dd)) yield break;

        if (dd.captionText != null)
        {
            dd.captionText.text = txt;
            dd.captionText.color = col;
        }
    }

    public void FillField(string fieldName)
    {
        TMP_Dropdown target = null;

        switch (fieldName)
        {
            case "Age": target = ageDropdown; break;
            case "BMI": target = bmiDropdown; break;
            case "Waist": target = waistDropdown; break;
            case "Activity": target = activityDropdown; break;
            case "Fruit": target = fruitDropdown; break;
            case "BP": target = bpDropdown; break;
            case "Glucose": target = glucoseDropdown; break;
            case "Family": target = familyDropdown; break;
        }

        if (target != null && emptyDropdowns.Contains(target))
        {
            emptyDropdowns.Remove(target);
            UpdateDropdownVisuals();
        }
    }

    public void RefreshDropdowns(Patient p)
    {
        // Reset visuals according to current patient data (dipanggil mis. dari minigame setelah update)
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
            "Low"               => new Color(0.0f, 0.6f, 0.0f),
            "Slightly Elevated" => new Color(0.8f, 0.6f, 0.0f),
            "Moderate"          => new Color(1.0f, 0.4f, 0.0f),
            "High"              => new Color(0.8f, 0.0f, 0.0f),
            "Very High"         => new Color(0.5f, 0.0f, 0.0f),
            _                   => Color.white
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

        // activityDropdown: index 1 == Inactive -> penalize
        if (!emptyDropdowns.Contains(activityDropdown))
            score += activityDropdown.value == 1 ? 2 : 0;

        // fruitDropdown: index 1 == Insufficient -> penalize
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

    // Mapping metode untuk index dropdown (untuk set awal dari Patient data)
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
        if (waist < 94f) return 0;
        if (waist <= 102f) return 1;
        return 2;
    }
}

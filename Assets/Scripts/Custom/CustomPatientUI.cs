using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomPatientUI : MonoBehaviour
{
    [Header("Mode Settings")]
    public bool isCustomMode = true;

    [Header("Referensi UI")]
    public TextMeshProUGUI riskLevelText;
    public TextMeshProUGUI riskScoreText;

    [Header("Input Fields")]
    public TMP_InputField nameInput;
    public TMP_Dropdown genderDropdown;

    [Header("Dropdowns")]
    public TMP_Dropdown ageDropdown, bmiDropdown, waistDropdown,
                        activityDropdown, fruitDropdown, bpDropdown,
                        glucoseDropdown, familyDropdown;

    [Header("Appearance Dropdowns")]
    public TMP_Dropdown hairDropdown, clothesDropdown;
    public TMP_Dropdown skinColorDropdown;
    public TMP_Dropdown hairColorDropdown;
    public TMP_Dropdown clothesColorDropdown;

    [Header("Tombol")]
    public Button submitButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dropdownSfx;

    [Header("Visual Manager")]
    public CustomPatientVisualManager visualManager;

    private Color32[] skinColors = new Color32[]
    {
        new Color32(255,224,189,255),
        new Color32(241,194,125,255),
        new Color32(224,172,105,255),
        new Color32(198,134,66,255),
        new Color32(141,85,36,255),
        new Color32(92,60,30,255)
    };

private Color32[] hairColors = new Color32[]
{
    new Color32(30, 20, 15, 255),   // Coklat sangat gelap / hampir hitam
    new Color32(60, 40, 30, 255),   // Coklat gelap
    new Color32(110, 70, 50, 255),  // Coklat medium
    new Color32(160, 120, 90, 255), // Coklat terang
    new Color32(210, 180, 140, 255),// Blonde hangat
    new Color32(200, 160, 130, 255),// Blonde soft
    new Color32(190, 90, 50, 255),  // Ginger / merah alami
    new Color32(150, 150, 150, 255),// Ash grey / abu-abu muda
    new Color32(180, 160, 140, 255) // Light ash brown
};


private Color32[] clothesColors = new Color32[]
{
    new Color32(180, 100, 100, 255), // merah kalem
    new Color32(100, 180, 140, 255), // hijau mint lembut
    new Color32(100, 120, 200, 255), // biru soft
    new Color32(200, 180, 120, 255), // kuning gading
    new Color32(150, 130, 180, 255), // ungu lembut
    new Color32(120, 180, 180, 255), // teal soft
    new Color32(200, 150, 130, 255), // oranye kalem
    new Color32(130, 130, 130, 255)  // abu-abu netral
};

    private bool ignoreDropdownSfx = true;
    private List<TMP_Dropdown> emptyDropdowns = new List<TMP_Dropdown>();

    private void Awake()
    {
        SetupDropdowns();
        SetupAppearanceDropdowns();
        SetupListeners();
        LoadSavedData();
        ignoreDropdownSfx = false;
        UpdateVisuals();
        UpdateRisk();
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

        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(new List<string> { "Perempuan", "Laki-laki" });

        hairDropdown.ClearOptions();
        hairDropdown.AddOptions(new List<string> { "Rambut 1", "Rambut 2", "Rambut 3" });

        clothesDropdown.ClearOptions();
        clothesDropdown.AddOptions(new List<string> { "Busana 1", "Busana 2", "Busana 3" });
    }

    private void SetupAppearanceDropdowns()
    {
        skinColorDropdown.ClearOptions();
        List<string> skinNames = new List<string>();
        for (int i = 0; i < skinColors.Length; i++) skinNames.Add("Warna Kulit " + (i + 1));
        skinColorDropdown.AddOptions(skinNames);

        hairColorDropdown.ClearOptions();
        List<string> hairNames = new List<string>();
        for (int i = 0; i < hairColors.Length; i++) hairNames.Add("Warna Rambut " + (i + 1));
        hairColorDropdown.AddOptions(hairNames);

        clothesColorDropdown.ClearOptions();
        List<string> clothesNames = new List<string>();
        for (int i = 0; i < clothesColors.Length; i++) clothesNames.Add("Warna Busana " + (i + 1));
        clothesColorDropdown.AddOptions(clothesNames);
    }

    private void SetupListeners()
    {
        TMP_Dropdown[] dropdowns =
        { ageDropdown, bmiDropdown, waistDropdown, activityDropdown, fruitDropdown,
          bpDropdown, glucoseDropdown, familyDropdown, genderDropdown,
          hairDropdown, clothesDropdown, skinColorDropdown, hairColorDropdown, clothesColorDropdown };

        foreach (var dd in dropdowns)
            if (dd != null) dd.onValueChanged.AddListener(_ => OnDropdownChanged());

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnDropdownChanged()
    {
        UpdateRisk();
        UpdateVisuals();

        if (!ignoreDropdownSfx && audioSource != null && dropdownSfx != null)
            audioSource.PlayOneShot(dropdownSfx);
    }

    private void UpdateVisuals()
    {
        if (visualManager == null) return;

        Patient temp = new Patient
        {
            gender = genderDropdown.value,
            age = MapAgeToDropdown(ageDropdown.value),
            bmi = MapBmiToDropdown(bmiDropdown.value),
            bodyType = MapBodyType(genderDropdown.value, bmiDropdown.value),
            skinColor = skinColors[skinColorDropdown.value],
            hairColor = hairColors[hairColorDropdown.value],
            clothesColor = clothesColors[clothesColorDropdown.value]
        };

        visualManager.SetupPatient(temp, hairDropdown.value, clothesDropdown.value);
    }

    private int MapBodyType(int genderValue, int bmiValue)
    {
        float bmi = MapBmiToDropdown(bmiValue);
        return bmi > 30f ? 1 : 0;
    }

private void OnSubmit()
{
    // 1. Simpan patient
    CustomPatientData data = new CustomPatientData
    {
        playerInitials = PlayerPrefs.GetString("PlayerInitials", "ANON"),
        patientName = nameInput.text,
        gender = genderDropdown.value,
        age = ageDropdown.value,
        bmi = bmiDropdown.value,
        waist = waistDropdown.value,
        activity = activityDropdown.value,
        fruit = fruitDropdown.value,
        bp = bpDropdown.value,
        glucose = glucoseDropdown.value,
        family = familyDropdown.value,
        skinColorIndex = skinColorDropdown.value,
        hairColorIndex = hairColorDropdown.value,
        clothesColorIndex = clothesColorDropdown.value,
        hairStyleIndex = hairDropdown.value,
        clothesStyleIndex = clothesDropdown.value
    };

    CustomPatientSaveSystem.Save(data);

    // 2. Hitung risk lagi
    int score = CalculateRisk();
    string riskLevel = Patient.CalculateRiskLevel(score);

    // 3. Simpan ke PlayerPrefs supaya MainMenuGreeting bisa baca
    PlayerPrefs.SetString("LastPatientName", data.patientName);
    PlayerPrefs.SetInt("LastRiskScore", score);
    PlayerPrefs.SetString("LastRiskLevel", riskLevel);
    PlayerPrefs.Save();

    // 4. Update greeting text
    FindObjectOfType<MainMenuGreeting>()?.UpdateGreeting();

    // 5. Update visual UI lokal
    UpdateRisk();
    UpdateVisuals();
}


    private void LoadSavedData()
    {
        string playerInitials = PlayerPrefs.GetString("PlayerInitials", "ANON");
        if (CustomPatientSaveSystem.HasData(playerInitials))
        {
            CustomPatientData data = CustomPatientSaveSystem.Load(playerInitials);
            if (data == null) return;

            nameInput.text = data.patientName;

            ageDropdown.SetValueWithoutNotify(data.age);
            bmiDropdown.SetValueWithoutNotify(data.bmi);
            waistDropdown.SetValueWithoutNotify(data.waist);
            activityDropdown.SetValueWithoutNotify(data.activity);
            fruitDropdown.SetValueWithoutNotify(data.fruit);
            bpDropdown.SetValueWithoutNotify(data.bp);
            glucoseDropdown.SetValueWithoutNotify(data.glucose);
            familyDropdown.SetValueWithoutNotify(data.family);

            genderDropdown.SetValueWithoutNotify(data.gender);

            skinColorDropdown.SetValueWithoutNotify(data.skinColorIndex);
            hairColorDropdown.SetValueWithoutNotify(data.hairColorIndex);
            clothesColorDropdown.SetValueWithoutNotify(data.clothesColorIndex);
            hairDropdown.SetValueWithoutNotify(data.hairStyleIndex);
clothesDropdown.SetValueWithoutNotify(data.clothesStyleIndex);

        }
    }

    private void UpdateRisk()
    {
        int score = CalculateRisk();
        string riskLevel = Patient.CalculateRiskLevel(score);
        string riskIndo = riskLevel switch
        {
            "Low" => "Rendah",
            "Slightly Elevated" => "Sedikit Meningkat",
            "Moderate" => "Sedang",
            "High" => "Tinggi",
            "Very High" => "Sangat Tinggi",
            _ => riskLevel
        };

        riskLevelText.text = riskIndo;
        riskLevelText.fontSize = riskIndo == "Sedikit Meningkat" ? 14 : 20;
        riskScoreText.text = score.ToString();
        riskScoreText.color = GetRiskColor(riskIndo);
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

    private int MapAgeToDropdown(int value) => value switch { 0 => 30, 1 => 50, 2 => 60, 3 => 70, _ => 30 };
    private float MapBmiToDropdown(int value) => value switch { 0 => 22f, 1 => 27f, 2 => 32f, _ => 22f };
    private float MapWaistToDropdown(int value) => value switch { 0 => 90f, 1 => 98f, 2 => 105f, _ => 90f };

    private Color GetRiskColor(string riskLevel) => riskLevel switch
    {
        "Rendah" => new Color(0.0f, 0.6f, 0.0f),
        "Sedikit Meningkat" => new Color(0.8f, 0.6f, 0.0f),
        "Sedang" => new Color(1.0f, 0.4f, 0.0f),
        "Tinggi" => new Color(0.8f, 0.0f, 0.0f),
        "Sangat Tinggi" => new Color(0.5f, 0.0f, 0.0f),
        _ => Color.white
    };
}

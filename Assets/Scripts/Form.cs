// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;

// public class Form : MonoBehaviour
// {
//     public static Form Instance;

//     [Header("UI References")]
//     public TextMeshProUGUI idText;
//     public TextMeshProUGUI nameText;
//     public TextMeshProUGUI riskLevelText;
//     public TextMeshProUGUI riskScoreText;
//     public TextMeshProUGUI patientInfoText;
//     public TextMeshProUGUI genderText;

//     [Header("Text Fields (replacing dropdowns)")]
//     public TextMeshProUGUI ageText;
//     public TextMeshProUGUI bmiText;
//     public TextMeshProUGUI waistText;
//     public TextMeshProUGUI activityText;
//     public TextMeshProUGUI fruitText;
//     public TextMeshProUGUI bpText;
//     public TextMeshProUGUI glucoseText;
//     public TextMeshProUGUI familyText;

//     [Header("Button")]
//     public Button submitButton;

//     [HideInInspector] public Patient currentPatient;

//     // internal indices
//     private int ageIndex = -1;
//     private int bmiIndex = -1;
//     private int waistIndex = -1;
//     private bool activityState = false;
//     private bool fruitState = false;
//     private bool bpState = false;
//     private bool glucoseState = false;
//     private int familyLevel = 0;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);

//         ResetFields();
//         if (submitButton != null)
//             submitButton.onClick.AddListener(OnSubmit);
//     }

//     public void ShowPatient(Patient p)
//     {
//         if (p == null) return;
//         currentPatient = p;

//         // tampilkan info pasien
//         if (idText != null) idText.text = $"ID: {p.patientID}";
//         if (nameText != null) nameText.text = $"Name: {p.patientName}";
//         if (genderText != null) genderText.text = $"Sex: {(p.gender == 0 ? "Female" : "Male")}";

//         if (patientInfoText != null)
//         {
//             patientInfoText.text =
//                 $"ID: {p.patientID}\n" +
//                 $"Name: {p.patientName}\n" +
//                 $"Gender: {(p.gender == 0 ? "Female" : "Male")}\n" +
//                 $"BodyType: {(p.bodyType == 0 ? "Normal" : "Overweight")}\n" +
//                 $"Skin: #{ColorUtility.ToHtmlStringRGB(p.skinColor)}";
//         }

//         ResetFields();
//         UpdateRiskFromIndex();
//     }

//     private void ResetFields()
//     {
//         ageIndex = -1;
//         bmiIndex = -1;
//         waistIndex = -1;
//         activityState = false;
//         fruitState = false;
//         bpState = false;
//         glucoseState = false;
//         familyLevel = 0;

//         ageText.text = "-";
//         bmiText.text = "-";
//         waistText.text = "-";
//         activityText.text = "-";
//         fruitText.text = "-";
//         bpText.text = "-";
//         glucoseText.text = "-";
//         familyText.text = "-";

//         if (riskScoreText != null) riskScoreText.text = "0";
//         if (riskLevelText != null) riskLevelText.text = "-";
//     }

//     private void OnSubmit()
//     {
//         if (currentPatient == null) return;
//         int score = CalculateScoreFromIndex();
//         GameManager.Instance.SubmitAnswer(score);
//     }

//     // --- Setters (dipanggil minigame) ---
//     public void SetAgeIndex(int index)
//     {
//         ageIndex = index;
//         ageText.text = index switch
//         {
//             0 => "<45",
//             1 => "45-54",
//             2 => "55-64",
//             3 => ">64",
//             _ => "-"
//         };
//         UpdateRiskFromIndex();
//     }

//     public void SetBMIIndex(int index)
//     {
//         bmiIndex = index;
//         bmiText.text = index switch
//         {
//             0 => "<25",
//             1 => "25-30",
//             2 => ">30",
//             _ => "-"
//         };
//         UpdateRiskFromIndex();
//     }

//     public void SetWaistIndex(int index)
//     {
//         waistIndex = index;
//         waistText.text = index switch
//         {
//             0 => "<94",
//             1 => "94-102",
//             2 => ">102",
//             _ => "-"
//         };
//         UpdateRiskFromIndex();
//     }

//     public void SetActivity(bool isActive)
//     {
//         activityState = isActive;
//         activityText.text = isActive ? "Active" : "Inactive";
//         UpdateRiskFromIndex();
//     }

//     public void SetFruit(bool sufficient)
//     {
//         fruitState = sufficient;
//         fruitText.text = sufficient ? "Sufficient" : "Insufficient";
//         UpdateRiskFromIndex();
//     }

//     public void SetBP(bool high)
//     {
//         bpState = high;
//         bpText.text = high ? "High" : "Normal";
//         UpdateRiskFromIndex();
//     }

//     public void SetGlucose(bool high)
//     {
//         glucoseState = high;
//         glucoseText.text = high ? "High" : "Normal";
//         UpdateRiskFromIndex();
//     }

//     public void SetFamily(int level)
//     {
//         familyLevel = level;
//         familyText.text = level switch
//         {
//             0 => "None",
//             1 => "Level1",
//             2 => "Level2",
//             _ => "-"
//         };
//         UpdateRiskFromIndex();
//     }

//     // --- Scoring ---
//     private void UpdateRiskFromIndex()
//     {
//         if (currentPatient == null) return;
//         int score = CalculateScoreFromIndex();

//         if (riskScoreText != null) riskScoreText.text = score.ToString();
//         if (riskLevelText != null) riskLevelText.text = Patient.CalculateRiskLevel(score);
//     }

//     private int CalculateScoreFromIndex()
//     {
//         int score = 0;
//         score += ageIndex switch { 0 => 0, 1 => 2, 2 => 3, 3 => 4, _ => 0 };
//         score += bmiIndex switch { 0 => 0, 1 => 1, 2 => 3, _ => 0 };
//         score += waistIndex switch { 0 => 0, 1 => 3, 2 => 4, _ => 0 };
//         score += activityState ? 0 : 2;
//         score += fruitState ? 0 : 1;
//         score += bpState ? 2 : 0;
//         score += glucoseState ? 5 : 0;
//         score += familyLevel switch { 1 => 3, 2 => 5, _ => 0 };
//         return score;
//     }
// }

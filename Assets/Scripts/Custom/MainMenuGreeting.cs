using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuGreeting : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI greetingText;
    public Image bubbleImage;
    public RectTransform bubblePanel;

    [Header("Animation")]
    public float animDuration = 0.5f;

    private Coroutine animRoutine;

    void Start()
    {
        RefreshGreeting();
    }

    private void RefreshGreeting()
    {
        string playerInitials = PlayerPrefs.GetString("PlayerInitials", "ANON");

        if (CustomPatientSaveSystem.HasData(playerInitials))
        {
            CustomPatientData data = CustomPatientSaveSystem.Load(playerInitials);
            if (data == null)
            {
                greetingText.text = "Halo Pemain. Silakan buat profil terlebih dahulu.";
                PlayAnim();
                return;
            }

            // Hitung ulang risk
            int score = CalculateRiskFromData(data);
            string riskLevel = Patient.CalculateRiskLevel(score);

            string riskIndo = riskLevel switch
            {
                "Low" => "rendah",
                "Slightly Elevated" => "sedikit meningkat",
                "Moderate" => "sedang",
                "High" => "tinggi",
                "Very High" => "sangat tinggi",
                _ => "tidak diketahui"
            };

            string suggestions = GetSuggestions(riskLevel);

            greetingText.text = $"Halo {data.patientName}. Risiko diabetesmu {riskIndo}. {suggestions}";
        }
        else
        {
            greetingText.text = "Halo Pemain. Silakan buat profil terlebih dahulu.";
        }

        PlayAnim();
    }

    private int CalculateRiskFromData(CustomPatientData data)
    {
        int score = 0;
        score += data.age switch { 0 => 0, 1 => 2, 2 => 3, 3 => 4, _ => 0 };
        score += data.bmi switch { 0 => 0, 1 => 1, 2 => 3, _ => 0 };
        score += data.waist switch { 0 => 0, 1 => 3, 2 => 4, _ => 0 };
        score += data.activity == 1 ? 2 : 0;
        score += data.fruit == 1 ? 1 : 0;
        score += data.bp == 1 ? 2 : 0;
        score += data.glucose == 1 ? 5 : 0;
        score += data.family switch { 1 => 3, 2 => 5, _ => 0 };
        return score;
    }

    private string GetSuggestions(string riskLevel)
    {
        return riskLevel switch
        {
            "Low" => "Keren! Terus jaga pola makan dan tetap aktif ya.",
            "Slightly Elevated" => "Kurangi gula, perbanyak buah, dan tetap gerak.",
            "Moderate" => "Coba konsultasi, jaga berat badan, dan makan lebih sehat.",
            "High" => "Sebaiknya periksa ke dokter dan mulai pola hidup seimbang.",
            "Very High" => "Segera konsultasi ke spesialis dan ikuti saran medis.",
            _ => "Tetap jaga kesehatanmu dengan pola hidup baik."
        };
    }

    // 🔹 Dipanggil dari CustomPatientUI setelah Save
    public void UpdateGreeting()
    {
        RefreshGreeting();
    }

    // ------------------ ANIMASI ------------------
    private void PlayAnim()
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimateBubble());
    }

    private IEnumerator AnimateBubble()
    {
        if (bubblePanel == null) yield break;

        Vector2 startPos = bubblePanel.anchoredPosition + new Vector2(0, -50f);
        Vector2 endPos = bubblePanel.anchoredPosition;
        float t = 0f;

        bubblePanel.anchoredPosition = startPos;
        SetAlpha(0f);

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;

            bubblePanel.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);
            SetAlpha(Mathf.SmoothStep(0f, 1f, lerp));

            yield return null;
        }

        bubblePanel.anchoredPosition = endPos;
        SetAlpha(1f);
    }

    private void SetAlpha(float a)
    {
        if (greetingText != null)
        {
            var c = greetingText.color;
            c.a = a;
            greetingText.color = c;
        }
        if (bubbleImage != null)
        {
            var c = bubbleImage.color;
            c.a = a;
            bubbleImage.color = c;
        }
    }
}

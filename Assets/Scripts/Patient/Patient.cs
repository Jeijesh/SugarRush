using UnityEngine;

public class Patient
{
    public string patientID;
    public string patientName;
    public int age;
    public float bmi;
    public float waist;
    public int activity;
    public int fruit;
    public int bp;
    public int glucose;
    public int family;
    public int gender;
    public int bodyType;
    public Color skinColor;
    public Color hairColor;
    public Color clothesColor;
    public string riskLevel;

    // Hitung risk level string & skor total
    public void CalculateRiskScore()
    {
        int score = 0;
        // Age
        if (age < 45) score += 0;
        else if (age <= 54) score += 2;
        else if (age <= 64) score += 3;
        else score += 4;

        // BMI
        if (bmi < 25) score += 0;
        else if (bmi <= 30) score += 1;
        else score += 3;

        // Waist
        if (waist < 94) score += 0;
        else if (waist <= 102) score += 3;
        else score += 4;

        // Activity
        if (activity == 0) score += 2;

        // Fruit
        if (fruit == 0) score += 1;

        // BP
        if (bp == 1) score += 2;

        // Glucose
        if (glucose == 1) score += 5;

        // Family
        if (family == 1) score += 3;
        else if (family == 2) score += 5;

        // Tentukan risk level
        riskLevel = GetRiskLevel(score);
    }

    private string GetRiskLevel(int score)
    {
        if (score < 7) return "Low";
        else if (score <= 11) return "Slightly Elevated";
        else if (score <= 15) return "Moderate";
        else if (score <= 20) return "High";
        else return "Very High";
    }

    // Ambil skor numeric total untuk delta
    public int GetTotalScore()
    {
        int score = 0;
        score += age < 45 ? 0 : age <= 54 ? 2 : age <= 64 ? 3 : 4;
        score += bmi < 25 ? 0 : bmi <= 30 ? 1 : 3;
        score += waist < 94 ? 0 : waist <= 102 ? 3 : 4;
        score += activity == 0 ? 2 : 0;
        score += fruit == 0 ? 1 : 0;
        score += bp == 1 ? 2 : 0;
        score += glucose == 1 ? 5 : 0;
        score += family == 1 ? 3 : family == 2 ? 5 : 0;
        return score;
    }

    // Static untuk dropdown / user submit mapping
    public static string CalculateRiskLevel(int score)
    {
        if (score < 7) return "Low";
        else if (score <= 11) return "Slightly Elevated";
        else if (score <= 15) return "Moderate";
        else if (score <= 20) return "High";
        else return "Very High";
    }
}

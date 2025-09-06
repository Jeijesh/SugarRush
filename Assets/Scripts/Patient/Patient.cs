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

    public void CalculateRiskScore()
    {
        int score = 0;
        score += age < 45 ? 0 : age < 55 ? 2 : age < 65 ? 3 : 4;
        score += bmi < 25 ? 0 : bmi < 30 ? 1 : 3;
        score += waist < 94 ? 0 : waist <= 102 ? 3 : 4;
        score += activity == 1 ? 2 : 0;
        score += fruit == 1 ? 1 : 0;
        score += bp == 1 ? 2 : 0;
        score += glucose == 1 ? 5 : 0;
        score += family switch { 1 => 3, 2 => 5, _ => 0 };
        riskLevel = CalculateRiskLevel(score);
    }

    public int GetNumericRisk()
    {
        return riskLevel switch { "Low"=>1, "Medium"=>2, "High"=>3, _=>0 };
    }

    public static string CalculateRiskLevel(int score)
    {
        if (score <= 5) return "Low";
        if (score <= 10) return "Medium";
        return "High";
    }

    public static int ConvertRiskToNumeric(string risk)
    {
        return risk switch { "Low"=>1, "Medium"=>2, "High"=>3, _=>0 };
    }
}

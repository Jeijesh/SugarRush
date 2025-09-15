using UnityEngine;

[System.Serializable]
public class CustomPatientData
{
    public string playerInitials;
    public string patientName;
    public int gender;
    public int age;
    public int bmi;
    public int waist;
    public int activity;
    public int fruit;
    public int bp;
    public int glucose;
    public int family;

    public int skinColorIndex;
    public int hairStyleIndex;
    public int hairColorIndex;
    public int clothesStyleIndex;
    public int clothesColorIndex;
    
}

public static class CustomPatientSaveSystem
{
    private static string GetSaveKey(string playerInitials)
    {
        return "CUSTOM_PATIENT_" + playerInitials.ToUpper();
    }

    public static void Save(CustomPatientData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GetSaveKey(data.playerInitials), json);
        PlayerPrefs.Save();
    }

    public static CustomPatientData Load(string playerInitials)
    {
        string key = GetSaveKey(playerInitials);
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<CustomPatientData>(json);
        }
        return null;
    }

    public static bool HasData(string playerInitials)
    {
        return PlayerPrefs.HasKey(GetSaveKey(playerInitials));
    }
}

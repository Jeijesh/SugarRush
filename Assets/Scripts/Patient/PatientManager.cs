using UnityEngine;

public class PatientManager : MonoBehaviour
{
    private Color[] skinTones = { new Color32(255,224,189,255), new Color32(241,194,125,255), new Color32(224,172,105,255), new Color32(198,134,66,255), new Color32(141,85,36,255), new Color32(92,60,30,255) };
    private Color[] youngHairColors = { Color.black, new Color32(139,69,19,255), new Color32(255,215,0,255), new Color32(210,180,140,255) };
    private Color[] oldHairColors = { Color.white, Color.grey };
    private Color[] clothesColors = { new Color32(102,153,204,255), new Color32(119,178,117,255), new Color32(178,102,102,255), new Color32(204,153,102,255), new Color32(153,119,178,255), new Color32(229,229,153,255) };

    private string[] femaleNames = { "Dewi","Ayu","Indah","Siti","Ratna","Lestari","Anita","Fitri","Rina","Yulia" };
    private string[] maleNames = { "Andi","Budi","Eko","Ilham","Agus","Bayu","Fajar","Rizki","Adi","Arif" };
    private string[] lastNames = { "Hidayat","Santoso","Wijaya","Pratama","Wibowo","Cahya","Purnama","Anggara","Permana","Lesmana" };

    public Patient GeneratePatient()
    {
        Patient p = new Patient
        {
            age = Random.Range(18, 81),
            bmi = Random.Range(18f, 36f),
            waist = Random.Range(70f, 120f),
            activity = Random.Range(0,2),
            fruit = Random.Range(0,2),
            bp = Random.Range(0,2),
            glucose = Random.Range(0,2),
            family = Random.Range(0,3),
            gender = Random.Range(0,2),
            bodyType = Random.Range(0,2),
            skinColor = skinTones[Random.Range(0, skinTones.Length)],
            hairColor = (Random.Range(0,2) == 0) ? youngHairColors[Random.Range(0, youngHairColors.Length)] : oldHairColors[Random.Range(0, oldHairColors.Length)],
            clothesColor = clothesColors[Random.Range(0, clothesColors.Length)]
        };

        p.CalculateRiskScore();
        p.patientID = "P-" + Random.Range(10000, 99999);
        string first = p.gender == 1 ? maleNames[Random.Range(0,maleNames.Length)] : femaleNames[Random.Range(0,femaleNames.Length)];
        string last = lastNames[Random.Range(0,lastNames.Length)];
        p.patientName = $"{first} {last}";

        return p;
    }
}

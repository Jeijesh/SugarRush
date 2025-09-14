using UnityEngine;

public class PatientManager : MonoBehaviour
{
    private Color[] skinTones = {
        new Color32(255,224,189,255),
        new Color32(241,194,125,255),
        new Color32(224,172,105,255),
        new Color32(198,134,66,255),
        new Color32(141,85,36,255),
        new Color32(92,60,30,255)
    };

    // Tambah warna rambut muda & tua
    private Color[] youngHairColors = {
        new Color32(20,20,20,255),      // hitam
        new Color32(90,50,30,255),      // coklat gelap
        new Color32(184,134,11,255),    // coklat terang
        new Color32(160,130,100,255),   // coklat pasir
        new Color32(255,215,0,255),     // pirang muda
        new Color32(150,75,0,255)       // coklat kemerahan
    };
    private Color[] oldHairColors = {
        new Color32(230,230,230,255),   // abu-abu terang
        new Color32(180,180,180,255),   // abu-abu
        new Color32(150,150,150,255),   // abu gelap
        new Color32(200,180,160,255)    // coklat pucat
    };

    private Color[] clothesColors = {
        new Color32(102,153,204,255),
        new Color32(119,178,117,255),
        new Color32(178,102,102,255),
        new Color32(204,153,102,255),
        new Color32(153,119,178,255),
        new Color32(229,229,153,255),
        new Color32(255,182,193,255),   // pink
        new Color32(135,206,235,255)    // sky blue
    };

    // Tambah nama depan
    private string[] femaleNames = {
        "Dewi","Ayu","Indah","Siti","Ratna","Lestari","Anita","Fitri","Rina","Yulia",
        "Nina","Maya","Putri","Citra","Tasya","Vina","Lina","Rika","Sasha","Reni"
    };
    private string[] maleNames = {
        "Andi","Budi","Eko","Ilham","Agus","Bayu","Fajar","Rizki","Adi","Arif",
        "Rian","Dimas","Hendra","Galang","Rudy","Fauzi","Tono","Yuda","Bagus","Iqbal"
    };

    // Tambah last name
    private string[] lastNames = {
        "Hidayat","Santoso","Wijaya","Pratama","Wibowo","Cahya","Purnama","Anggara","Permana","Lesmana",
        "Saputra","Alam","Ramadhan","Utama","Setiawan","Gunawan","Prasetyo","Kusuma","Rahman","Nugroho"
    };

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
// Di PatientManager.GeneratePatient()
if (p.age < 45) 
    p.hairColor = youngHairColors[Random.Range(0, youngHairColors.Length)];
else 
    p.hairColor = oldHairColors[Random.Range(0, oldHairColors.Length)];

        p.CalculateRiskScore();
        p.patientID = "P-" + Random.Range(10000, 99999);
        string first = p.gender == 1 ? maleNames[Random.Range(0,maleNames.Length)] : femaleNames[Random.Range(0,femaleNames.Length)];
        string last = lastNames[Random.Range(0,lastNames.Length)];
        p.patientName = $"{first} {last}";

        return p;
    }
}

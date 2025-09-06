using UnityEngine;

public class PatientVisualManager : MonoBehaviour
{
    [Header("Body Sprites")]
    public Sprite normalWoman;
    public Sprite overweightWoman;
    public Sprite normalMan;
    public Sprite overweightMan;

    [Header("Face Sprites")]
    public Sprite oldWoman;
    public Sprite youngWoman;
    public Sprite oldMan;
    public Sprite youngMan;

    [Header("Hair Sprites")]
    public Sprite hair1Woman;
    public Sprite hair2Woman;
    public Sprite hair3Woman;
    public Sprite hair1Man;
    public Sprite hair2Man;
    public Sprite hair3Man;

    [Header("Clothes Sprites - Woman Normal")]
    public Sprite clothes1WomanNormal;
    public Sprite clothes2WomanNormal;
    public Sprite clothes3WomanNormal;

    [Header("Clothes Sprites - Woman Overweight")]
    public Sprite clothes1WomanOverweight;
    public Sprite clothes2WomanOverweight;
    public Sprite clothes3WomanOverweight;

    [Header("Clothes Sprites - Man Normal")]
    public Sprite clothes1ManNormal;
    public Sprite clothes2ManNormal;
    public Sprite clothes3ManNormal;

    [Header("Clothes Sprites - Man Overweight")]
    public Sprite clothes1ManOverweight;
    public Sprite clothes2ManOverweight;
    public Sprite clothes3ManOverweight;

    [Header("Target Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer faceRenderer;
    public SpriteRenderer hairRenderer;
    public SpriteRenderer clothesRenderer;

    public void SetupPatient(Patient patient)
    {
        bool isMale = patient.gender == 1;
        bool isYoung = patient.age < 45;
        bool isOverweight = patient.bodyType == 1;

        // Body
        if (!isMale) bodyRenderer.sprite = isOverweight ? overweightWoman : normalWoman;
        else bodyRenderer.sprite = isOverweight ? overweightMan : normalMan;
        bodyRenderer.color = patient.skinColor;

        // Face
        if (!isMale) faceRenderer.sprite = isYoung ? youngWoman : oldWoman;
        else faceRenderer.sprite = isYoung ? youngMan : oldMan;
        faceRenderer.color = patient.skinColor;

        // Hair
        Sprite[] hairOptions = isMale ? 
            new Sprite[] { hair1Man, hair2Man, hair3Man } : 
            new Sprite[] { hair1Woman, hair2Woman, hair3Woman };
        hairRenderer.sprite = hairOptions[Random.Range(0, hairOptions.Length)];
        hairRenderer.color = patient.hairColor;

        // Clothes
        Sprite[] clothesOptions;
        if (!isMale && !isOverweight)
            clothesOptions = new Sprite[] { clothes1WomanNormal, clothes2WomanNormal, clothes3WomanNormal };
        else if (!isMale && isOverweight)
            clothesOptions = new Sprite[] { clothes1WomanOverweight, clothes2WomanOverweight, clothes3WomanOverweight };
        else if (isMale && !isOverweight)
            clothesOptions = new Sprite[] { clothes1ManNormal, clothes2ManNormal, clothes3ManNormal };
        else
            clothesOptions = new Sprite[] { clothes1ManOverweight, clothes2ManOverweight, clothes3ManOverweight };

        clothesRenderer.sprite = clothesOptions[Random.Range(0, clothesOptions.Length)];

        // Ambil warna pakaian langsung dari PatientManager (clothesColor)
        clothesRenderer.color = patient.clothesColor;
    }
}

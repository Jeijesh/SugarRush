using UnityEngine;

public class PatientVisualManager : MonoBehaviour
{
    private Vector3 defaultHairPos;
    private Vector3 defaultClothesPos;

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

    [Header("Hair Sprites + Offsets (Woman)")]
    public Sprite hair1Woman;
    public Vector2 hair1WomanOffset;
    public Sprite hair2Woman;
    public Vector2 hair2WomanOffset;
    public Sprite hair3Woman;
    public Vector2 hair3WomanOffset;

    [Header("Hair Sprites + Offsets (Man)")]
    public Sprite hair1Man;
    public Vector2 hair1ManOffset;
    public Sprite hair2Man;
    public Vector2 hair2ManOffset;
    public Sprite hair3Man;
    public Vector2 hair3ManOffset;

    [Header("Clothes Sprites + Offsets - Woman Normal")]
    public Sprite clothes1WomanNormal;
    public Vector2 clothes1WomanNormalOffset;
    public Sprite clothes2WomanNormal;
    public Vector2 clothes2WomanNormalOffset;
    public Sprite clothes3WomanNormal;
    public Vector2 clothes3WomanNormalOffset;

    [Header("Clothes Sprites + Offsets - Woman Overweight")]
    public Sprite clothes1WomanOverweight;
    public Vector2 clothes1WomanOverweightOffset;
    public Sprite clothes2WomanOverweight;
    public Vector2 clothes2WomanOverweightOffset;
    public Sprite clothes3WomanOverweight;
    public Vector2 clothes3WomanOverweightOffset;

    [Header("Clothes Sprites + Offsets - Man Normal")]
    public Sprite clothes1ManNormal;
    public Vector2 clothes1ManNormalOffset;
    public Sprite clothes2ManNormal;
    public Vector2 clothes2ManNormalOffset;
    public Sprite clothes3ManNormal;
    public Vector2 clothes3ManNormalOffset;

    [Header("Clothes Sprites + Offsets - Man Overweight")]
    public Sprite clothes1ManOverweight;
    public Vector2 clothes1ManOverweightOffset;
    public Sprite clothes2ManOverweight;
    public Vector2 clothes2ManOverweightOffset;
    public Sprite clothes3ManOverweight;
    public Vector2 clothes3ManOverweightOffset;

    [Header("Target Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer faceRenderer;
    public SpriteRenderer hairRenderer;
    public SpriteRenderer clothesRenderer;

    private void Awake()
    {
        // simpan posisi default sekali saja
        defaultHairPos = hairRenderer.transform.localPosition;
        defaultClothesPos = clothesRenderer.transform.localPosition;
    }

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
        Sprite chosenHair;
        Vector2 chosenHairOffset = Vector2.zero;
        if (isMale)
        {
            int index = Random.Range(0, 3);
            if (index == 0) { chosenHair = hair1Man; chosenHairOffset = hair1ManOffset; }
            else if (index == 1) { chosenHair = hair2Man; chosenHairOffset = hair2ManOffset; }
            else { chosenHair = hair3Man; chosenHairOffset = hair3ManOffset; }
        }
        else
        {
            int index = Random.Range(0, 3);
            if (index == 0) { chosenHair = hair1Woman; chosenHairOffset = hair1WomanOffset; }
            else if (index == 1) { chosenHair = hair2Woman; chosenHairOffset = hair2WomanOffset; }
            else { chosenHair = hair3Woman; chosenHairOffset = hair3WomanOffset; }
        }
        hairRenderer.sprite = chosenHair;
        hairRenderer.color = patient.hairColor;
        hairRenderer.transform.localPosition = defaultHairPos + (Vector3)chosenHairOffset;

        // Clothes
        Sprite[] clothesOptions;
        Vector2[] clothesOffsets;
        if (!isMale && !isOverweight)
        {
            clothesOptions = new Sprite[] { clothes1WomanNormal, clothes2WomanNormal, clothes3WomanNormal };
            clothesOffsets = new Vector2[] { clothes1WomanNormalOffset, clothes2WomanNormalOffset, clothes3WomanNormalOffset };
        }
        else if (!isMale && isOverweight)
        {
            clothesOptions = new Sprite[] { clothes1WomanOverweight, clothes2WomanOverweight, clothes3WomanOverweight };
            clothesOffsets = new Vector2[] { clothes1WomanOverweightOffset, clothes2WomanOverweightOffset, clothes3WomanOverweightOffset };
        }
        else if (isMale && !isOverweight)
        {
            clothesOptions = new Sprite[] { clothes1ManNormal, clothes2ManNormal, clothes3ManNormal };
            clothesOffsets = new Vector2[] { clothes1ManNormalOffset, clothes2ManNormalOffset, clothes3ManNormalOffset };
        }
        else
        {
            clothesOptions = new Sprite[] { clothes1ManOverweight, clothes2ManOverweight, clothes3ManOverweight };
            clothesOffsets = new Vector2[] { clothes1ManOverweightOffset, clothes2ManOverweightOffset, clothes3ManOverweightOffset };
        }

        int clothesIndex = Random.Range(0, clothesOptions.Length);
        clothesRenderer.sprite = clothesOptions[clothesIndex];
        clothesRenderer.color = patient.clothesColor;
        clothesRenderer.transform.localPosition = defaultClothesPos + (Vector3)clothesOffsets[clothesIndex];
    }
}

using UnityEngine;

public class CustomPatientVisualManager : MonoBehaviour
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
        defaultHairPos = hairRenderer.transform.localPosition;
        defaultClothesPos = clothesRenderer.transform.localPosition;
    }

    public void SetupPatient(Patient patient, int hairIndex, int clothesIndex)
    {
        bool isMale = patient.gender == 1;
        bool isYoung = patient.age < 45;
        bool isOverweight = patient.bodyType == 1;

        // Body
        bodyRenderer.sprite = !isMale ? (isOverweight ? overweightWoman : normalWoman) : (isOverweight ? overweightMan : normalMan);
        bodyRenderer.color = patient.skinColor;

        // Face
        faceRenderer.sprite = !isMale ? (isYoung ? youngWoman : oldWoman) : (isYoung ? youngMan : oldMan);
        faceRenderer.color = patient.skinColor;

        // Hair
        Sprite chosenHair;
        Vector2 chosenHairOffset = Vector2.zero;

        if (isMale)
        {
            chosenHair = hairIndex switch
            {
                0 => hair1Man,
                1 => hair2Man,
                _ => hair3Man
            };
            chosenHairOffset = hairIndex switch
            {
                0 => hair1ManOffset,
                1 => hair2ManOffset,
                _ => hair3ManOffset
            };
        }
        else
        {
            chosenHair = hairIndex switch
            {
                0 => hair1Woman,
                1 => hair2Woman,
                _ => hair3Woman
            };
            chosenHairOffset = hairIndex switch
            {
                0 => hair1WomanOffset,
                1 => hair2WomanOffset,
                _ => hair3WomanOffset
            };
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

        int cIndex = Mathf.Clamp(clothesIndex, 0, clothesOptions.Length - 1);
        clothesRenderer.sprite = clothesOptions[cIndex];
        clothesRenderer.color = patient.clothesColor;
        clothesRenderer.transform.localPosition = defaultClothesPos + (Vector3)clothesOffsets[cIndex];
    }

    public void ResetVisuals()
    {
        bodyRenderer.sprite = null;
        faceRenderer.sprite = null;
        hairRenderer.sprite = null;
        clothesRenderer.sprite = null;
        hairRenderer.transform.localPosition = defaultHairPos;
        clothesRenderer.transform.localPosition = defaultClothesPos;
    }
}

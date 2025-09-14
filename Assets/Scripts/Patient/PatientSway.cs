using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PatientSway : MonoBehaviour
{
    [Header("Rotate Sway")]
    public float rotateAmount = 3f;
    public float rotateSpeed = 1f;

    [Header("Position Sway")]
    public Vector3 positionSwayAmount = new Vector3(0.02f, 0.05f, 0);
    public float positionSwaySpeed = 1f;

    [Header("Popup Fade Up")]
    public float fadeDuration = 0.5f;
    public float popupOffsetY = 50f; // seberapa tinggi popup naik

    private Vector3 initialPos;
    private float rotatePhase;
    private float positionPhase;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        initialPos = transform.localPosition;
        rotatePhase = Random.Range(0f, Mathf.PI * 2);
        positionPhase = Random.Range(0f, Mathf.PI * 2);

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // mulai dari transparan

        // mulai animasi popup
        StartCoroutine(PopupFadeUp());
    }

    private void Update()
    {
        // Rotasi sway
        float angle = Mathf.Sin(Time.time * rotateSpeed + rotatePhase) * rotateAmount;
        transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        // Posisi sway
        Vector3 offset = new Vector3(
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase) * positionSwayAmount.x,
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase * 1.5f) * positionSwayAmount.y,
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase * 0.7f) * positionSwayAmount.z
        );

        transform.localPosition = initialPos + offset;
    }

    private System.Collections.IEnumerator PopupFadeUp()
    {
        Vector3 startPos = initialPos - new Vector3(0, popupOffsetY, 0);
        Vector3 endPos = initialPos;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            // Fade in alpha
            canvasGroup.alpha = t;

            // Naik ke posisi akhir
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localPosition = endPos;
    }
}

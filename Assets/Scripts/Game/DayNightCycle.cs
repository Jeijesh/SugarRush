using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle2D_BrighterDay : MonoBehaviour
{
    [Header("Overlay")]
    public SpriteRenderer overlaySprite;

    [Header("Global Light")]
    public Light2D globalLight;

    [Header("Sun Light")]
    public Light2D sunLight;

    [Header("Cycle Settings")]
    public float maxTime = 180f; 
    public Vector2 sunStartPos = new Vector2(-10f, 5f);
    public Vector2 sunEndPos = new Vector2(10f, 5f);

    [Header("Timer")]
    public float remainingTime;

    private void Start()
    {
        remainingTime = maxTime;
        UpdateCycle();
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Clamp(remainingTime, 0f, maxTime);
        UpdateCycle();
    }

    private void UpdateCycle()
    {
        float t = remainingTime / maxTime; // 1 = pagi, 0 = malam

        // -------------------
        // Overlay alpha
        // -------------------
        if (overlaySprite != null)
        {
            Color c = overlaySprite.color;
            c.a = 1f - t;
            overlaySprite.color = c;
        }

        // -------------------
        // Hitung color segment
        // -------------------
        Color dayColor;
        if (t > 0.75f) // pagi → siang
        {
            float segmentT = (t - 0.75f) / 0.25f; // 0→1
            dayColor = Color.Lerp(new Color(0.5f, 0.7f, 1f), new Color(1f, 1f, 0.8f), 1f - segmentT); // siang lebih cerah
        }
        else if (t > 0.5f) // siang → sore
        {
            float segmentT = (t - 0.5f) / 0.25f;
            dayColor = Color.Lerp(new Color(1f, 1f, 0.8f), new Color(1f, 0.6f, 0.2f), 1f - segmentT); // transisi ke sore
        }
        else if (t > 0.25f) // sore → malam
        {
            float segmentT = (t - 0.25f) / 0.25f;
            dayColor = Color.Lerp(new Color(1f,0.6f,0.2f), new Color(0.2f,0.2f,0.6f), 1f - segmentT);
        }
        else // malam
        {
            dayColor = new Color(0.2f, 0.2f, 0.6f);
        }

        // -------------------
        // Global Light
        // -------------------
        if (globalLight != null)
        {
            globalLight.color = dayColor;
            globalLight.intensity = Mathf.Lerp(0.2f, 1.2f, t); // intensitas siang lebih tinggi
        }

        // -------------------
        // Sun / Point Light
        // -------------------
        if (sunLight != null)
        {
            sunLight.transform.position = Vector2.Lerp(sunEndPos, sunStartPos, t);
            sunLight.color = dayColor;
            sunLight.intensity = Mathf.Lerp(0.5f, 1.2f, t); // sun lebih terang di siang
        }
    }

    public void ResetCycle()
    {
        remainingTime = maxTime;
    }
}

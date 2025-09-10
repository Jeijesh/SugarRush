using UnityEngine;

public class SkyLoopContinuousRight : MonoBehaviour
{
    [Header("Sky Settings")]
    public Transform sky1;
    public Transform sky2;
    public float totalTime = 180f; // durasi penuh untuk 1 lebar sprite

    private float spriteWidth;
    private float elapsedTime = 0f;

    private void Start()
    {
        if (sky1 == null || sky2 == null)
        {
            Debug.LogError("Assign sky1 dan sky2!");
            return;
        }

        SpriteRenderer sr = sky1.GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteWidth = sr.bounds.size.x;
        else
            spriteWidth = 20f;

        // Tempatkan sky2 di kiri sky1
        sky2.position = new Vector3(sky1.position.x - spriteWidth, sky2.position.y, sky2.position.z);
    }

    private void Update()
    {
        if (elapsedTime >= totalTime) return;

        float t = Time.deltaTime / totalTime; // proporsi per frame
        float moveAmount = spriteWidth * t;

        // geser kedua sprite ke kanan
        sky1.position += Vector3.right * moveAmount;
        sky2.position += Vector3.right * moveAmount;

        // offset modulo untuk continuous loop
        Vector3 p1 = sky1.position;
        Vector3 p2 = sky2.position;

        if (p1.x >= spriteWidth)
            p1.x -= 2 * spriteWidth;
        if (p2.x >= spriteWidth)
            p2.x -= 2 * spriteWidth;

        sky1.position = p1;
        sky2.position = p2;

        elapsedTime += Time.deltaTime;
    }

    public void ResetLoop()
    {
        elapsedTime = 0f;
        sky1.localPosition = Vector3.zero;
        sky2.localPosition = new Vector3(-spriteWidth, 0, 0);
    }
}

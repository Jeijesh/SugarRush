using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirtWipeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float wipeSpeed = 1f;
    private Image img;
    private bool wiping = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip wipeSfx;
    [Range(0.9f, 1.1f)] public float minPitch = 0.95f;
    [Range(0.9f, 1.1f)] public float maxPitch = 1.05f;

    public System.Action onCleaned; // callback ke panel

    private void Awake()
    {
        img = GetComponent<Image>();
        if (img != null)
            img.material = new Material(img.materialForRendering); // unik per dirt

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wiping = true;
        if (audioSource != null && wipeSfx != null)
        {
            audioSource.clip = wipeSfx;
            audioSource.loop = true;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        wiping = false;
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!wiping || img == null) return;

        float dragMagnitude = eventData.delta.magnitude;

        float alphaReduction = (wipeSpeed * 0.5f * Time.deltaTime)
                             + (dragMagnitude * wipeSpeed * 0.05f * Time.deltaTime);

        alphaReduction = Mathf.Min(alphaReduction, img.color.a);

        Color c = img.color;
        c.a -= alphaReduction;
        img.color = c;

        if (c.a <= 0f)
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            onCleaned?.Invoke();

            Destroy(gameObject); // hanya prefab dirt yang di-destroy
        }
    }
}

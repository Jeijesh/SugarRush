using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirtWipeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float wipeSpeed = 1f;
    private Image img;
    private bool wiping = false;

    [Header("Audio")]
    public AudioSource audioSource;    // drag AudioSource di Inspector
    public AudioClip wipeSfx;          // drag SFX gesekan di Inspector
    [Range(0.9f, 1.1f)] public float minPitch = 0.95f;
    [Range(0.9f, 1.1f)] public float maxPitch = 1.05f;

    private void Awake()
    {
        img = GetComponent<Image>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false; // pastikan tidak bunyi saat start
            audioSource.loop = false;        // default non-loop, kita kontrol manual
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wiping = true;

        if (audioSource != null && wipeSfx != null)
        {
            audioSource.clip = wipeSfx;
            audioSource.loop = true;
            audioSource.pitch = Random.Range(minPitch, maxPitch); // variasi pitch
            audioSource.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        wiping = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (wiping && img != null)
        {
            Color c = img.color;
            c.a -= wipeSpeed * Time.deltaTime;
            img.color = c;

            if (c.a <= 0)
            {
                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();

                Destroy(gameObject);
            }
        }
    }
}

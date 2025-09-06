using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirtWipeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float wipeSpeed = 1f;
    private Image img;
    private bool wiping = false;

    private void Awake() => img = GetComponent<Image>();

    public void OnPointerDown(PointerEventData eventData) => wiping = true;
    public void OnPointerUp(PointerEventData eventData) => wiping = false;

    public void OnDrag(PointerEventData eventData)
    {
        if(wiping && img!=null)
        {
            Color c = img.color;
            c.a -= wipeSpeed * Time.deltaTime;
            img.color = c;
            if(c.a<=0) Destroy(gameObject);
        }
    }
}

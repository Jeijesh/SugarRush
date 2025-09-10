using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IDCardPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText, idText, ageText;
    public GameObject panel;
    public Button closeButton;

    [Header("Dirt Settings")]
    public GameObject dirtPrefab;
    public int dirtCount = 5;
    public RectTransform dirtParent;

    private Patient lastPatient;

    private void Start()
    {
        if(closeButton!=null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    private void Update()
    {
        if(PatientUI.Instance!=null && PatientUI.Instance.currentPatient!=null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if(p!=lastPatient)
            {
                SpawnDirt();
                lastPatient = p;
            }

            if(nameText!=null) nameText.text = $"Name: {p.patientName}";
            if(idText!=null) idText.text = $"ID: {p.patientID}";
            if(ageText!=null) ageText.text = $"Age: {p.age}";
        }

        if(Input.GetKeyDown(KeyCode.Escape) && panel.activeSelf)
            ClosePanel();
    }

    public void ClosePanel() => panel.SetActive(false);

private void SpawnDirt()
{
    if (dirtPrefab == null || dirtParent == null) return;

    // hapus sisa dirt lama
    foreach (Transform child in dirtParent) Destroy(child.gameObject);

    // spawn dirt random
    for (int i = 0; i < dirtCount; i++)
    {
        GameObject dirt = Instantiate(dirtPrefab, dirtParent);
        RectTransform rt = dirt.GetComponent<RectTransform>();

        // posisi random
        rt.anchoredPosition = new Vector2(
            Random.Range(-dirtParent.rect.width / 2, dirtParent.rect.width / 2),
            Random.Range(-dirtParent.rect.height / 2, dirtParent.rect.height / 2)
        );

        // rotasi random kecil biar variasi
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
    }

    // tambahkan 1 dirt di kanan atas
    GameObject topRightDirt = Instantiate(dirtPrefab, dirtParent);
    RectTransform tr = topRightDirt.GetComponent<RectTransform>();

    // kanan atas relatif terhadap rect
    tr.anchoredPosition = new Vector2(
        dirtParent.rect.width / 2 - 50f,   // offset sedikit biar nggak mepet banget
        dirtParent.rect.height / 2 - 50f
    );

    // rotasi acak juga
    tr.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
}

}

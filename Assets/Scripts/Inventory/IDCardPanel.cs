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

    [Header("Game Settings")]
    public float gameDuration = 5f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failedText;

    private Patient lastPatient;
    private float timer;
    private bool gameActive;
    private int totalDirt;
    private int cleanedDirt;
    private bool needsSpawn = false;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);
    }

private void Update()
{
    if (PatientUI.Instance == null || PatientUI.Instance.currentPatient == null)
        return;

    Patient p = PatientUI.Instance.currentPatient;

    // pasien baru
    if (p != lastPatient)
    {
        lastPatient = p;
        needsSpawn = true;
    }

    if (needsSpawn)
    {
        ResetPanel();
        SpawnDirt();
        needsSpawn = false;
    }

    // update info pasien
    if (nameText != null) nameText.text = $"{p.patientName}";
    if (idText != null) idText.text = $"{p.patientID}";
    if (ageText != null) ageText.text = $"Umur: {p.age}";

    // tutup panel dengan Escape
    if (Input.GetKeyDown(KeyCode.Escape) && panel.activeSelf)
        ClosePanel();

    if (!gameActive) return;

    // update timer
    timer -= Time.deltaTime;
    if (timerText != null)
        timerText.text = $"Waktu: {Mathf.CeilToInt(timer)}";

    // cek jumlah dirt tersisa di parent
    int currentDirt = dirtParent != null ? dirtParent.childCount : 0;

    // jika semua dirt hilang, progress langsung 100%
    float progress = (totalDirt <= 0) ? 1f : (float)cleanedDirt / totalDirt;
    if (currentDirt == 0)
        progress = 1f;

    if (progressText != null)
        progressText.text = $"Progres: {(progress * 100f):F0}%";

    // end game otomatis jika progress >= 90%
    if (progress >= 0.9f)
        EndGame(true);
    else if (timer <= 0f)
        EndGame(false);
}


    private void EndGame(bool success)
    {
        gameActive = false;

        if (success)
        {
            if (successText != null) successText.gameObject.SetActive(true);
            if (failedText != null) failedText.gameObject.SetActive(false);

            if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
            {
                Patient p = PatientUI.Instance.currentPatient;
                PatientUI.Instance.FillField("Age");
                PatientUI.Instance.RefreshDropdowns(p);
            }

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddGameResult(true);
        }
        else
        {
            if (successText != null) successText.gameObject.SetActive(false);
            if (failedText != null) failedText.gameObject.SetActive(true);

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddGameResult(false);
        }
    }

    private void ResetPanel()
    {
        timer = gameDuration;
        gameActive = true;
        cleanedDirt = 0;

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.text = $"Waktu: {Mathf.CeilToInt(timer)}";
        if (progressText != null)
            progressText.text = "Progres: 0%";
    }

    // 🔹 self-healing dirtParent
    private void EnsureDirtParent()
    {
        if (dirtParent != null) return;

        GameObject panelGO = panel != null ? panel : GameObject.Find("IDCardPanel");
        if (panelGO != null)
        {
            Transform existing = panelGO.transform.Find("DirtParent");
            if (existing != null)
            {
                dirtParent = existing.GetComponent<RectTransform>();
                return;
            }

            // buat baru kalau hilang
            GameObject go = new GameObject("DirtParent", typeof(RectTransform));
            go.transform.SetParent(panelGO.transform, false);
            dirtParent = go.GetComponent<RectTransform>();
        }
    }

    private void SpawnDirt()
    {
        EnsureDirtParent();

        if (dirtPrefab == null || dirtParent == null) return;

        for (int i = dirtParent.childCount - 1; i >= 0; i--)
        {
            Transform child = dirtParent.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }

        cleanedDirt = 0;
        totalDirt = 0;

        for (int i = 0; i < dirtCount; i++)
        {
            CreateDirt();
            totalDirt++;
        }

        CreateDirt(true);
        totalDirt++;
    }

    private void CreateDirt(bool topRight = false)
    {
        GameObject dirt = Instantiate(dirtPrefab, dirtParent);
        RectTransform rt = dirt.GetComponent<RectTransform>();
        if (rt == null) return;

        if (topRight)
            rt.anchoredPosition = new Vector2(dirtParent.rect.width / 2 - 50f, dirtParent.rect.height / 2 - 50f);
        else
            rt.anchoredPosition = new Vector2(
                Random.Range(-dirtParent.rect.width / 2, dirtParent.rect.width / 2),
                Random.Range(-dirtParent.rect.height / 2, dirtParent.rect.height / 2)
            );

        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));

        DirtWipeUI wipe = dirt.GetComponent<DirtWipeUI>();
        if (wipe != null)
            wipe.onCleaned = OnDirtCleaned;
    }

    private void OnDirtCleaned()
    {
        cleanedDirt++;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        lastPatient = null;
        needsSpawn = false;
    }
}

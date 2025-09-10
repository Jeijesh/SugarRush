using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GlukometerShellGame : MonoBehaviour
{
    [Header("UI References")]
    public Button[] strips;                  // 3 tombol strip (button objects)
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI countdownText;
    public Button exitButton;

    [Header("Settings")]
    public float shuffleDuration = 0.5f;     // durasi animasi pindah posisi (saat swap dua posisi)
    public int shuffleCount = 5;             // berapa kali swap terjadi
    public int countdownTime = 3;            // detik sebelum shuffle mulai / sebelum player boleh klik

    // internal
    private Vector3[] startPositions;        // posisi awal slot (lokal) — tetap
    private int[] posToButton;               // mapping positionIndex -> buttonIndex (siapa menempati posisi itu)
    private bool canClick = false;
    private Patient lastPatient;

    // NOTE: specification: button index 0 adalah yang "benar"
    private const int ORIGINAL_CORRECT_BUTTON_INDEX = 0;

    void Start()
    {
        // safety
        if (strips == null || strips.Length < 1)
        {
            Debug.LogError("Assign strips (buttons) di inspector!");
            enabled = false;
            return;
        }

        // simpan posisi awal (anggap tiap button awalnya berada di slot pos i)
        startPositions = new Vector3[strips.Length];
        posToButton = new int[strips.Length];

        for (int i = 0; i < strips.Length; i++)
        {
            startPositions[i] = strips[i].transform.localPosition;
            posToButton[i] = i; // awal: posisi i ditempati button i

            // register click listener (capture index)
            int idx = i;
            strips[i].onClick.RemoveAllListeners();
            strips[i].onClick.AddListener(() => OnStripClicked(idx));
        }

        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

        // mulai pertama kali
        ResetGame();
    }

    void Update()
    {
        // reset otomatis bila pasien berubah (sama gaya ScalePanel)
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient)
            {
                lastPatient = p;
                ResetGame();
            }
        }
    }

    void ResetGame()
    {
        StopAllCoroutines();

        // kembalikan semua button ke posisi awal dan mapping
        for (int pos = 0; pos < strips.Length; pos++)
        {
            strips[pos].transform.localPosition = startPositions[pos];
            posToButton[pos] = pos;
            strips[pos].interactable = false;
        }

        canClick = false;
        if (feedbackText != null) feedbackText.text = "Menyiapkan...";
        if (countdownText != null) countdownText.text = "";

        // mulai countdown lalu shuffle
        StartCoroutine(CountdownThenShuffle());
    }

    IEnumerator CountdownThenShuffle()
    {
        // countdown visible sekali
        float t = countdownTime;
        while (t > 0f)
        {
            if (countdownText != null) countdownText.text = Mathf.CeilToInt(t).ToString();
            yield return new WaitForSeconds(1f);
            t -= 1f;
        }

        if (countdownText != null) countdownText.text = "";

        // jalankan shuffleCount kali (swap 2 posisi tiap iterasi)
        for (int k = 0; k < shuffleCount; k++)
        {
            // pilih dua posisi acak (position indices)
            int posA = Random.Range(0, posToButton.Length);
            int posB = Random.Range(0, posToButton.Length);
            while (posB == posA) posB = Random.Range(0, posToButton.Length);

            // animasi swap button yang menempati posA <-> posB
            yield return StartCoroutine(AnimateSwapPositions(posA, posB));
            // kecil jeda antar swap biar lebih kelihatan
            yield return new WaitForSeconds(0.05f);
        }

        // aktifkan klik setelah shuffle selesai
        canClick = true;
        for (int i = 0; i < strips.Length; i++) strips[i].interactable = true;

        if (feedbackText != null) feedbackText.text = "Pilih strip!";
    }

    // animasi memindahkan button yang menempati posA ke posB, dan posB ke posA
    private IEnumerator AnimateSwapPositions(int posA, int posB)
    {
        // button indices yang sekarang menempati posisi posA dan posB
        int btnA = posToButton[posA];
        int btnB = posToButton[posB];

        Vector3 fromA = strips[btnA].transform.localPosition;
        Vector3 fromB = strips[btnB].transform.localPosition;
        Vector3 toA = startPositions[posB]; // btnA akan ke posisi posB
        Vector3 toB = startPositions[posA]; // btnB akan ke posisi posA

        float elapsed = 0f;
        while (elapsed < shuffleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shuffleDuration);
            strips[btnA].transform.localPosition = Vector3.Lerp(fromA, toA, t);
            strips[btnB].transform.localPosition = Vector3.Lerp(fromB, toB, t);
            yield return null;
        }

        // pastikan posisi akhir tepat
        strips[btnA].transform.localPosition = toA;
        strips[btnB].transform.localPosition = toB;

        // swap mapping: posisi sekarang ditempati oleh tombol yang saling bertukar
        int tmp = posToButton[posA];
        posToButton[posA] = posToButton[posB];
        posToButton[posB] = tmp;
    }

    // idx = index tombol di array strips[] (button object index)
    private void OnStripClicked(int idx)
    {
        if (!canClick) return;

        // ambil pasien saat ini
        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        string glucoseText = p.glucose == 1 ? "High" : "Normal";

        // cek apakah tombol yang diklik adalah button index 0 (original correct)
        if (idx == ORIGINAL_CORRECT_BUTTON_INDEX)
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Benar)";
        }
        else
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Salah)";
        }

        // nonaktifkan klik sampai reset pasien berikutnya
        canClick = false;
        foreach (var b in strips) b.interactable = false;
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

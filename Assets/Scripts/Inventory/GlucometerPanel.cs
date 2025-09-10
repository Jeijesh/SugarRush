using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GlukometerShellGame : MonoBehaviour
{
    [Header("UI References")]
    public Button[] strips;                  // 3 tombol strip
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI countdownText;
    public Button exitButton;

    [Header("Settings")]
    public float shuffleDuration = 0.5f;     // durasi animasi pindah posisi
    public int shuffleCount = 5;             // berapa kali acak
    public float countdownTime = 3f;         // waktu tunggu sebelum bisa klik

    private Vector3[] startPositions;        // posisi awal tiap strip
    private int correctIndex;
    private bool canClick = false;
    private Patient lastPatient;

    private void Start()
    {
        // Simpan posisi awal
        startPositions = new Vector3[strips.Length];
        for (int i = 0; i < strips.Length; i++)
        {
            startPositions[i] = strips[i].transform.localPosition;
            int idx = i;
            strips[i].onClick.AddListener(() => OnStripClicked(idx));
        }

        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

        ResetGame();
    }

    private void Update()
    {
        // Cek pasien aktif
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient) // pasien baru
            {
                ResetGame();
                lastPatient = p;
            }
        }
    }

    private void ResetGame()
    {
        StopAllCoroutines();

        // Reset posisi strip
        for (int i = 0; i < strips.Length; i++)
            strips[i].transform.localPosition = startPositions[i];

        correctIndex = Random.Range(0, strips.Length); // tentukan strip benar
        canClick = false;

        if (feedbackText != null) feedbackText.text = "Tunggu...";
        if (countdownText != null) countdownText.text = countdownTime.ToString("F0");

        StartCoroutine(CountdownAndShuffle());
    }

    private IEnumerator CountdownAndShuffle()
    {
        // countdown sebelum mulai klik
        float timer = countdownTime;
        while (timer > 0f)
        {
            if (countdownText != null) countdownText.text = timer.ToString("F0");
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (countdownText != null) countdownText.text = "Mulai!";

        // Shuffle beberapa kali
        for (int i = 0; i < shuffleCount; i++)
        {
            yield return StartCoroutine(ShuffleOnce());
            yield return new WaitForSeconds(0.1f);
        }

        // Setelah shuffle selesai, player bisa klik
        canClick = true;
        if (feedbackText != null) feedbackText.text = "Pilih strip!";
    }

    private IEnumerator ShuffleOnce()
    {
        // Pilih dua index acak untuk ditukar
        int i = Random.Range(0, strips.Length);
        int j = Random.Range(0, strips.Length);
        while (j == i) j = Random.Range(0, strips.Length);

        Vector3 startPosI = strips[i].transform.localPosition;
        Vector3 startPosJ = strips[j].transform.localPosition;

        float elapsed = 0f;
        while (elapsed < shuffleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shuffleDuration;
            strips[i].transform.localPosition = Vector3.Lerp(startPosI, startPosJ, t);
            strips[j].transform.localPosition = Vector3.Lerp(startPosJ, startPosI, t);
            yield return null;
        }

        strips[i].transform.localPosition = startPosJ;
        strips[j].transform.localPosition = startPosI;

        // update correctIndex kalau strip benar ikut dipindahkan
        if (correctIndex == i) correctIndex = j;
        else if (correctIndex == j) correctIndex = i;
    }

    private void OnStripClicked(int index)
    {
        if (!canClick) return;

        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        string glucoseText = p.glucose == 1 ? "High" : "Normal";

        if (index == correctIndex)
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Benar)";
        }
        else
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Salah)";
        }

        canClick = false; // cuma sekali klik
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlukometerShellGame : MonoBehaviour
{
    [Header("Referensi UI")]
    public Button[] strips;                  // tombol strip
    public Button exitButton;
    public TextMeshProUGUI feedbackText;     // hasil Glukometer
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failedText;

    [Header("Pengaturan")]
    public float shuffleDuration = 0.5f;     // durasi swap animasi
    public int shuffleCount = 5;             // jumlah swap

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swapSound;

    private Vector3[] startPositions;        // posisi awal tombol
    private int[] posToButton;               // mapping posisi -> tombol
    private bool canClick = false;
    private Patient lastPatient;

    private const int CORRECT_BUTTON_INDEX = 0; // tombol benar

    private void Start()
    {
        if (strips == null || strips.Length < 1)
        {
            Debug.LogError("Tentukan strips di inspector!");
            enabled = false;
            return;
        }

        startPositions = new Vector3[strips.Length];
        posToButton = new int[strips.Length];

        for (int i = 0; i < strips.Length; i++)
        {
            startPositions[i] = strips[i].transform.localPosition;
            posToButton[i] = i;

            int idx = i;
            strips[i].onClick.RemoveAllListeners();
            strips[i].onClick.AddListener(() => OnStripClicked(idx));
        }

        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

        ResetGame();
    }

    private void Update()
    {
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

    private void ResetGame()
    {
        StopAllCoroutines();

        for (int pos = 0; pos < strips.Length; pos++)
        {
            strips[pos].transform.localPosition = startPositions[pos];
            posToButton[pos] = pos;
            strips[pos].interactable = false;
            Image img = strips[pos].GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }

        canClick = false;

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        StartCoroutine(BlinkCorrectThenShuffle());
    }

    private IEnumerator BlinkCorrectThenShuffle()
    {
        Image correctImg = strips[CORRECT_BUTTON_INDEX].GetComponent<Image>();
        if (correctImg != null)
        {
            Color original = correctImg.color;
            correctImg.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            correctImg.color = original;
        }

        for (int k = 0; k < shuffleCount; k++)
        {
            int posA = Random.Range(0, posToButton.Length);
            int posB = Random.Range(0, posToButton.Length);
            while (posB == posA) posB = Random.Range(0, posToButton.Length);

            yield return StartCoroutine(AnimateSwap(posA, posB));
            yield return new WaitForSeconds(0.05f);
        }

        canClick = true;
        foreach (var b in strips) b.interactable = true;
    }

    private IEnumerator AnimateSwap(int posA, int posB)
    {
        if (audioSource != null && swapSound != null)
            audioSource.PlayOneShot(swapSound);

        int btnA = posToButton[posA];
        int btnB = posToButton[posB];

        Vector3 fromA = strips[btnA].transform.localPosition;
        Vector3 fromB = strips[btnB].transform.localPosition;
        Vector3 toA = startPositions[posB];
        Vector3 toB = startPositions[posA];

        float elapsed = 0f;
        while (elapsed < shuffleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shuffleDuration);
            strips[btnA].transform.localPosition = Vector3.Lerp(fromA, toA, t);
            strips[btnB].transform.localPosition = Vector3.Lerp(fromB, toB, t);
            yield return null;
        }

        strips[btnA].transform.localPosition = toA;
        strips[btnB].transform.localPosition = toB;

        int tmp = posToButton[posA];
        posToButton[posA] = posToButton[posB];
        posToButton[posB] = tmp;
    }

    private void OnStripClicked(int idx)
    {
        if (!canClick) return;

        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        bool success = (idx == CORRECT_BUTTON_INDEX);

        int glucoseValue = success ? p.glucose : 1 - p.glucose;
        p.glucose = glucoseValue;

        if (PatientUI.Instance != null)
        {
            PatientUI.Instance.FillField("Glucose");
            PatientUI.Instance.RefreshDropdowns(p);
        }

        if (feedbackText != null)
            feedbackText.text = $"Glukosa: {(glucoseValue == 1 ? "Tinggi" : "Normal")}";

        if (success)
        {
            if (successText != null) successText.gameObject.SetActive(true);
            if (failedText != null) failedText.gameObject.SetActive(false);
        }
        else
        {
            if (failedText != null) failedText.gameObject.SetActive(true);
            if (successText != null) successText.gameObject.SetActive(false);
        }

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(success);

        canClick = false;
        foreach (var b in strips) b.interactable = false;
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FamilyHistoryGame : MonoBehaviour
{
    [Header("UI References")]
    public Button[] boxes;                    // kotak interaktif (misalnya 4 kotak)
    public TextMeshProUGUI feedbackText;
    public Button exitButton;

    [Header("Settings")]
    public float flashDuration = 0.5f;        // durasi kedip kotak
    public float delayBetweenFlashes = 0.3f;  // jeda antar kotak

    private List<int> generatedSequence = new List<int>(); 
    private List<int> playerInput = new List<int>();
    private int requiredClicks = 0;
    private bool canClick = false;
    private Patient lastPatient;

    private void Start()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

        // Tambahkan listener pada setiap box
        for (int i = 0; i < boxes.Length; i++)
        {
            int idx = i;
            boxes[i].onClick.AddListener(() => OnBoxClicked(idx));
        }

        ResetGame();
    }

    private void Update()
    {
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
        generatedSequence.Clear();
        playerInput.Clear();
        canClick = false;

        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        // Tentukan panjang urutan sesuai family history
        requiredClicks = p.family == 0 ? 1 : (p.family == 1 ? 2 : 3);

        // Generate urutan acak
        for (int i = 0; i < requiredClicks; i++)
        {
            generatedSequence.Add(Random.Range(0, boxes.Length));
        }

        // Mainkan urutan kedipan
        StartCoroutine(PlaySequence());

        // Info awal
        string familyText = p.family == 0 ? "None" : p.family == 1 ? "Level1" : "Level2";
        if (feedbackText != null) feedbackText.text = $"Family: {familyText}\nTunggu giliran...";
    }

    private IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (int idx in generatedSequence)
        {
            Image img = boxes[idx].GetComponent<Image>();
            Color originalColor = img.color;

            img.color = Color.yellow; 
            yield return new WaitForSeconds(flashDuration);
            img.color = originalColor;

            yield return new WaitForSeconds(delayBetweenFlashes);
        }

        canClick = true;
        if (feedbackText != null) feedbackText.text = "Ikuti urutan!";
    }

    private void OnBoxClicked(int index)
    {
        if (!canClick) return;

        playerInput.Add(index);

        // bikin kotak berkedip saat diklik
        StartCoroutine(FlashBox(index));

        if (playerInput.Count >= requiredClicks)
        {
            FinishGame();
        }
    }

    private IEnumerator FlashBox(int index)
    {
        Image img = boxes[index].GetComponent<Image>();
        Color originalColor = img.color;

        img.color = Color.green; 
        yield return new WaitForSeconds(0.2f);
        img.color = originalColor;
    }

    private void FinishGame()
    {
        canClick = false;

        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        string familyText = p.family == 0 ? "None" : p.family == 1 ? "Level1" : "Level2";
        string sequenceStr = string.Join(", ", generatedSequence);

        if (feedbackText != null)
            feedbackText.text = $"Family: {familyText}\nSequence: {sequenceStr}\n(Result: Benar)";
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

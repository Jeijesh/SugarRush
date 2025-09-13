using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FamilyHistoryGame : MonoBehaviour
{
    [Header("UI References")]
    public Button[] boxes;                    
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failedText;
    public Button exitButton;

    [Header("Settings")]
    public float flashDuration = 0.5f;        
    public float delayBetweenFlashes = 0.3f;  

    [Header("Audio")]
    public AudioSource clickAudio;    // drag SFX klik di sini
    public AudioSource successAudio;  // drag SFX sukses
    public AudioSource failAudio;     // drag SFX gagal

    private List<int> generatedSequence = new List<int>(); 
    private List<int> playerInput = new List<int>();
    private int requiredClicks = 0;
    private bool canClick = false;
    private Patient lastPatient;

    private void Start()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

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
            if (p != lastPatient)
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

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = ""; 

        Patient p = PatientUI.Instance != null ? PatientUI.Instance.currentPatient : null;
        if (p == null) return;

        requiredClicks = p.family == 0 ? 1 : (p.family == 1 ? 2 : 3);

        for (int i = 0; i < requiredClicks; i++)
        {
            generatedSequence.Add(Random.Range(0, boxes.Length));
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (int idx in generatedSequence)
        {
            Image img = boxes[idx].GetComponent<Image>();
            Color originalColor = img.color;

            img.color = Color.yellow;
            if (clickAudio != null) clickAudio.Play(); // SFX saat flash
            yield return new WaitForSeconds(flashDuration);
            img.color = originalColor;

            yield return new WaitForSeconds(delayBetweenFlashes);
        }

        canClick = true;
        if (feedbackText != null)
            feedbackText.text = "Klik kotak sesuai urutan!";
    }

    private void OnBoxClicked(int index)
    {
        if (!canClick) return;

        playerInput.Add(index);
        StartCoroutine(FlashBox(index));

        if (clickAudio != null) clickAudio.Play(); // SFX saat klik kotak

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

        bool correct = true;
        for (int i = 0; i < requiredClicks; i++)
        {
            if (playerInput[i] != generatedSequence[i])
            {
                correct = false;
                break;
            }
        }

        if (!correct)
        {
            p.family = Mathf.Clamp(p.family + 1, 0, 2); 
            if (failAudio != null) failAudio.Play(); // SFX gagal
        }
        else
        {
            if (successAudio != null) successAudio.Play(); // SFX sukses
        }

        string familyText = p.family == 0 ? "None" : p.family == 1 ? "Level1" : "Level2";
        if (feedbackText != null)
            feedbackText.text = $"Family: {familyText}";

        if (correct)
        {
            if (successText != null) successText.gameObject.SetActive(true);
            if (failedText != null) failedText.gameObject.SetActive(false);
        }
        else
        {
            if (failedText != null) failedText.gameObject.SetActive(true);
            if (successText != null) successText.gameObject.SetActive(false);
        }

        if (PatientUI.Instance != null)
        {
            PatientUI.Instance.FillField("Family");
            PatientUI.Instance.RefreshDropdowns(p);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddGameResult(correct);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

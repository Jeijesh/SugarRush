using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GlukometerShellGame : MonoBehaviour
{
    [Header("UI References")]
    public Button[] strips;                  // 3 strip buttons
    public TextMeshProUGUI feedbackText;
    public Button exitButton;

    [Header("Settings")]
    public float shuffleDuration = 0.5f;     // duration of swap animation
    public int shuffleCount = 5;             // number of swaps

    [Header("Audio")]
    public AudioSource audioSource;          // assign in inspector
    public AudioClip swapSound;              // sound played when swapping

    // internal
    private Vector3[] startPositions;        // initial slot positions
    private int[] posToButton;               // mapping: positionIndex -> buttonIndex
    private bool canClick = false;
    private Patient lastPatient;

    // specification: button index 0 is the "correct" one
    private const int ORIGINAL_CORRECT_BUTTON_INDEX = 0;

    void Start()
    {
        if (strips == null || strips.Length < 1)
        {
            Debug.LogError("Assign strips (buttons) in the inspector!");
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

    void Update()
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

    void ResetGame()
    {
        StopAllCoroutines();

        for (int pos = 0; pos < strips.Length; pos++)
        {
            strips[pos].transform.localPosition = startPositions[pos];
            posToButton[pos] = pos;
            strips[pos].interactable = false;
        }

        canClick = false;
        if (feedbackText != null) feedbackText.text = "Preparing...";

        StartCoroutine(BlinkCorrectStripThenShuffle());
    }

    IEnumerator BlinkCorrectStripThenShuffle()
    {
        Image img = strips[ORIGINAL_CORRECT_BUTTON_INDEX].GetComponent<Image>();
        if (img != null)
        {
            img.color = Color.white;
            img.color = Color.green;
            yield return new WaitForSeconds(1f);
            img.color = Color.white;
        }

        for (int k = 0; k < shuffleCount; k++)
        {
            int posA = Random.Range(0, posToButton.Length);
            int posB = Random.Range(0, posToButton.Length);
            while (posB == posA) posB = Random.Range(0, posToButton.Length);

            yield return StartCoroutine(AnimateSwapPositions(posA, posB));
            yield return new WaitForSeconds(0.05f);
        }

        canClick = true;
        for (int i = 0; i < strips.Length; i++) strips[i].interactable = true;

        if (feedbackText != null) feedbackText.text = "Pick a strip!";
    }

    private IEnumerator AnimateSwapPositions(int posA, int posB)
    {
        // 🔊 Play swap sound
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

        string glucoseText = p.glucose == 1 ? "High" : "Normal";

        if (idx == ORIGINAL_CORRECT_BUTTON_INDEX)
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Correct)";
        }
        else
        {
            if (feedbackText != null) feedbackText.text = $"Glucose: {glucoseText}\n(Result: Wrong)";
        }

        canClick = false;
        foreach (var b in strips) b.interactable = false;
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatientDialogue : MonoBehaviour
{
    public static PatientDialogue Instance;

    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image dialogueImage;               // 🔥 gambar bubble / patient
    public RectTransform dialoguePanel;       // 🔥 container (panel) yg naik turun
    public float animDuration = 0.5f;         // durasi animasi

    private List<string> dialogues = new List<string>();
    private int lastIndex = -1;
    private Coroutine animRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadDialogues();
    }

    private void LoadDialogues()
    {
        TextAsset file = Resources.Load<TextAsset>("dialogues"); 
        if (file != null)
        {
            string[] lines = file.text.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    dialogues.Add(trimmed);
            }
        }
        else
        {
            Debug.LogWarning("dialogues.txt not found in Resources!");
        }
    }

    public void ShowRandomDialogue()
    {
        if (dialogues.Count == 0 || dialogueText == null) return;

        int index;
        do
        {
            index = Random.Range(0, dialogues.Count);
        } while (index == lastIndex && dialogues.Count > 1);

        lastIndex = index;
        dialogueText.text = dialogues[index];

        // animasi muncul
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimateDialogue());
    }

    private IEnumerator AnimateDialogue()
    {
        if (dialoguePanel == null) yield break;

        Vector2 startPos = dialoguePanel.anchoredPosition + new Vector2(0, -50f);
        Vector2 endPos = dialoguePanel.anchoredPosition;
        float t = 0f;

        // mulai dari bawah + alpha 0
        dialoguePanel.anchoredPosition = startPos;
        SetAlpha(0f);

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;

            // posisi naik
            dialoguePanel.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);

            // fade in
            SetAlpha(Mathf.SmoothStep(0f, 1f, lerp));

            yield return null;
        }

        dialoguePanel.anchoredPosition = endPos;
        SetAlpha(1f);
    }

    private void SetAlpha(float a)
    {
        if (dialogueText != null)
        {
            var c = dialogueText.color;
            c.a = a;
            dialogueText.color = c;
        }
        if (dialogueImage != null)
        {
            var c = dialogueImage.color;
            c.a = a;
            dialogueImage.color = c;
        }
    }
}

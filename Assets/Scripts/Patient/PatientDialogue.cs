using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PatientDialogue : MonoBehaviour
{
    public static PatientDialogue Instance;

    [Header("UI References")]
    public TextMeshProUGUI dialogueText;

    private List<string> dialogues = new List<string>();
    private int lastIndex = -1;

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

        // pastikan tidak keluar teks sama 2x berturut
        int index;
        do
        {
            index = Random.Range(0, dialogues.Count);
        } while (index == lastIndex && dialogues.Count > 1);

        lastIndex = index;
        dialogueText.text = dialogues[index];
    }
}

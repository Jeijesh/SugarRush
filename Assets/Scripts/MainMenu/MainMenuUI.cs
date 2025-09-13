using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject initialsPanel;       // panel input initials
    public GameObject leaderboardPanel;    // panel leaderboard
    public GameObject creditsPanel;        // panel credits

    [Header("Initials Input")]
    public TMP_InputField initialsInput;
    public Button initialsSubmitButton;

    [Header("Buttons")]
    public Button startButton;
    public Button leaderboardButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Leaderboard")]
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;

    private void Start()
    {
        // --- Setup button listeners ---
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (leaderboardButton != null) leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnCreditsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if (initialsSubmitButton != null) initialsSubmitButton.onClick.AddListener(OnInitialsSubmit);

        // Hide all secondary panels at start
        if (initialsPanel != null) initialsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        ShowLeaderboard(); // optional: show last leaderboard entries
    }

    // ================== MAIN MENU BUTTONS ==================
    private void OnStartClicked()
    {
        // Show panel input initials
        if (initialsPanel != null)
        {
            initialsPanel.SetActive(true);
        }
    }

    private void OnLeaderboardClicked()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            ShowLeaderboard();
        }
    }

    private void OnCreditsClicked()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    // ================== INITIALS INPUT ==================
    private void OnInitialsSubmit()
    {
        if (initialsInput == null) return;

        string initials = initialsInput.text.Trim();
        if (string.IsNullOrEmpty(initials)) return;

        // Set initials ke GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerInitials(initials);
        }

        // Load RushMode scene
        SceneManager.LoadScene("RushMode");
    }

    // ================== LEADERBOARD ==================
private void ShowLeaderboard()
{
    if (leaderboardContent == null || leaderboardEntryPrefab == null)
    {
        Debug.LogWarning("LeaderboardContent atau LeaderboardEntryPrefab belum diassign!");
        return;
    }

    foreach (Transform child in leaderboardContent)
        Destroy(child.gameObject);

    var entries = LeaderboardManager.Instance?.GetLeaderboard();
    if (entries == null)
    {
        Debug.LogWarning("LeaderboardManager.Instance.GetLeaderboard() mengembalikan null!");
        return;
    }

    foreach (var entry in entries)
    {
        GameObject go = Instantiate(leaderboardEntryPrefab, leaderboardContent);
        TMP_Text text = go.GetComponent<TMP_Text>();
        if (text != null)
            text.text = $"{entry.initials} - {entry.score}";
    }
}


    // ================== CLOSE PANELS ==================
    public void CloseInitialsPanel()
    {
        if (initialsPanel != null)
            initialsPanel.SetActive(false);
    }

    public void CloseLeaderboardPanel()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void CloseCreditsPanel()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}

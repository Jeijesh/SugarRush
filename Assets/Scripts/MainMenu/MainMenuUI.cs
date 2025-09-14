using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject initialsPanel;
    public GameObject leaderboardPanel;
    public GameObject creditsPanel;
    public GameObject exitConfirmationPanel; // panel konfirmasi keluar

    [Header("Initials Input")]
    public TMP_InputField initialsInput;
    public Button initialsSubmitButton;

    [Header("Buttons")]
    public Button startButton;
    public Button leaderboardButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Panel Exit Buttons")]
    public Button initialsExitButton;
    public Button leaderboardExitButton;
    public Button creditsExitButton;
    public Button confirmExitYesButton;
    public Button confirmExitNoButton;

    [Header("Leaderboard (10 entries)")]
    public TMP_Text[] leaderboardTexts = new TMP_Text[10]; // assign 10 TextMeshPro di Inspector

    private void Start()
    {
        // Setup main buttons
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (leaderboardButton != null) leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnCreditsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if (initialsSubmitButton != null) initialsSubmitButton.onClick.AddListener(OnInitialsSubmit);

        // Setup panel exit buttons
        if (initialsExitButton != null) initialsExitButton.onClick.AddListener(CloseInitialsPanel);
        if (leaderboardExitButton != null) leaderboardExitButton.onClick.AddListener(CloseLeaderboardPanel);
        if (creditsExitButton != null) creditsExitButton.onClick.AddListener(CloseCreditsPanel);

        // Konfirmasi exit
        if (confirmExitYesButton != null) confirmExitYesButton.onClick.AddListener(Application.Quit);
        if (confirmExitNoButton != null) confirmExitNoButton.onClick.AddListener(() =>
        {
            if (exitConfirmationPanel != null)
                exitConfirmationPanel.SetActive(false);
        });

        // Hide all secondary panels at start
        if (initialsPanel != null) initialsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(false);

        ShowLeaderboard(); // tampilkan leaderboard terakhir
    }

    private void OnStartClicked()
    {

        // Buka panel input initials
        if (initialsPanel != null)
        {
            initialsPanel.SetActive(true);
            initialsInput.text = PlayerPrefs.GetString("PlayerInitials", "");
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
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
        else
            Application.Quit();
    }

    private void OnInitialsSubmit()
    {
        if (initialsInput == null) return;

        string initials = initialsInput.text.Trim();
        if (string.IsNullOrEmpty(initials)) return;

        PlayerPrefs.SetString("PlayerInitials", initials.ToUpper());
        PlayerPrefs.Save();

        SceneManager.LoadScene("RushMode");
    }

    private void ShowLeaderboard()
    {
        if (leaderboardTexts == null || leaderboardTexts.Length == 0) return;

        var entries = LeaderboardManager.Instance?.GetLeaderboard();
        if (entries == null) entries = new List<LeaderboardManager.LeaderboardEntry>();

        for (int i = 0; i < leaderboardTexts.Length; i++)
        {
            if (leaderboardTexts[i] == null) continue;

            if (i < entries.Count)
            {
                var entry = entries[i];
                string name = entry.initials.Length > 10 ? entry.initials.Substring(0, 10) : entry.initials;
                leaderboardTexts[i].text = $"{i + 1}. {name}: {entry.score} Poin";
            }
            else
            {
                leaderboardTexts[i].text = $"{i + 1}. ---";
            }
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

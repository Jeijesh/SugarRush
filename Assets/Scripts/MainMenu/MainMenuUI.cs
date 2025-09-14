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
    public GameObject exitConfirmationPanel;

    [Header("Initials Input")]
    public TMP_InputField initialsInput;
    public Button initialsSubmitButton;

    [Header("Main Buttons")]
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
    public TMP_Text[] leaderboardTexts = new TMP_Text[10];

    [Header("Audio Clips")]
    public AudioClip bgmMainMenu;
    public AudioClip buttonSFX;

    private void Start()
    {
        // Main Menu BGM (pakai BGMManager langsung, dicek biar ga duplikat)
        if (BGMManager.Instance != null && bgmMainMenu != null)
            BGMManager.Instance.PlayBGM(bgmMainMenu);

        // Setup main buttons
        startButton.onClick.AddListener(() => { PlayButtonSFX(); OnStartClicked(); });
        leaderboardButton.onClick.AddListener(() => { PlayButtonSFX(); OnLeaderboardClicked(); });
        creditsButton.onClick.AddListener(() => { PlayButtonSFX(); OnCreditsClicked(); });
        exitButton.onClick.AddListener(() => { PlayButtonSFX(); OnExitClicked(); });

        if (initialsSubmitButton != null)
            initialsSubmitButton.onClick.AddListener(() => { PlayButtonSFX(); OnInitialsSubmit(); });

        // Setup panel exit buttons
        if (initialsExitButton != null)
            initialsExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseInitialsPanel(); });
        if (leaderboardExitButton != null)
            leaderboardExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseLeaderboardPanel(); });
        if (creditsExitButton != null)
            creditsExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseCreditsPanel(); });

        // Confirm exit
        if (confirmExitYesButton != null)
            confirmExitYesButton.onClick.AddListener(() => { PlayButtonSFX(); Application.Quit(); });
        if (confirmExitNoButton != null)
            confirmExitNoButton.onClick.AddListener(() => { PlayButtonSFX(); exitConfirmationPanel.SetActive(false); EnableMainButtons(); });

        // Hide all panels
        initialsPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        creditsPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);

        ShowLeaderboard();
    }

    private void PlayButtonSFX()
    {
        if (BGMManager.Instance != null && buttonSFX != null)
            BGMManager.Instance.PlaySFX(buttonSFX);
    }

    private void SetMainButtonsActive(bool active)
    {
        startButton.interactable = active;
        leaderboardButton.interactable = active;
        creditsButton.interactable = active;
        exitButton.interactable = active;
    }

    private void EnableMainButtons() => SetMainButtonsActive(true);
    private void DisableMainButtons() => SetMainButtonsActive(false);

    private void OnStartClicked()
    {
        DisableMainButtons();
        if (initialsPanel != null)
        {
            initialsPanel.SetActive(true);
            initialsInput.text = PlayerPrefs.GetString("PlayerInitials", "");
        }
    }

    private void OnLeaderboardClicked()
    {
        DisableMainButtons();
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            ShowLeaderboard();
        }
    }

    private void OnCreditsClicked()
    {
        DisableMainButtons();
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void OnExitClicked()
    {
        DisableMainButtons();
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

        var entries = LeaderboardManager.Instance?.GetLeaderboard() ?? new List<LeaderboardManager.LeaderboardEntry>();

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
        EnableMainButtons();
    }

    public void CloseLeaderboardPanel()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        EnableMainButtons();
    }

    public void CloseCreditsPanel()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        EnableMainButtons();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject initialsPanel;
    public GameObject customPatientPanel;  // Panel Custom
    public GameObject leaderboardPanel;
    public GameObject creditsPanel;
    public GameObject exitConfirmationPanel;
    public GameObject tutorialPanel;   // ✅ Tambah panel tutorial
    [Header("Initials Input")]
    public TMP_InputField initialsInput;
    public Button initialsSubmitButton;

    [Header("Main Buttons")]
    public Button startButton;
    public Button customButton;  // Tombol Custom
    public Button leaderboardButton;
    public Button creditsButton;
    public Button exitButton;
    public Button tutorialButton;      // ✅ Tambah tombol tutorial
    [Header("Panel Exit Buttons")]
    public Button initialsExitButton;
    public Button customExitButton; // Tombol close panel custom
    public Button leaderboardExitButton;
    public Button creditsExitButton;
    public Button confirmExitYesButton;
    public Button confirmExitNoButton;
    public Button tutorialExitButton;  // ✅ Tombol close tutorial
    [Header("Leaderboard (10 entries)")]
    public TMP_Text[] leaderboardTexts = new TMP_Text[10];

    [Header("UI Texts")]
    public TMP_Text titleText; // Contoh 2 teks di menu utama
    public TMP_Text subtitleText;

[Header("Extra UI Texts")]
public TMP_Text greetingText;
public GameObject bubbleText1;
public GameObject bubbleText2;


    [Header("Audio Clips")]
    public AudioClip bgmMainMenu;
    public AudioClip buttonSFX;

private void Start()
{
    if (BGMManager.Instance != null && bgmMainMenu != null)
        BGMManager.Instance.PlayBGM(bgmMainMenu);

    // Setup main buttons
    startButton.onClick.AddListener(() => { PlayButtonSFX(); OnStartClicked(); });
    customButton.onClick.AddListener(() => { PlayButtonSFX(); OnCustomClicked(); });
    leaderboardButton.onClick.AddListener(() => { PlayButtonSFX(); OnLeaderboardClicked(); });
    creditsButton.onClick.AddListener(() => { PlayButtonSFX(); OnCreditsClicked(); });
    exitButton.onClick.AddListener(() => { PlayButtonSFX(); OnExitClicked(); });
    tutorialButton.onClick.AddListener(() => { PlayButtonSFX(); OnTutorialClicked(); }); // ✅ tutorial

    if (initialsSubmitButton != null)
        initialsSubmitButton.onClick.AddListener(() => { PlayButtonSFX(); OnInitialsSubmit(); });

    // Setup panel exit buttons
    if (initialsExitButton != null)
        initialsExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseInitialsPanel(); });
    if (customExitButton != null)
        customExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseCustomPanel(); });
    if (leaderboardExitButton != null)
        leaderboardExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseLeaderboardPanel(); });
    if (creditsExitButton != null)
        creditsExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseCreditsPanel(); });
    if (tutorialExitButton != null)
        tutorialExitButton.onClick.AddListener(() => { PlayButtonSFX(); CloseTutorialPanel(); }); // ✅ close tutorial

    // Confirm exit
    if (confirmExitYesButton != null)
        confirmExitYesButton.onClick.AddListener(() => { PlayButtonSFX(); Application.Quit(); });
    if (confirmExitNoButton != null)
        confirmExitNoButton.onClick.AddListener(() => { PlayButtonSFX(); exitConfirmationPanel.SetActive(false); EnableMainUI(); });

    // Hide all panels initially
    initialsPanel.SetActive(false);
    customPatientPanel.SetActive(false);
    leaderboardPanel.SetActive(false);
    creditsPanel.SetActive(false);
    exitConfirmationPanel.SetActive(false);
    tutorialPanel.SetActive(false);   // ✅ sembunyikan awal

    ShowLeaderboard();
}


    private void PlayButtonSFX()
    {
        if (BGMManager.Instance != null && buttonSFX != null)
            BGMManager.Instance.PlaySFX(buttonSFX);
    }

    // ===================== MAIN UI VISIBILITY =====================
private void SetMainUIVisible(bool visible)
{
    startButton.gameObject.SetActive(visible);
    customButton.gameObject.SetActive(visible);
    leaderboardButton.gameObject.SetActive(visible);
    creditsButton.gameObject.SetActive(visible);
    exitButton.gameObject.SetActive(visible);

    if (titleText != null) titleText.gameObject.SetActive(visible);
    if (subtitleText != null) subtitleText.gameObject.SetActive(visible);

    if (greetingText != null) greetingText.gameObject.SetActive(visible);
    if (bubbleText1 != null) bubbleText1.SetActive(visible);
    if (bubbleText2 != null) bubbleText2.SetActive(visible);
}


    private void EnableMainUI() => SetMainUIVisible(true);
    private void DisableMainUI() => SetMainUIVisible(false);

    // =================== MAIN BUTTON EVENTS ===================
    private void OnStartClicked()
    {
        DisableMainUI();
        if (initialsPanel != null)
        {
            initialsPanel.SetActive(true);
            initialsInput.text = PlayerPrefs.GetString("PlayerInitials", "");
        }
    }

    private void OnCustomClicked()
    {
        DisableMainUI();
        if (customPatientPanel != null)
        {
            customPatientPanel.SetActive(true);
        }
        // Hanya tombol exit credit yang tetap terlihat
        if (creditsExitButton != null)
            creditsExitButton.gameObject.SetActive(true);
    }

    private void OnLeaderboardClicked()
    {
        DisableMainUI();
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            ShowLeaderboard();
        }
    }

    private void OnCreditsClicked()
    {
        DisableMainUI();
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void OnExitClicked()
    {
        DisableMainUI();
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

    // =================== LEADERBOARD ===================
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

    // =================== CLOSE PANELS ===================
    public void CloseInitialsPanel()
    {
        if (initialsPanel != null)
            initialsPanel.SetActive(false);
        EnableMainUI();
    }

    public void CloseCustomPanel()
    {
        if (customPatientPanel != null)
            customPatientPanel.SetActive(false);
        EnableMainUI();
    }

    public void CloseLeaderboardPanel()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        EnableMainUI();
    }

    public void CloseCreditsPanel()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        EnableMainUI();
    }

        private void OnTutorialClicked()
    {
        DisableMainUI();
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
    }

    // =================== CLOSE PANELS ===================
    public void CloseTutorialPanel()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
        EnableMainUI();
    }
}

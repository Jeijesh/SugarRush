using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string initials;
        public int score;
    }

    [System.Serializable]
    private class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries;
    }

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private const int maxEntries = 10;
    private const string playerPrefsKey = "LeaderboardJSON";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // tetap hidup lintas scene
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tambah skor pemain baru dan simpan
    /// </summary>
    public void AddScore(string initials, int score)
    {
        if (string.IsNullOrEmpty(initials)) initials = "AAA";

        leaderboard.Add(new LeaderboardEntry
        {
            initials = initials,
            score = score
        });

        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        if (leaderboard.Count > maxEntries)
            leaderboard.RemoveAt(leaderboard.Count - 1);

        SaveLeaderboard();
    }

    /// <summary>
    /// Ambil seluruh leaderboard
    /// </summary>
    public List<LeaderboardEntry> GetLeaderboard()
    {
        return new List<LeaderboardEntry>(leaderboard);
    }

    /// <summary>
    /// Simpan leaderboard ke PlayerPrefs
    /// </summary>
    public void SaveLeaderboard()
    {
        LeaderboardWrapper wrapper = new LeaderboardWrapper { entries = leaderboard };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(playerPrefsKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load leaderboard dari PlayerPrefs
    /// </summary>
    public void LoadLeaderboard()
    {
        if (!PlayerPrefs.HasKey(playerPrefsKey)) return;

        string json = PlayerPrefs.GetString(playerPrefsKey);
        LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
        if (wrapper != null && wrapper.entries != null)
        {
            leaderboard = new List<LeaderboardEntry>(wrapper.entries);
        }
    }

}

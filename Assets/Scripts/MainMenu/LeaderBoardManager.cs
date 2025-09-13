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

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private const int maxEntries = 10;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Tambah skor pemain baru ke leaderboard
    /// </summary>
    public void AddScore(string initials, int score)
    {
        if (string.IsNullOrEmpty(initials)) initials = "AAA"; // default

        leaderboard.Add(new LeaderboardEntry
        {
            initials = initials,
            score = score
        });

        // Urutkan descending
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        // Hanya simpan maxEntries
        if (leaderboard.Count > maxEntries)
            leaderboard.RemoveAt(leaderboard.Count - 1);
    }

    /// <summary>
    /// Ambil seluruh leaderboard
    /// </summary>
    public List<LeaderboardEntry> GetLeaderboard()
    {
        return leaderboard;
    }
}

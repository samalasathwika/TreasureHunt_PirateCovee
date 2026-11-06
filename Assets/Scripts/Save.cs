using UnityEngine;

// Saved progress + level settings (PlayerPrefs).
public static class Save
{
    public const int LevelCount = 4;
    public static int SelectedLevel = 1;   // runtime only
    public static readonly string[] LevelNames = { "Moonlit Night", "Sunny Day", "Golden Sunset", "Storm at Skull Rock" };

    public static int TotalCoins  { get { return PlayerPrefs.GetInt("coins", 0); }  set { PlayerPrefs.SetInt("coins", value); } }
    public static int TotalChests { get { return PlayerPrefs.GetInt("chests", 0); } set { PlayerPrefs.SetInt("chests", value); } }
    public static int TotalMeters { get { return PlayerPrefs.GetInt("meters", 0); } set { PlayerPrefs.SetInt("meters", value); } }
    public static int UnlockedLevel { get { return Mathf.Max(1, PlayerPrefs.GetInt("unlocked", 1)); } set { PlayerPrefs.SetInt("unlocked", value); } }
    public static float Music { get { return PlayerPrefs.GetFloat("music", 0.7f); } set { PlayerPrefs.SetFloat("music", value); } }
    public static float Sfx   { get { return PlayerPrefs.GetFloat("sfx", 1f); }   set { PlayerPrefs.SetFloat("sfx", value); } }

    public static int GetStars(int level) { return PlayerPrefs.GetInt("stars" + level, 0); }
    public static void SetStars(int level, int s) { if (s > GetStars(level)) PlayerPrefs.SetInt("stars" + level, s); }
    public static void Flush() { PlayerPrefs.Save(); }

    // Level difficulty
    public static int TargetDistance(int l) { return 500 * l; }
    public static float StartSpeed(int l) { return 9f + 1.5f * (l - 1); }
    public static float ObstacleChance(int l) { return Mathf.Min(0.85f, 0.35f + 0.1f * l); }

    // Missions and achievements share the same lifetime totals
    public static readonly string[] MissionNames = { "Collect 100 Coins", "Find 3 Treasure Chests", "Run 500 Meters" };
    public static readonly int[] MissionGoals = { 100, 3, 500 };
    public static readonly string[] AchievementNames = { "Coin Collector - collect 1000 coins", "Treasure Hunter - open 10 chests", "Marathon Runner - run 5000 meters" };
    public static readonly int[] AchievementGoals = { 1000, 10, 5000 };

    public static int Value(int i) { return i == 0 ? TotalCoins : (i == 1 ? TotalChests : TotalMeters); }
}

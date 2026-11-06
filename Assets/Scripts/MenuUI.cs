using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Main menu: Play, Levels, Missions, Achievements, Settings, Quit.
public class MenuUI : MonoBehaviour
{
    public GameObject mainPanel, levelsPanel, missionsPanel, achievementsPanel, settingsPanel;
    public TMP_Text[] levelLabels;
    public Button[] levelButtons;
    public TMP_Text[] missionTexts, achievementTexts;
    public RectTransform[] missionBars;
    public Slider musicSlider, sfxSlider;
    public TMP_Text qualityText;

    GameObject[] all;

    void Start()
    {
        Time.timeScale = 1f;
        AudioManager.Ensure();
        all = new[] { mainPanel, levelsPanel, missionsPanel, achievementsPanel, settingsPanel };
        musicSlider.value = Save.Music; sfxSlider.value = Save.Sfx;
        musicSlider.onValueChanged.AddListener(v => { Save.Music = v; AudioManager.I.ApplyVolumes(); });
        sfxSlider.onValueChanged.AddListener(v => { Save.Sfx = v; AudioManager.I.ApplyVolumes(); });
        RefreshQuality();
        ShowMain();
    }

    void Show(GameObject p) { foreach (var g in all) g.SetActive(g == p); }
    public void ShowMain() { Show(mainPanel); }
    public void ShowLevels() { RefreshLevels(); Show(levelsPanel); }
    public void ShowMissions() { RefreshMissions(); Show(missionsPanel); }
    public void ShowAchievements() { RefreshAchievements(); Show(achievementsPanel); }
    public void ShowSettings() { Show(settingsPanel); }

    public void Play() { Save.SelectedLevel = Save.UnlockedLevel; SceneManager.LoadScene("Game"); }
    public void PlayLevel(int level)
    {
        if (level > Save.UnlockedLevel) return;
        Save.SelectedLevel = level; SceneManager.LoadScene("Game");
    }

    public void CycleQuality()
    {
        int n = QualitySettings.names.Length;
        QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel() + 1) % n, true);
        RefreshQuality();
    }
    void RefreshQuality() { qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()]; }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void RefreshLevels()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int lv = i + 1; bool open = lv <= Save.UnlockedLevel;
            levelButtons[i].interactable = open;
            levelLabels[i].text = open ? "Level " + lv + "\n" + Save.LevelNames[i] + "\n" + Save.GetStars(lv) + "/3 stars\n" + Save.TargetDistance(lv) + " m" : "Level " + lv + "\nLOCKED";
        }
    }

    void RefreshMissions()
    {
        for (int i = 0; i < 3; i++)
        {
            int v = Mathf.Min(Save.Value(i), Save.MissionGoals[i]);
            missionTexts[i].text = Save.MissionNames[i] + "      " + v + "/" + Save.MissionGoals[i];
            missionBars[i].anchorMax = new Vector2((float)v / Save.MissionGoals[i], 1f);
        }
    }

    void RefreshAchievements()
    {
        for (int i = 0; i < 3; i++)
        {
            int v = Save.Value(i); bool done = v >= Save.AchievementGoals[i];
            achievementTexts[i].text = Save.AchievementNames[i] + (done ? "   [UNLOCKED]" : "   " + v + "/" + Save.AchievementGoals[i]);
            achievementTexts[i].color = done ? new Color(0.1f, 0.5f, 0.15f) : new Color(0.25f, 0.15f, 0.08f);
        }
    }
}

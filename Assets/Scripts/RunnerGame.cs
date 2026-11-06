using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Game state, scoring, lives, HUD, pause / game-over / level-complete.
public class RunnerGame : MonoBehaviour
{
    public static RunnerGame I { get; private set; }
    public enum State { Playing, Paused, GameOver, LevelComplete }
    public State state = State.Playing;

    [Header("HUD")]
    public TMP_Text coinText, starText, distText, levelText, gameOverInfo, completeInfo;
    public Image[] hearts;
    public GameObject pausePanel, gameOverPanel, completePanel;

    [Header("Tuning")]
    public int maxLives = 3;
    public float acceleration = 0.12f;

    public int Level { get; private set; }
    public float Speed { get; private set; }
    public float Distance { get; private set; }
    public int Coins { get; private set; }
    public int Chests { get; private set; }
    public int Stars { get; private set; }
    public int Lives { get; private set; }
    public int Target { get; private set; }
    public bool IsInvulnerable { get { return invul > 0f; } }

    float startSpeed, maxSpeed, invul;

    void Awake() { I = this; Time.timeScale = 1f; }

    void Start()
    {
        AudioManager.Ensure();
        Level = Mathf.Clamp(Save.SelectedLevel, 1, Save.LevelCount);
        startSpeed = Save.StartSpeed(Level);
        Speed = startSpeed; maxSpeed = startSpeed + 6f;
        Target = Save.TargetDistance(Level);
        Lives = maxLives;
        pausePanel.SetActive(false); gameOverPanel.SetActive(false); completePanel.SetActive(false);
        if (levelText) levelText.text = "Level " + Level + " - " + Save.LevelNames[Level - 1];
        UpdateHUD();
    }

    void Update()
    {
        if ((state == State.Playing || state == State.Paused) && GameInput.PausePressed()) TogglePause();
        if (state == State.GameOver && GameInput.RestartPressed()) Restart();
        if (state != State.Playing) return;

        Speed = Mathf.Min(maxSpeed, Speed + acceleration * Time.deltaTime);
        Distance += Speed * Time.deltaTime;
        if (invul > 0f) invul -= Time.deltaTime;
        if (Distance >= Target) { Complete(); return; }
        UpdateHUD();
    }

    public void AddCoin(int v) { Coins += v; if (AudioManager.I) AudioManager.I.Play("coin"); }
    public void AddChest(int v) { Chests++; Coins += v; if (AudioManager.I) AudioManager.I.Play("chest"); }
    public void AddStar() { Stars = Mathf.Min(5, Stars + 1); if (AudioManager.I) AudioManager.I.Play("star"); }

    public void Hit()
    {
        if (state != State.Playing || invul > 0f) return;
        Lives--;
        invul = 1.5f;
        Speed = Mathf.Max(startSpeed, Speed * 0.85f);
        if (AudioManager.I) AudioManager.I.Play("hit");
        UpdateHUD();
        if (Lives <= 0) GameOver();
    }

    int Rating() { return Stars >= 5 ? 3 : (Stars >= 3 ? 2 : 1); }

    void Finish(bool won)
    {
        Save.TotalCoins += Coins; Save.TotalChests += Chests; Save.TotalMeters += (int)Distance;
        if (won)
        {
            Save.UnlockedLevel = Mathf.Max(Save.UnlockedLevel, Mathf.Min(Level + 1, Save.LevelCount));
            Save.SetStars(Level, Rating());
        }
        Save.Flush();
    }

    void GameOver()
    {
        state = State.GameOver; Finish(false);
        if (PirateChasers.I != null) PirateChasers.I.SnapClose();
        gameOverInfo.text = "Distance: " + (int)Distance + " m\nCoins: " + Coins + "   Chests: " + Chests + "   Stars: " + Stars + "/5";
        gameOverPanel.SetActive(true); Time.timeScale = 0f;
    }

    void Complete()
    {
        state = State.LevelComplete; Finish(true);
        completeInfo.text = "Level " + Level + " complete!\nCoins: " + Coins + "   Chests: " + Chests + "   Stars: " + Stars + "/5\nRating: " + Rating() + "/3 stars";
        completePanel.SetActive(true); Time.timeScale = 0f;
    }

    void UpdateHUD()
    {
        coinText.text = Coins.ToString();
        starText.text = Stars + "/5";
        distText.text = (int)Distance + " m / " + Target + " m";
        for (int i = 0; i < hearts.Length; i++) hearts[i].color = i < Lives ? new Color(0.9f, 0.1f, 0.15f) : new Color(0.25f, 0.2f, 0.2f);
    }

    // ----- button handlers -----
    public void TogglePause()
    {
        if (state == State.Playing) { state = State.Paused; pausePanel.SetActive(true); Time.timeScale = 0f; }
        else if (state == State.Paused) Resume();
    }
    public void Resume() { state = State.Playing; pausePanel.SetActive(false); Time.timeScale = 1f; }
    public void Restart() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void NextLevel() { Save.SelectedLevel = Mathf.Min(Level + 1, Save.LevelCount); Restart(); }
    public void BackToMenu() { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }
}

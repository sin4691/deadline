using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Michsky.UI.Dark;
public enum GameState { Playing, Dead, Cleared }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentState = GameState.Playing;
    public int itemSpawnCount;
    public MainPanelManager menuManager;
    public GameObject panels;
    public GameObject clearPanel;
    public UIDissolveEffect fadeDissolve;
    public TextMeshProUGUI counterText;
    private Timer timer;

    private int currentItemCount = 0;
    public bool isPaused = false;
    bool settingsOpen = false;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        timer = GetComponent<Timer>();
        timer.StartTimer();
    }
    void Start()
    {
        fadeDissolve.location = 0f;
        fadeDissolve.DissolveOut();
    }
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                TogglePause();
        }
    }
    public void SetTotalItemCount(int total)
    {
        itemSpawnCount = total;
        UpdateUI();
    }
    public void AddItemCount()
    {
        currentItemCount++;
        UpdateUI();
        // 효과음 재생
        if (currentItemCount == itemSpawnCount)
            GameClear();
    }
    void UpdateUI()
    {
        counterText.text = $"{currentItemCount} / {itemSpawnCount}";
    }
    public void PlayerDied()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Dead;
        // 사망모션 처리
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GameClear()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Cleared;
        // 클리어 처리
        Time.timeScale = 0f;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        clearPanel.SetActive(true);
    }
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void TogglePause()
    {
        if (currentState != GameState.Playing) return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        panels.SetActive(isPaused);
        if (isPaused)
            menuManager.EnableFirstPanel();
        UnityEngine.Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void OpenSettings()
    {
        settingsOpen = true;
        menuManager.OpenPanel("Settings");
    }
    public void CloseSettings()
    {
        settingsOpen = false;
        menuManager.OpenPanel("Pause");
    }
    public void Resume() 
    {
        isPaused = false;
        settingsOpen = false;
        Time.timeScale = 1f;
        panels.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
}

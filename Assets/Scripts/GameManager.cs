using Michsky.UI.Dark;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public enum GameState { Playing, Dead, Cleared }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentState = GameState.Playing;
    public MainPanelManager menuManager;
    public GameObject clearPanel;
    public GameObject panels;
    public UIDissolveEffect fadeDissolve;
    public TextMeshProUGUI counterText;

    private int itemSpawnCount;
    private int currentItemCount = 0;
    public bool isPaused = false;
    bool settingsOpen = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;      
    public AudioClip victoryClip;        
    public float victoryVolume = 1.0f;   

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }
    void Start()
    {
        panels.SetActive(true);
        panels.SetActive(false);

        fadeDissolve.location = 0f;
        fadeDissolve.DissolveOut();
        Timer.Instance.StartTimer();
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
        if (audioSource != null && victoryClip != null)
            audioSource.PlayOneShot(victoryClip, victoryVolume);
        Time.timeScale = 0f;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
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
        UnityEngine.Cursor.visible = isPaused;
    }
    public void OpenSettings()
    {
        settingsOpen = true;
    }
    public void CloseSettings()
    {
        settingsOpen = false;
    }
    public void Resume() 
    {
        isPaused = false;
        settingsOpen = false;
        Time.timeScale = 1f;
        panels.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}

using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    public TextMeshProUGUI timerText;
    private float elapsed = 0f;
    private bool isRunning = false;
    void Awake()
    {
        Instance = this;
    }
    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public float GetElapsed() => elapsed;
    void Start()
    {
        StartTimer();
        //timerText.gameObject.SetActive(PlayerPrefs.GetString("TimerDarkUISwitch", "false") == "true");
        string saved = PlayerPrefs.GetString("TimerDarkUISwitch", "false");
        timerText.gameObject.SetActive(saved == "true");
    }
    void Update()
    {
        if (!isRunning) return;
        elapsed += Time.deltaTime;

        int min = (int)(elapsed / 60);
        int sec = (int)(elapsed % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }
    public void SetTimerVisible(bool value)
    {
        timerText.gameObject.SetActive(value);
    }
}
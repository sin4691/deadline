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
    }
    void Update()
    {
        if (!isRunning) return;
        elapsed += Time.deltaTime;

        int min = (int)(elapsed / 60);
        int sec = (int)(elapsed % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }
}
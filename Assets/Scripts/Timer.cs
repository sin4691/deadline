using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsed = 0f;
    private bool isRunning = false;

    public void StartTimer() => isRunning = true;

    void Update()
    {
        if (!isRunning) return;
        elapsed += Time.deltaTime;

        int min = (int)(elapsed / 60);
        int sec = (int)(elapsed % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }
}
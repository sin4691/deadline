using TMPro;
using UnityEngine;

public class ClearPanel : MonoBehaviour
{
    public TextMeshProUGUI clearTimeText;
    public TextMeshProUGUI bestTimeText;

    void OnEnable()
    {
        float time = Timer.Instance.GetElapsed();
        int min = (int)(time / 60);
        int sec = (int)(time % 60);
        clearTimeText.text = $"{min:00}:{sec:00}";

        // 베스트 타임 갱신
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (time < bestTime)
        {
            bestTime = time;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        // 베스트 타임 표시
        if (bestTime == float.MaxValue)
            bestTimeText.text = "--:--";
        else
        {
            int bestMin = (int)(bestTime / 60);
            int bestSec = (int)(bestTime % 60);
            bestTimeText.text = $"{bestMin:00}:{bestSec:00}";
        }
    }
}
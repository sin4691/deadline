using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int itemSpawnCount;
    private int currentItemCount = 0;
    public TextMeshProUGUI counterText;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
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
    }
    void UpdateUI()
    {
        counterText.text = $"{currentItemCount} / {itemSpawnCount}";
    }
}

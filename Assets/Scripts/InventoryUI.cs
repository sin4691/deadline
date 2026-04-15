using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public RectTransform[] slots;
    public RectTransform highlight;
    public Image[] slotImages;
    [Header("highlight")]
    public float highlightSpeed = 10f;
    private void LateUpdate()
    {
        int currentIndex = inventory.GetCurrentSlotIndex();
        Vector3 targetPos = slots[currentIndex].position;
        highlight.position = Vector3.Lerp(highlight.position, targetPos, Time.deltaTime * highlightSpeed);
    }
    public void UpdateSlotUI(int slotIndex, ItemData itemData)
    {
        if (itemData == null)
        {
            slotImages[slotIndex].sprite =null;
            slotImages[slotIndex].enabled = false;
            return;
        }
        slotImages[slotIndex].sprite = itemData.icon;
        slotImages[slotIndex].enabled = true;
    }
}

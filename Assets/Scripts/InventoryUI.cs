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
    public void UpdateSlotUI(int index, ItemData data)
    {
        if (data != null && data.icon != null)
        {
            RuntimePreviewGenerator.BackgroundColor = new Color(1f, 1f, 1f, 0f);
            RuntimePreviewGenerator.PreviewDirection = new Vector3(1f, -0.5f, 1f);
            RuntimePreviewGenerator.OrthographicMode = true;
            RuntimePreviewGenerator.RenderSupersampling = 2.0f;
            Texture2D previewTexture = RuntimePreviewGenerator.GenerateModelPreview(data.icon.transform, 128, 128);
            if(previewTexture != null)
            {
                Sprite iconSprite = Sprite.Create(previewTexture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
                slotImages[index].gameObject.SetActive(true);
                slotImages[index].sprite = iconSprite;
                slotImages[index].enabled = true;
                slotImages[index].color = Color.white;
            }
        }
        else
        {
            slotImages[index].enabled = false;
        }
    }
}

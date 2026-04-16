using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class Inventory : MonoBehaviour
{
    public ItemData[] slots = new ItemData[4];
    public Transform itemHolder;
    public float pickupRange = 3f;
    public TextMeshProUGUI pickSub;
    public InventoryUI inventoryUI;
    public Vector3 handOffset = new Vector3(0.4f, -0.4f, 0.7f);
    public float scrollCooldown = 0.15f;

    private float lastScrollTime;
    private int currentSlotIndex;
    private GameObject currentActiveModel;
    private ItemWorld targetItem;
    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            inventoryUI.UpdateSlotUI(i, slots[i]);
        }
        UpdateHandleModel();
    }
    void Update()
    {
        Vector3 targetPos = Camera.main.transform.TransformPoint(handOffset);
        itemHolder.position = targetPos;
        itemHolder.rotation = Camera.main.transform.rotation;
        CheckInteraction();
    }
    private void OnInteract(InputValue value)
    {
        if (value.isPressed && targetItem != null)
        {
            PerformPickUp(targetItem);
        }
    }
    private void OnDrop(InputValue value) => Drop(value.isPressed);
    private void OnNext(InputValue value)
    {
        if (Time.time - lastScrollTime < scrollCooldown) return;
        currentSlotIndex = (currentSlotIndex + 1) % slots.Length;
        UpdateHandleModel();
        lastScrollTime = Time.time;
    }
    private void OnPrevious(InputValue value)
    {
        if (Time.time - lastScrollTime < scrollCooldown) return;
        currentSlotIndex--;

        if (currentSlotIndex < 0)
        {
            currentSlotIndex = slots.Length - 1;
        }
        UpdateHandleModel();
        lastScrollTime = Time.time;
    }

    private void Drop(bool drop)
    {
        if (!drop || slots[currentSlotIndex] == null) return;
        Vector3 spawnPos = GetDropPosition();
        float playerYaw = transform.eulerAngles.y;
        Quaternion spawnLot = Quaternion.Euler(slots[currentSlotIndex].itemPrefab.transform.eulerAngles.x, playerYaw, 0);
        Instantiate(slots[currentSlotIndex].itemPrefab, spawnPos, spawnLot);
        slots[currentSlotIndex] = null;
        inventoryUI.UpdateSlotUI(currentSlotIndex, null);
        UpdateHandleModel();
    }
    private Vector3 GetDropPosition()
    {
        Vector3 desiredPos = itemHolder.position;
        Vector3 playerCenter = transform.position + Vector3.up * 0.5f;
        if (Physics.Linecast(playerCenter, desiredPos, out RaycastHit wallHit))
        {
            // 벽에 막히면 벽 바로 앞으로 보정
            desiredPos = wallHit.point + wallHit.normal * 0.3f;
        }
        // 바닥 너무 가까우면 살짝 위로
        if (Physics.Raycast(desiredPos + Vector3.up * 0.5f, Vector3.down, out RaycastHit groundHit, 1f))
        {
            if (desiredPos.y - groundHit.point.y < 0.15f)
                desiredPos.y = groundHit.point.y + 0.15f;
        }

        return desiredPos;
    }
    // 상호작용 체크
    private void CheckInteraction()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Item") && hit.collider.TryGetComponent(out ItemWorld itemWorld))
            {
                targetItem = itemWorld; 
                pickSub.text = "Press [E] to Pick Up";
                pickSub.gameObject.SetActive(true);
                return;
            }
        }
        targetItem = null;
        pickSub.gameObject.SetActive(false);

    }

    // 인벤토리 빈 슬롯 찾기
    private void PerformPickUp(ItemWorld item)
    {
        if (slots[currentSlotIndex] == null)
        {
            AddItemToSlot(currentSlotIndex, item);
        }
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    AddItemToSlot(i, item);
                    return;
                }
            }
        }
    }
    private void UpdateHandleModel()
    {
        if (currentActiveModel != null)
        {
            Destroy(currentActiveModel);
            currentActiveModel = null;
        }
        if (slots[currentSlotIndex] != null && slots[currentSlotIndex].modelPrefab != null)
        {
            currentActiveModel = Instantiate(slots[currentSlotIndex].modelPrefab,itemHolder);
            currentActiveModel.transform.localPosition = Vector3.zero;
            currentActiveModel.transform.localRotation = Quaternion.identity;
        } 
    }

    public int GetCurrentSlotIndex(){ return currentSlotIndex;}

    private void AddItemToSlot(int index, ItemWorld itemWorld)
    {
        slots[index] = itemWorld.itemData;
        Destroy(itemWorld.gameObject);
        inventoryUI.UpdateSlotUI(index, slots[index]);
        if (index == currentSlotIndex)
            UpdateHandleModel();
    }
}

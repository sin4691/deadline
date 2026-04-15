using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public ItemData[] slots = new ItemData[4];
    public Transform itemHolder;
    public float pickupRange = 3f;
    public InventoryUI inventoryUI;
    public Vector3 handOffset = new Vector3(0.4f, -0.4f, 0.7f);
    private int currentSlotIndex;
    private GameObject currentActiveModel;
    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            inventoryUI.UpdateSlotUI(i, slots[i]);
        }
        UpdateHandleModel();
    }
    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            PickUp();
        }
    } 
    private void OnDrop(InputValue value) => Drop(value.isPressed);
    private void OnNext(InputValue value)
    {
        currentSlotIndex = (currentSlotIndex + 1) % slots.Length;
        UpdateHandleModel();
    }
    private void OnPrevious(InputValue value)
    {
        currentSlotIndex--;

        if (currentSlotIndex < 0)
        {
            currentSlotIndex = slots.Length - 1;
        }
        UpdateHandleModel();
    }
    void Update()
    {
        Vector3 targetPos = Camera.main.transform.TransformPoint(handOffset);
        itemHolder.position = targetPos;

        itemHolder.rotation = Camera.main.transform.rotation;
    }
    private void Drop(bool drop)
    {
        if (!drop || slots[currentSlotIndex] == null) return;
        Vector3 spawnPos = itemHolder.position;
        float playerYaw = transform.eulerAngles.y;
        Quaternion spawnLot = Quaternion.Euler(slots[currentSlotIndex].itemPrefab.transform.eulerAngles.x, playerYaw, 0);
        Instantiate(slots[currentSlotIndex].itemPrefab, spawnPos, spawnLot);
        slots[currentSlotIndex] = null;
        inventoryUI.UpdateSlotUI(currentSlotIndex, null);
        UpdateHandleModel();
    }
    private void PickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 1.0f);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.TryGetComponent(out ItemWorld itemWorld))
            {
                if (slots[currentSlotIndex] == null)
                {
                    AddItemToSlot(currentSlotIndex, itemWorld);
                    return;
                }
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == null)
                    {
                        AddItemToSlot(i, itemWorld);
                        return;
                    }
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
    public int GetCurrentSlotIndex()
    {
        return currentSlotIndex;
    }
    private void AddItemToSlot(int index, ItemWorld itemWorld)
    {
        slots[index] = itemWorld.itemData;
        Destroy(itemWorld.gameObject);
        inventoryUI.UpdateSlotUI(index, slots[index]);
        if (index == currentSlotIndex)
            UpdateHandleModel();
    }
}

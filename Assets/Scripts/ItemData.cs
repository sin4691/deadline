using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
    public GameObject modelPrefab;
    public Sprite icon;
}

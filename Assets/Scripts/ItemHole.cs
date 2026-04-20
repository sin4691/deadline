using UnityEngine;

public class ItemHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Destroy(other.gameObject);

            GameManager.Instance.AddItemCount();
        }
    }
}

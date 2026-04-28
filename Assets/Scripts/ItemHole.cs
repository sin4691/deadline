using UnityEngine;

public class ItemHole : MonoBehaviour
{
    [Header("사운드 설정")]
    public AudioSource audioSource; 
    public float skipSeconds = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (audioSource != null)
            {
                audioSource.time = skipSeconds;
                audioSource.Play();
            }

            GameManager.Instance.AddItemCount();

            Destroy(other.gameObject);
        }
    }
}
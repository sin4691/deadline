using NUnit.Framework.Interfaces;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;

    [Header("낙하 방지 설정")]
    public float fallThreshold = -50f; // 이 값보다 y가 작아지면 리스폰
    public Vector3 respawnPosition = new Vector3(0, -3, 0);
    private void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            RespawnItem();
        }
    }
    private void RespawnItem()
    {
        transform.position = respawnPosition;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.ResetInertiaTensor();
        }
    }
    private void OnValidate()
    {
        InitializeView();
    }

    public void InitializeView()
    {
        if (itemData != null && itemData.modelPrefab != null)
        {
            MeshFilter modelMesh = itemData.modelPrefab.GetComponentInChildren<MeshFilter>();
            MeshRenderer modelRenderer = itemData.modelPrefab.GetComponentInChildren<MeshRenderer>();

            if (modelMesh != null)
            {
                GetComponent<MeshFilter>().sharedMesh = modelMesh.sharedMesh;

                MeshCollider meshCol = GetComponent<MeshCollider>();
                if (meshCol != null)
                {
                    meshCol.sharedMesh = modelMesh.sharedMesh;
                    meshCol.convex = true;
                }
            }

            if (modelRenderer != null)
            {
                GetComponent<MeshRenderer>().sharedMaterials = modelRenderer.sharedMaterials;
            }
        }
    }
}

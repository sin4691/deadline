using NUnit.Framework.Interfaces;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
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

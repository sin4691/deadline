using DunGen;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonNavMesh : MonoBehaviour
{
    NavMeshSurface surface;
    RuntimeDungeon runtimeDungeon;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
        runtimeDungeon = GetComponent<RuntimeDungeon>();

        runtimeDungeon.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
    }

    void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
    {
        if (status == GenerationStatus.Complete)
        {
            StartCoroutine(BakeRoutine());
        }
    }

    IEnumerator BakeRoutine()
    {
        yield return null;

        surface.RemoveData();
        surface.BuildNavMesh();
    }
}
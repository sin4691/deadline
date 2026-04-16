using DunGen;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonNavMesh : MonoBehaviour
{
    NavMeshSurface surface;
    RuntimeDungeon runtimeDungeon;

    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        runtimeDungeon = GetComponent<RuntimeDungeon>();

        runtimeDungeon.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
    }

    void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
    {
        // 생성 완료 시에만 베이크
        if (status == GenerationStatus.Complete)
        {
            surface.BuildNavMesh();
            Debug.Log("NavMesh 베이크 완료");
        }
    }
}
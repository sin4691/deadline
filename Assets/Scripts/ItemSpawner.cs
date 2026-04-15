using DunGen;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private int itemSpawnCount = 5;
    public List<ItemData> itemPool;
    private void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }
    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(2.0f);

        SpawnItems();
    }
    public void SpawnItems()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("ItemPoint");

        List<GameObject> pointList = new List<GameObject>(spawnPoints);
        pointList = pointList.OrderBy(x => UnityEngine.Random.value).ToList(); ;

        for(int i = 0; i < itemSpawnCount && i < pointList.Count; i++)
        {
            ItemData randomData = itemPool[UnityEngine.Random.Range(0, itemPool.Count)];

            Instantiate(randomData.itemPrefab, pointList[i].transform.position, randomData.itemPrefab.transform.rotation);
        }
    }
}

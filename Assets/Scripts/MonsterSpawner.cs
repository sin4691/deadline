using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monster;

    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }
    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(2.0f);

        MonsterSpawn();
    }
    private void MonsterSpawn()
    {
        GameObject spawnPoint = GameObject.FindWithTag("MonsterPoint");
        Instantiate(monster, spawnPoint.transform.position, Quaternion.identity);
    }
}

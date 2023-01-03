using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemySpawner : MonoBehaviour
{

    public Collider spawnBounds;

    public List<EnemyChance> enemyList;

    public Transform player;

    //public float enemiesToSpawn;

    public float timeElapsed;

    void Start()
    {
        InvokeRepeating("EnemySpawnTimer", 0, 10);
    }
    private void Update()
    {
        timeElapsed += Time.deltaTime; 
        //enemiesToSpawn = Mathf.Ceil(0.5f * Mathf.Log(timeElapsed));
    }
    private void EnemySpawnTimer()
    {
        /*for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }*/
        foreach (EnemyChance chance in enemyList)
        {
            GameObject prefab = chance.enemyPrefab;
            int num = chance.numberToSpawn;
            for (int i = 0; i < num; i++)
            {
                SpawnEnemy(prefab);
            }
        }
    }
    private void SpawnEnemy(GameObject prefab)
    {
        spawnBounds.enabled = true;
        Vector3 point = RandomPointInBounds(spawnBounds.bounds);
        spawnBounds.enabled = false;
        GameObject enemy = Instantiate(prefab, point, Quaternion.identity);
        AIDestinationSetter pathSetter = enemy.GetComponent<AIDestinationSetter>();
        pathSetter.target = player;
    }
    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }


}

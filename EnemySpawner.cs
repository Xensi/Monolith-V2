using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemySpawner : MonoBehaviour
{
     

    public List<EnemyChance> enemyList;

    public Transform player;

    //public float enemiesToSpawn;

    public float timeElapsed;

    public float timeToSpawn = 15;
    public float gracePeriod = 5;
    public float difficultyTime = 60;

    public Transform min;
    public Transform max;
    public static EnemySpawner Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself. 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        InvokeRepeating("EnemySpawnTimer", gracePeriod, timeToSpawn);
        InvokeRepeating("DifficultyIncrease", difficultyTime, difficultyTime);
    }
    private void Update()
    {
        timeElapsed += Time.deltaTime; 
        //enemiesToSpawn = Mathf.Ceil(0.5f * Mathf.Log(timeElapsed));
    }
    private void DifficultyIncrease()
    {
        enemyList[Random.Range(0, enemyList.Count)].numberToSpawn++;
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
        
        GameObject enemy = Instantiate(prefab, RandomPoint(), Quaternion.identity);
    }
    public Vector3 RandomPoint()
    {
        Vector3 minPos = min.position;
        Vector3 maxPos = max.position;
        return new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));
    }
}

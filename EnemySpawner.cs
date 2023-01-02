using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemySpawner : MonoBehaviour
{

    public Collider spawnBounds;

    public GameObject enemyPrefab;

    public Transform player;
    void Start()
    {
        InvokeRepeating("EnemySpawnTimer", 0, 2);
    } 

    private void EnemySpawnTimer()
    {
        spawnBounds.enabled = true;
        Vector3 point = RandomPointInBounds(spawnBounds.bounds);
        spawnBounds.enabled = false;
        GameObject enemy = Instantiate(enemyPrefab, point, Quaternion.identity);
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

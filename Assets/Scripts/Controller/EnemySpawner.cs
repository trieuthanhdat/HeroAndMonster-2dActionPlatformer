using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<IAIController> enemyPrefabs; // The prefabs for the enemies to spawn
    public float spawnDelay = 1.0f; // The delay between enemy spawns
    public float spawnRadius = 1.0f; // The radius around the spawning surface to spawn enemies
    public float spawnFrequency = 5f;
    public float firstDelaySpawnTime = 1f;
    public float limitSpawnTimeRange = 6f;

    private Collider2D surface;
    private int totalEnemySpawn;
    private bool spawnOnce = false;
    public List<IAIController> spawnedEnemies;

    private void Start()
    {
        surface = GetComponent<Collider2D>();
        spawnedEnemies = new List<IAIController>(enemyPrefabs.Count);
        InvokeRepeating("StartSpawning", firstDelaySpawnTime, spawnFrequency);
        totalEnemySpawn = enemyPrefabs.Count/2;
    }

    public void StartSpawning()
    {
        if(surface == null || enemyPrefabs == null|| enemyPrefabs.Count <= 0) return;
        
        if(spawnedEnemies.Count >= totalEnemySpawn) 
        {
            spawnOnce = true;
        }
        if(!spawnOnce)
        {
            for(int i = 0; i < enemyPrefabs.Count; i++)
            {
                if(!spawnedEnemies.Contains(enemyPrefabs[i])) 
                {
                    StartCoroutine(SpawnEnemyOnCollider(surface, enemyPrefabs[i], i)); 
                }
            }   
        }
        else
        {
            if(spawnedEnemies.Count <= 0 || spawnedEnemies == null) return;
            try
            {

                for(int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if(!spawnedEnemies[i].gameObject.activeInHierarchy && spawnedEnemies[i] != null)
                    {
                        if(spawnedEnemies[i].GetComponent<SnailEnemyAIController>() != null)
                        {
                            if(spawnedEnemies[i].GetComponent<SnailEnemyAIController>().IsDead())
                            {
                                spawnedEnemies.Remove(spawnedEnemies[i]);
                                continue;
                            }
                        }
                        StartCoroutine(SpawnExistEnemyOnCollider(i));
                    }
                }
            }catch
            {
                
            }
        }
    }
    
    
    public IEnumerator SpawnEnemyOnCollider(Collider2D spawningSurface, IAIController enemyPrefab, int index)
    {
         float waitTime = Random.Range(2, limitSpawnTimeRange);
        yield return new WaitForSeconds(waitTime);

        Bounds bounds = spawningSurface.bounds;
        float spawnX = Random.Range(bounds.min.x, bounds.max.x);
        Vector2 spawnPoint = new Vector2(spawnX, bounds.max.y);

        IAIController enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }
    
    public IEnumerator SpawnExistEnemyOnCollider(int targetIndex)
    {
        float waitTime = Random.Range(2, limitSpawnTimeRange);
        yield return new WaitForSeconds(waitTime);

        if(spawnedEnemies.Contains(spawnedEnemies[targetIndex]) && !spawnedEnemies[targetIndex].GetComponent<SnailEnemyAIController>().IsDead())
        {
            Bounds bounds = surface.bounds;
            float spawnX = Random.Range(bounds.min.x, bounds.max.x);
            Vector2 spawnPoint = new Vector2(spawnX, bounds.max.y);
            spawnedEnemies[targetIndex].gameObject.SetActive(true);
            spawnedEnemies[targetIndex].transform.position = spawnPoint;
        }
    }


}

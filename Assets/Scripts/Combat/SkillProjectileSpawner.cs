using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectileSpawner : MonoBehaviour
{
   public GameObject projectilePrefab;
    public float spawnDelay = 1f;
    public float projectileSpeed = 10f;
    public float screenEdgeOffset = 1f;

    private void Start()
    {
        StartCoroutine(SpawnProjectiles());
    }

    private IEnumerator SpawnProjectiles()
    {
        int counter = 0;
        while (true)
        {
            int numProjectiles = Random.Range(3, 6);
            for (int i = 0; i < numProjectiles; i++)
            {
                counter ++;
                Vector2 spawnPosition = GetScreenEdgePosition();
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
                projectile.transform.localScale *= 2;
                LaunchProjectile(spawnPosition, projectile);

                
                yield return new WaitForSeconds(0.1f);
            }
            if(counter>= numProjectiles)
            {
                Destroy(gameObject, spawnDelay);
                break;
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void LaunchProjectile(Vector2 firePoint, GameObject projectileInstance)
    {
        // Instantiate the projectile prefab at the fire point

        // Calculate the direction vector towards the player
        Vector2 truePosPlayer = new Vector2( GameObject.FindGameObjectWithTag("Player").transform.position.x,GameObject.FindGameObjectWithTag("Player").transform.position.y);
        Vector2 direction = (truePosPlayer - (Vector2)firePoint).normalized;
        // Set the velocity of the projectile
        projectileInstance.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }

    private Vector2 GetScreenEdgePosition()
    {
        float randX = Screen.width;
        float randY = Screen.height;
        if (Random.Range(0, 2) == 0)
        {
            randX += screenEdgeOffset;
            randY = Random.Range(0, Screen.height);
        }
        else
        {
            randX = Random.Range(0, Screen.width);
            randY += screenEdgeOffset;
        }
        Vector3 screenPosition = new Vector3(randX, randY, 0f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

}

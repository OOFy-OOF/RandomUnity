using UnityEngine;
public class SpawnManager : MonoBehaviour
{
    public GameObject obstaclePrefab;  // Prefab reference for spawning clones
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float obstacleHeightRange = 9f;

    private Vector3 initialSpawnPosition; // To store the initial obstacle spawn position
    private bool isSpawning = true;  // To track if obstacles are currently being spawned

    void Start()
    {
        // Save the original X and Z position of the obstacle (Y will be random)
        initialSpawnPosition = new Vector3(obstaclePrefab.transform.position.x, 0, obstaclePrefab.transform.position.z);
    }

    public void SpawnObstacle()
    {
        if (isSpawning)
        {
            // Ensure the Y position is between 0 and the max height range
            float randomYPosition = Random.Range(0, obstacleHeightRange);

            // Spawn a new obstacle at the original X and Z positions but with a random Y height
            Vector3 spawnPosition = new Vector3(initialSpawnPosition.x, randomYPosition, initialSpawnPosition.z);
            Instantiate(obstaclePrefab, spawnPosition, obstaclePrefab.transform.rotation);

            // Schedule the next obstacle spawn after a random interval
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            Invoke("SpawnObstacle", spawnInterval);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke("SpawnObstacle");
    }

    public void StartSpawning()
    {
        isSpawning = true;
        SpawnObstacle();
    }

    public void NotifyObstacleDestroyed()
    {
        // Additional logic if needed in the future, IDK what to do now
    }
}

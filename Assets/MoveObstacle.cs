using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    public float speed = 5f;
    public float offScreenZ = -25f;
    private PlayerController playerController;
    private SpawnManager spawnManager;
    private bool isStopped = false;  // To track if the obstacle movement is stopped
    private bool isPassed = false; // To track if the obstacle has been passed




    void Start()
    {
        // Find the SpawnManager instance
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        // Find the PlayerController instance
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!isStopped)
        {
            // Move the obstacle backward
            transform.Translate(Vector3.back * speed * Time.deltaTime);

            if (!playerController.isGameOver && !isPassed && transform.position.z < playerController.transform.position.z)
            {
                // Increment the score when the obstacle is passed
                playerController.IncrementScore();
                isPassed = true;
            }

            // Check if the obstacle has moved off-screen
            if (transform.position.z < offScreenZ)
            {
                // Notify the SpawnManager (if needed)
                if (spawnManager != null)
                {
                    spawnManager.NotifyObstacleDestroyed();
                }

                // Destroy the obstacle to free up resources
                Destroy(gameObject);
            }
        }
    }

    public void StopObstacle()
    {
        isStopped = true;
    }
    public void StartObstacle()
    {
        isStopped = false;
    }
}
